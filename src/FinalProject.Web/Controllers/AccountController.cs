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

        public AccountController(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
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

            await SignInUser(customer, isPersistent: false);
            return RedirectToAction("Index", "Dashboard");
        }

        // ===== LOGOUT =====
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
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

            await SignInUser(customer, isPersistent: false);
            return RedirectToAction("IndexAr", "Dashboard");
        }

        [HttpGet]
        public async Task<IActionResult> LogoutAr()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("IndexAr", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied() => View();

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

        private async Task SignInUser(User user, bool isPersistent)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.UserId.ToString()), // bridge property
                new(ClaimTypes.Name,           user.Username),           // bridge property
                new(ClaimTypes.Role,           user.Role.ToString()),
                new("FullName",                user.FullName)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties { IsPersistent = isPersistent });
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