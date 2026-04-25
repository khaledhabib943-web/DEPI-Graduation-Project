using FinalProject.Application.Interfaces;
using FinalProject.Domain.Entities;
using FinalProject.Domain.Enums;
using FinalProject.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FinalProject.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ===== ENGLISH =====
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectByRole();
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

            // Redirect based on role
            return user.Role switch
            {
                UserRole.Admin => RedirectToAction("Index", "AdminDashboard"),
                UserRole.Worker => RedirectToAction("Index", "WorkerDashboard"),
                _ => !string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl)
                    ? Redirect(model.ReturnUrl)
                    : RedirectToAction("Index", "Dashboard")
            };
        }

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

            // Check if username exists
            var existingUsername = await _unitOfWork.Customers.FindAsync(c => c.Username == model.Username);
            if (existingUsername.Any())
            {
                model.ErrorMessage = "This username is already taken.";
                return View(model);
            }

            // Check if email exists (across all user types)
            var existingEmail = await _unitOfWork.Customers.FindAsync(c => c.Email == model.Email);
            if (existingEmail.Any())
            {
                model.ErrorMessage = "This email is already registered.";
                return View(model);
            }

            // Check if National ID exists
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
                PasswordHash = HashPassword(model.Password),
                PhoneNumber = model.PhoneNumber.Trim(),
                NationalId = model.NationalId.Trim(),
                Age = model.Age,
                Username = model.Username.Trim(),
                Role = UserRole.Customer,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Address = model.Address.Trim()
            };

            await _unitOfWork.Customers.AddAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            await SignInUser(customer, false);
            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // ===== ARABIC =====
        [HttpGet]
        public IActionResult LoginAr(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectByRole(arabic: true);
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

            var existingUsername = await _unitOfWork.Customers.FindAsync(c => c.Username == model.Username);
            if (existingUsername.Any())
            {
                model.ErrorMessage = "اسم المستخدم مسجل مسبقاً.";
                return View(model);
            }

            var existingEmail = await _unitOfWork.Customers.FindAsync(c => c.Email == model.Email);
            if (existingEmail.Any())
            {
                model.ErrorMessage = "البريد الإلكتروني مسجل مسبقاً.";
                return View(model);
            }

            var existingNid = await _unitOfWork.Customers.FindAsync(c => c.NationalId == model.NationalId);
            if (existingNid.Any())
            {
                model.ErrorMessage = "الرقم القومي مسجل مسبقاً.";
                return View(model);
            }

            var customer = new Customer
            {
                FullName = model.FullName.Trim(), Email = model.Email.Trim().ToLowerInvariant(),
                PasswordHash = HashPassword(model.Password), PhoneNumber = model.PhoneNumber.Trim(),
                NationalId = model.NationalId.Trim(), Age = model.Age, Username = model.Username.Trim(),
                Role = UserRole.Customer, IsActive = true, CreatedAt = DateTime.UtcNow, Address = model.Address.Trim()
            };

            await _unitOfWork.Customers.AddAsync(customer);
            await _unitOfWork.SaveChangesAsync();
            await SignInUser(customer, false);
            return RedirectToAction("IndexAr", "Dashboard");
        }

        [HttpGet]
        public async Task<IActionResult> LogoutAr()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("IndexAr", "Home");
        }

        // ===== Access Denied =====
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // ===== Helpers =====
        private async Task<User?> FindUserByCredentials(string usernameOrEmail, string password)
        {
            var hash = HashPassword(password);

            // Search Customers
            var customers = await _unitOfWork.Customers.FindAsync(c =>
                (c.Username == usernameOrEmail || c.Email == usernameOrEmail) && c.PasswordHash == hash);
            var customer = customers.FirstOrDefault();
            if (customer != null) return customer;

            // Search Workers
            var workers = await _unitOfWork.Workers.FindAsync(w =>
                (w.Username == usernameOrEmail || w.Email == usernameOrEmail) && w.PasswordHash == hash);
            var worker = workers.FirstOrDefault();
            if (worker != null) return worker;

            // Search Admins
            var admins = await _unitOfWork.Admins.FindAsync(a =>
                (a.Username == usernameOrEmail || a.Email == usernameOrEmail) && a.PasswordHash == hash);
            return admins.FirstOrDefault();
        }

        private async Task SignInUser(User user, bool isPersistent)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Role, user.Role.ToString()),
                new("FullName", user.FullName)
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

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
