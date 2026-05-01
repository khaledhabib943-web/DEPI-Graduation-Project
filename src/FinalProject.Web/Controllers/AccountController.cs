using FinalProject.Application.Interfaces;
using FinalProject.Domain.Entities;
using FinalProject.Domain.Enums;
using FinalProject.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinalProject.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailSender _emailSender;

        public AccountController(IUnitOfWork unitOfWork, UserManager<User> userManager, SignInManager<User> signInManager, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        // ================= LOGIN =================
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true) return RedirectByRole();
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await FindUserByCredentials(model.Username, model.Password);
            if (user == null)
            {
                model.ErrorMessage = "Invalid username or password.";
                return View(model);
            }

            if (!user.IsActive)
            {
                model.ErrorMessage = "Your account is deactivated.";
                return View(model);
            }

            // ── 2FA check ──
            if (user.TwoFactorEnabled)
            {
                TempData["2fa_UserId"] = user.Id;
                TempData["2fa_RememberMe"] = model.RememberMe;
                TempData["2fa_ReturnUrl"] = model.ReturnUrl;
                return RedirectToAction("Verify2fa");
            }

            await SignInUser(user, model.RememberMe);
            return user.Role switch
            {
                UserRole.Admin => RedirectToAction("Index", "AdminDashboard"),
                UserRole.Worker => RedirectToAction("Index", "WorkerDashboard"),
                _ => !string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl)
                    ? Redirect(model.ReturnUrl)
                    : RedirectToAction("Index", "Dashboard")
            };
        }

        // ================= VERIFY 2FA (English) =================
        [HttpGet]
        public IActionResult Verify2fa()
        {
            if (TempData.Peek("2fa_UserId") == null)
                return RedirectToAction("Login");

            return View(new Verify2faViewModel
            {
                RememberMe = TempData.Peek("2fa_RememberMe") as bool? ?? false,
                ReturnUrl = TempData.Peek("2fa_ReturnUrl") as string
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Verify2fa(Verify2faViewModel model)
        {
            var userId = TempData.Peek("2fa_UserId");
            if (userId == null) return RedirectToAction("Login");
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByIdAsync(userId.ToString()!);
            if (user == null) return RedirectToAction("Login");

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(
                user,
                _userManager.Options.Tokens.AuthenticatorTokenProvider,
                model.Code.Trim());

            if (!isValid)
            {
                model.ErrorMessage = "Invalid verification code. Please try again.";
                return View(model);
            }

            TempData.Remove("2fa_UserId");
            TempData.Remove("2fa_RememberMe");
            TempData.Remove("2fa_ReturnUrl");

            await SignInUser(user, model.RememberMe);
            return RedirectByRole();
        }

        // ================= REGISTER =================
        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Dashboard");
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var existingEmail = await _unitOfWork.Customers.FindAsync(c => c.Email == model.Email);
            if (existingEmail.Any()) { model.ErrorMessage = "هذا البريد الإلكتروني مسجل مسبقاً."; return View(model); }

            var existingNid = await _unitOfWork.Customers.FindAsync(c => c.NationalId == model.NationalId);
            if (existingNid.Any()) { model.ErrorMessage = "الرقم القومي مسجل مسبقاً."; return View(model); }

            var customer = new Customer
            {
                FullName = model.FullName.Trim(),
                Email = model.Email.Trim().ToLowerInvariant(),
                UserName = model.Username.Trim(),
                PhoneNumber = model.PhoneNumber.Trim(),
                NationalId = model.NationalId.Trim(),
                Age = model.Age,
                Role = UserRole.Customer,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Address = model.Address.Trim()
            };

            var result = await _userManager.CreateAsync(customer, model.Password);
            if (!result.Succeeded)
            {
                model.ErrorMessage = string.Join(" ", result.Errors.Select(e => e.Description));
                return View(model);
            }

            // Generate email confirmation token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(customer);
            var callbackUrl = Url.Action(
                "ConfirmEmail",
                "Account",
                new { userId = customer.Id, token = token },
                protocol: Request.Scheme);

            var emailSubject = "Confirm your Salahly account";
            var emailBody = $@"
                <h2>Welcome to Salahly!</h2>
                <p>Thank you for registering. Please confirm your email address by clicking the link below:</p>
                <p><a href='{callbackUrl}'>Confirm Email</a></p>
                <p>If you didn't create an account with Salahly, please ignore this email.</p>";

            await _emailSender.SendEmailAsync(customer.Email, emailSubject, emailBody);
            return RedirectToAction("RegisterConfirmation");
        }

        // ================= EMAIL CONFIRMATION =================
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(int userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return View("Error");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Dashboard");
            }

            return View("Error");
        }

        [HttpGet]
        public IActionResult RegisterConfirmation() => View();

        // ================= LOGIN AR =================
        [HttpGet]
        public IActionResult LoginAr(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true) return RedirectByRole(arabic: true);
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAr(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await FindUserByCredentials(model.Username, model.Password);
            if (user == null)
            {
                model.ErrorMessage = "اسم المستخدم أو كلمة المرور غير صحيحة.";
                return View(model);
            }

            if (!user.IsActive)
            {
                model.ErrorMessage = "تم إيقاف حسابك. يرجى التواصل مع الدعم الفني.";
                return View(model);
            }

            // ── 2FA check (Arabic) ──
            if (user.TwoFactorEnabled)
            {
                TempData["2fa_UserId"] = user.Id;
                TempData["2fa_RememberMe"] = model.RememberMe;
                TempData["2fa_ReturnUrl"] = model.ReturnUrl;
                TempData["2fa_Arabic"] = true;
                return RedirectToAction("Verify2faAr");
            }

            await SignInUser(user, model.RememberMe);
            return user.Role switch
            {
                UserRole.Admin => RedirectToAction("IndexAr", "AdminDashboard"),
                UserRole.Worker => RedirectToAction("IndexAr", "WorkerDashboard"),
                _ => RedirectToAction("IndexAr", "Dashboard")
            };
        }

        // ================= VERIFY 2FA (Arabic) =================
        [HttpGet]
        public IActionResult Verify2faAr()
        {
            if (TempData.Peek("2fa_UserId") == null)
                return RedirectToAction("LoginAr");

            return View(new Verify2faViewModel
            {
                RememberMe = TempData.Peek("2fa_RememberMe") as bool? ?? false,
                ReturnUrl = TempData.Peek("2fa_ReturnUrl") as string
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Verify2faAr(Verify2faViewModel model)
        {
            var userId = TempData.Peek("2fa_UserId");
            if (userId == null) return RedirectToAction("LoginAr");
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByIdAsync(userId.ToString()!);
            if (user == null) return RedirectToAction("LoginAr");

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(
                user,
                _userManager.Options.Tokens.AuthenticatorTokenProvider,
                model.Code.Trim());

            if (!isValid)
            {
                model.ErrorMessage = "رمز التحقق غير صالح. يرجى المحاولة مرة أخرى.";
                return View(model);
            }

            TempData.Remove("2fa_UserId");
            TempData.Remove("2fa_RememberMe");
            TempData.Remove("2fa_ReturnUrl");
            TempData.Remove("2fa_Arabic");

            await SignInUser(user, model.RememberMe);
            return user.Role switch
            {
                UserRole.Admin => RedirectToAction("IndexAr", "AdminDashboard"),
                UserRole.Worker => RedirectToAction("IndexAr", "WorkerDashboard"),
                _ => RedirectToAction("IndexAr", "Dashboard")
            };
        }

        // ================= REGISTER AR =================
        [HttpGet]
        public IActionResult RegisterAr()
        {
            if (User.Identity?.IsAuthenticated == true) return RedirectToAction("IndexAr", "Dashboard");
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAr(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var existingEmail = await _unitOfWork.Customers.FindAsync(c => c.Email == model.Email);
            if (existingEmail.Any()) { model.ErrorMessage = "هذا البريد الإلكتروني مسجل مسبقاً."; return View(model); }

            var existingNid = await _unitOfWork.Customers.FindAsync(c => c.NationalId == model.NationalId);
            if (existingNid.Any()) { model.ErrorMessage = "الرقم القومي مسجل مسبقاً."; return View(model); }

            var customer = new Customer
            {
                FullName = model.FullName.Trim(),
                Email = model.Email.Trim().ToLowerInvariant(),
                UserName = model.Username.Trim(),
                PhoneNumber = model.PhoneNumber.Trim(),
                NationalId = model.NationalId.Trim(),
                Age = model.Age,
                Role = UserRole.Customer,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Address = model.Address.Trim()
            };

            var result = await _userManager.CreateAsync(customer, model.Password);
            if (!result.Succeeded)
            {
                model.ErrorMessage = string.Join(" ", result.Errors.Select(e => e.Description));
                return View(model);
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(customer);
            var callbackUrl = Url.Action(
                "ConfirmEmail",
                "Account",
                new { userId = customer.Id, token = token },
                protocol: Request.Scheme);

            var emailSubject = "تأكيد حسابك في صالحly";
            var emailBody = $@"
                <h2>مرحباً بك في صالحly!</h2>
                <p>شكراً لتسجيلك. يرجى تأكيد عنوان بريدك الإلكتروني بالنقر على الرابط أدناه:</p>
                <p><a href='{callbackUrl}'>تأكيد البريد الإلكتروني</a></p>
                <p>إذا لم تقم بإنشاء حساب في صالحly، يرجى تجاهل هذا البريد الإلكتروني.</p>";

            await _emailSender.SendEmailAsync(customer.Email, emailSubject, emailBody);
            return RedirectToAction("RegisterConfirmationAr");
        }

        [HttpGet]
        public IActionResult RegisterConfirmationAr() => View();

        // ================= GOOGLE LOGIN =================
        [HttpPost]
        public IActionResult ExternalLogin(string provider)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        public async Task<IActionResult> ExternalLoginCallback()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null) return RedirectToAction("Login");

            var email = info.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new Customer { Email = email, UserName = email, FullName = info.Principal.FindFirst(ClaimTypes.Name)?.Value };
                await _userManager.CreateAsync(user);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Dashboard");
        }

        // ================= LOGOUT & ACCESS DENIED =================
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied() => View();

        // ================= HELPERS =================
        private async Task<User?> FindUserByCredentials(string usernameOrEmail, string password)
        {
            var user = await _userManager.FindByNameAsync(usernameOrEmail)
                       ?? await _userManager.FindByEmailAsync(usernameOrEmail);

            if (user == null) return null;
            return await _userManager.CheckPasswordAsync(user, password) ? user : null;
        }

        private async Task SignInUser(User user, bool isPersistent)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.Role, user.Role.ToString())
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, new ClaimsPrincipal(identity),
                new AuthenticationProperties { IsPersistent = isPersistent });
        }

        private IActionResult RedirectByRole(bool arabic = false)
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            string action = arabic ? "IndexAr" : "Index";
            return role switch
            {
                "Admin" => RedirectToAction(action, "AdminDashboard"),
                "Worker" => RedirectToAction(action, "WorkerDashboard"),
                _ => RedirectToAction(action, "Dashboard")
            };
        }
    }
}