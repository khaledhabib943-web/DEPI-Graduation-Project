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

        // ===== LOGIN (English) =====
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
                model.ErrorMessage = "Your account has been deactivated. Please contact support.";
                return View(model);
            }

            await _signInManager.SignInAsync(user, model.RememberMe);

            return user.Role switch
            {
                UserRole.Admin => RedirectToAction("Index", "AdminDashboard"),
                UserRole.Worker => RedirectToAction("Index", "WorkerDashboard"),
                _ => !string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl)
                    ? Redirect(model.ReturnUrl)
                    : RedirectToAction("Index", "Dashboard")
            };
        }

        // ===== REGISTER (English) =====
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

            // NationalId check — Identity doesn't know about this field
            var existingNid = await _unitOfWork.Customers.FindAsync(c => c.NationalId == model.NationalId);
            if (existingNid.Any())
            {
                model.ErrorMessage = "This National ID is already registered.";
                return View(model);
            }

            var customer = new Customer
            {
                FullName = model.FullName.Trim(),
                Email = model.Email.Trim().ToLowerInvariant(),
                UserName = model.Username.Trim(),   // Identity uses UserName
                PhoneNumber = model.PhoneNumber.Trim(),
                NationalId = model.NationalId.Trim(),
                Age = model.Age,
                Role = UserRole.Customer,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Address = model.Address.Trim()
            };

            // CreateAsync: hashes password + enforces unique Username & Email automatically
            var result = await _userManager.CreateAsync(customer, model.Password);
            if (!result.Succeeded)
            {
                // Identity gives clear messages like "Username already taken" / "Email already in use"
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

            // Send confirmation email
            var emailSubject = "Confirm your Salahly account";
            var emailBody = $@"
                <h2>Welcome to Salahly!</h2>
                <p>Thank you for registering. Please confirm your email address by clicking the link below:</p>
                <p><a href='{callbackUrl}'>Confirm Email</a></p>
                <p>If you didn't create an account with Salahly, please ignore this email.</p>";

            await _emailSender.SendEmailAsync(customer.Email, emailSubject, emailBody);

            // Show "Check your email" view instead of signing in
            return RedirectToAction("RegisterConfirmation");
        }

        // ===== LOGOUT =====
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // ===== EMAIL CONFIRMATION =====
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(int userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return View("Error");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                // Email confirmed successfully, sign in the user
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Dashboard");
            }

            return View("Error");
        }

        // ===== REGISTER CONFIRMATION VIEW =====
        [HttpGet]
        public IActionResult RegisterConfirmation()
        {
            return View();
        }

        // ===== ARABIC versions (same pattern) =====
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

            await _signInManager.SignInAsync(user, model.RememberMe);

            return user.Role switch
            {
                UserRole.Admin => RedirectToAction("IndexAr", "AdminDashboard"),
                UserRole.Worker => RedirectToAction("IndexAr", "WorkerDashboard"),
                _ => RedirectToAction("IndexAr", "Dashboard")
            };
        }

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

            var existingNid = await _unitOfWork.Customers.FindAsync(c => c.NationalId == model.NationalId);
            if (existingNid.Any())
            {
                model.ErrorMessage = "الرقم القومي مسجل مسبقاً.";
                return View(model);
            }

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

            // Send confirmation email
            var emailSubject = "تأكيد حسابك في صالحly";
            var emailBody = $@"
                <h2>مرحباً بك في صالحly!</h2>
                <p>شكراً لتسجيلك. يرجى تأكيد عنوان بريدك الإلكتروني بالنقر على الرابط أدناه:</p>
                <p><a href='{callbackUrl}'>تأكيد البريد الإلكتروني</a></p>
                <p>إذا لم تقم بإنشاء حساب في صالحly، يرجى تجاهل هذا البريد الإلكتروني.</p>";

            await _emailSender.SendEmailAsync(customer.Email, emailSubject, emailBody);

            // Show "Check your email" view instead of signing in
            return RedirectToAction("RegisterConfirmationAr");
        }

        [HttpGet]
        public async Task<IActionResult> LogoutAr()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("IndexAr", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied() => View();

        // ===== REGISTER CONFIRMATION VIEW (ARABIC) =====
        [HttpGet]
        public IActionResult RegisterConfirmationAr()
        {
            return View();
        }

        // ===== HELPERS =====

        private async Task<User?> FindUserByCredentials(string usernameOrEmail, string password)
        {
            // Try by username first, then by email
            var user = await _userManager.FindByNameAsync(usernameOrEmail)
                    ?? await _userManager.FindByEmailAsync(usernameOrEmail);

            if (user == null) return null;

            // UserManager uses Identity's secure PBKDF2 verifier (no more SHA256)
            var passwordValid = await _userManager.CheckPasswordAsync(user, password);
            return passwordValid ? user : null;
        }


        private IActionResult RedirectByRole(bool arabic = false)
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            return role switch
            {
                "Admin" => RedirectToAction(arabic ? "IndexAr" : "Index", "AdminDashboard"),
                "Worker" => RedirectToAction(arabic ? "IndexAr" : "Index", "WorkerDashboard"),
                _ => RedirectToAction(arabic ? "IndexAr" : "Index", "Dashboard")
            };
        }
    }
}