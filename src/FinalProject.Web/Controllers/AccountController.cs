using FinalProject.Application.Interfaces;
using FinalProject.Domain.Entities;
using FinalProject.Domain.Enums;
using FinalProject.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinalProject.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;  

        public AccountController(IUnitOfWork unitOfWork, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
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
                // Store user info in TempData so the Verify2fa action can retrieve it
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

        // ===== 2FA VERIFICATION (English) =====
        [HttpGet]
        public IActionResult Verify2fa()
        {
            // Ensure we came from the login flow
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

            // Clear TempData after successful verification
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

            // Validations
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

            await SignInUser(customer, false);
            return RedirectToAction("IndexAr", "Dashboard");
        }

        // ================= GOOGLE LOGIN =================
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

        // ===== 2FA VERIFICATION (Arabic) =====
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

        [HttpGet]
        public IActionResult RegisterAr()

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