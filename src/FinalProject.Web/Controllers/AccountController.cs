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

        // ================= LOGIN =================
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
                model.ErrorMessage = "Your account is deactivated.";
                return View(model);
            }

            await SignInUser(user, model.RememberMe);

            return RedirectByRole();
        }

        // ================= GOOGLE LOGIN START =================
        [HttpPost]
        public IActionResult ExternalLogin(string provider)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account");

            var properties = new AuthenticationProperties
            {
                RedirectUri = redirectUrl
            };

            return Challenge(properties, provider);
        }

        // ================= GOOGLE CALLBACK =================
        public async Task<IActionResult> ExternalLoginCallback()
        {
            var result = await HttpContext.AuthenticateAsync("Identity.External");

            if (!result.Succeeded)
                return RedirectToAction("Login");

            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;

            if (email == null)
                return RedirectToAction("Login");

            // search user in your system
            var users = await _unitOfWork.Customers.FindAsync(c => c.Email == email);
            var user = users.FirstOrDefault();

            // create if not exists
            if (user == null)
            {
                user = new Customer
                {
                    FullName = name ?? email,
                    Email = email,
                    Username = email,
                    PasswordHash = "GOOGLE_LOGIN",
                    Role = UserRole.Customer,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Customers.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();
            }

            // IMPORTANT: sign in using YOUR system
            await SignInUser(user, false);

            return RedirectToAction("Index", "Dashboard");
        }

        // ================= REGISTER =================
        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Dashboard");

            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var existingEmail = await _unitOfWork.Customers.FindAsync(c => c.Email == model.Email);
            if (existingEmail.Any())
            {
                model.ErrorMessage = "Email already exists.";
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

        // ================= LOGOUT =================
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // ================= HELPERS =================
        private async Task<User?> FindUserByCredentials(string usernameOrEmail, string password)
        {
            var hash = HashPassword(password);

            var customers = await _unitOfWork.Customers.FindAsync(c =>
                (c.Username == usernameOrEmail || c.Email == usernameOrEmail) &&
                c.PasswordHash == hash);

            return customers.FirstOrDefault();
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

        private IActionResult RedirectByRole()
        {
            var role = User.FindFirstValue(ClaimTypes.Role);

            return role switch
            {
                "Admin" => RedirectToAction("Index", "AdminDashboard"),
                "Worker" => RedirectToAction("Index", "WorkerDashboard"),
                _ => RedirectToAction("Index", "Dashboard")
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
