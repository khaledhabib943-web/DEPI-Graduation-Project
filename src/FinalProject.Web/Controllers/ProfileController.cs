using FinalProject.Application.Interfaces;
using FinalProject.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinalProject.Web.Controllers
{
    [Authorize(Roles = "Customer")]
    public class ProfileController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProfileController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(GetUserId());
            if (customer == null) return RedirectToAction("Login", "Account");

            return View(new ProfileViewModel
            {
                UserId = customer.UserId, FullName = customer.FullName, Email = customer.Email,
                PhoneNumber = customer.PhoneNumber, Age = customer.Age, Address = customer.Address
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ProfileViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var customer = await _unitOfWork.Customers.GetByIdAsync(GetUserId());
            if (customer == null) return RedirectToAction("Login", "Account");

            // Check if email changed and is unique
            if (customer.Email != model.Email.Trim().ToLowerInvariant())
            {
                var existing = await _unitOfWork.Customers.FindAsync(c => c.Email == model.Email.Trim().ToLowerInvariant() && c.UserId != customer.UserId);
                if (existing.Any())
                {
                    ModelState.AddModelError("Email", "This email is already used by another account.");
                    return View(model);
                }
            }

            customer.FullName = model.FullName.Trim();
            customer.Email = model.Email.Trim().ToLowerInvariant();
            customer.PhoneNumber = model.PhoneNumber.Trim();
            customer.Age = model.Age;
            customer.Address = model.Address.Trim();

            _unitOfWork.Customers.Update(customer);
            await _unitOfWork.SaveChangesAsync();

            model.SuccessMessage = "Profile updated successfully!";
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> IndexAr()
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(GetUserId());
            if (customer == null) return RedirectToAction("LoginAr", "Account");

            return View(new ProfileViewModel
            {
                UserId = customer.UserId, FullName = customer.FullName, Email = customer.Email,
                PhoneNumber = customer.PhoneNumber, Age = customer.Age, Address = customer.Address
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IndexAr(ProfileViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var customer = await _unitOfWork.Customers.GetByIdAsync(GetUserId());
            if (customer == null) return RedirectToAction("LoginAr", "Account");

            if (customer.Email != model.Email.Trim().ToLowerInvariant())
            {
                var existing = await _unitOfWork.Customers.FindAsync(c => c.Email == model.Email.Trim().ToLowerInvariant() && c.UserId != customer.UserId);
                if (existing.Any())
                {
                    ModelState.AddModelError("Email", "هذا البريد الإلكتروني مستخدم من حساب آخر.");
                    return View(model);
                }
            }

            customer.FullName = model.FullName.Trim();
            customer.Email = model.Email.Trim().ToLowerInvariant();
            customer.PhoneNumber = model.PhoneNumber.Trim();
            customer.Age = model.Age;
            customer.Address = model.Address.Trim();

            _unitOfWork.Customers.Update(customer);
            await _unitOfWork.SaveChangesAsync();

            model.SuccessMessage = "تم تحديث الملف الشخصي بنجاح!";
            return View(model);
        }
    }
}
