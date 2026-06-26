using FinalProject.Application.Interfaces;
using FinalProject.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FinalProject.Domain.Enums;
using FinalProject.Domain.Entities;

namespace FinalProject.Web.Controllers
{
    [Authorize(Roles = "Customer,Worker")]
    public class ProfileController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProfileController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        private async Task PopulateCategoriesAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            ViewBag.Categories = categories.Where(c => c.IsActive).ToList();
        }

        // ── English GET ──────────────────────────────────────────────────────────────

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            var customer = await _unitOfWork.Customers.GetByIdAsync(userId);
            if (customer != null)
            {
                return View(BuildCustomerModel(customer));
            }

            var worker = await _unitOfWork.Workers.GetWorkerWithReviewsAsync(userId);
            if (worker != null)
            {
                await PopulateCategoriesAsync();
                return View(BuildWorkerModel(worker));
            }

            return RedirectToAction("Login", "Account");
        }

        // ── English POST ─────────────────────────────────────────────────────────────

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ProfileViewModel model)
        {
            var userId   = GetUserId();
            var isWorker = User.IsInRole("Worker");

            // Remove file validation errors — files are optional updates
            ModelState.Remove(nameof(model.ProfilePictureFile));
            ModelState.Remove(nameof(model.PortfolioFile));
            // Role is set server-side; ignore any client-posted value
            ModelState.Remove(nameof(model.Role));

            if (!ModelState.IsValid)
            {
                if (isWorker) await PopulateCategoriesAsync();
                model.Role = isWorker ? UserRole.Worker : UserRole.Customer;
                return View(model);
            }

            if (isWorker)
            {
                var worker = await _unitOfWork.Workers.GetByIdAsync(userId);
                if (worker == null) return RedirectToAction("Login", "Account");

                if (model.CategoryId is null or <= 0)
                {
                    ModelState.AddModelError("CategoryId", "Please select a category.");
                    await PopulateCategoriesAsync();
                    model.Role = UserRole.Worker;
                    return View(model);
                }
                if (model.ServicePrice is null or <= 0)
                {
                    ModelState.AddModelError("ServicePrice", "Please enter a valid service price.");
                    await PopulateCategoriesAsync();
                    model.Role = UserRole.Worker;
                    return View(model);
                }

                worker.FullName     = model.FullName.Trim();
                worker.Email        = model.Email?.Trim().ToLowerInvariant() ?? worker.Email;
                worker.PhoneNumber  = model.PhoneNumber.Trim();
                worker.Age          = model.Age;
                worker.Address      = model.Address?.Trim() ?? string.Empty;
                worker.CategoryId   = model.CategoryId.Value;
                worker.ServicePrice = model.ServicePrice.Value;

                if (model.ProfilePictureFile?.Length > 0)
                    worker.ProfilePicture = await UploadFileAsync(model.ProfilePictureFile, "profile_pics");

                if (model.PortfolioFile?.Length > 0)
                    worker.Portfolio = await UploadFileAsync(model.PortfolioFile, "portfolios");

                _unitOfWork.Workers.Update(worker);
                await _unitOfWork.SaveChangesAsync();

                // Re-issue auth cookie so header picks up new FullName + ProfilePicture immediately
                await AccountController.IssueAuthCookieAsync(
                    HttpContext,
                    userId:      worker.UserId.ToString(),
                    userName:    worker.UserName ?? worker.Email ?? "Unknown",
                    email:       worker.Email ?? string.Empty,
                    fullName:    worker.FullName,
                    role:        "Worker",
                    profilePic:  worker.ProfilePicture ?? string.Empty);

                await PopulateCategoriesAsync();
                model.Portfolio       = worker.Portfolio;
                model.ProfilePicture  = worker.ProfilePicture;
                model.Role            = UserRole.Worker;
                model.SuccessMessage  = "Profile updated successfully!";
                return View(model);
            }
            else
            {
                var customer = await _unitOfWork.Customers.GetByIdAsync(userId);
                if (customer == null) return RedirectToAction("Login", "Account");

                customer.FullName    = model.FullName.Trim();
                customer.Email       = model.Email?.Trim().ToLowerInvariant() ?? customer.Email;
                customer.PhoneNumber = model.PhoneNumber.Trim();
                customer.Age         = model.Age;
                customer.Address     = model.Address?.Trim() ?? string.Empty;

                if (model.ProfilePictureFile?.Length > 0)
                    customer.ProfilePicture = await UploadFileAsync(model.ProfilePictureFile, "profile_pics");

                _unitOfWork.Customers.Update(customer);
                await _unitOfWork.SaveChangesAsync();

                // Re-issue auth cookie so header picks up new FullName + ProfilePicture immediately
                await AccountController.IssueAuthCookieAsync(
                    HttpContext,
                    userId:      customer.UserId.ToString(),
                    userName:    customer.UserName ?? customer.Email ?? "Unknown",
                    email:       customer.Email ?? string.Empty,
                    fullName:    customer.FullName,
                    role:        "Customer",
                    profilePic:  customer.ProfilePicture ?? string.Empty);

                model.ProfilePicture = customer.ProfilePicture;
                model.Role           = UserRole.Customer;
                model.SuccessMessage = "Profile updated successfully!";
                return View(model);
            }
        }

        // ── Arabic GET ───────────────────────────────────────────────────────────────

        [HttpGet]
        public async Task<IActionResult> IndexAr()
        {
            var userId = GetUserId();
            var customer = await _unitOfWork.Customers.GetByIdAsync(userId);
            if (customer != null)
            {
                return View(BuildCustomerModel(customer));
            }

            var worker = await _unitOfWork.Workers.GetWorkerWithReviewsAsync(userId);
            if (worker != null)
            {
                await PopulateCategoriesAsync();
                return View(BuildWorkerModel(worker));
            }

            return RedirectToAction("LoginAr", "Account");
        }

        // ── Arabic POST ──────────────────────────────────────────────────────────────

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IndexAr(ProfileViewModel model)
        {
            var userId   = GetUserId();
            var isWorker = User.IsInRole("Worker");

            ModelState.Remove(nameof(model.ProfilePictureFile));
            ModelState.Remove(nameof(model.PortfolioFile));
            ModelState.Remove(nameof(model.Role));

            if (!ModelState.IsValid)
            {
                if (isWorker) await PopulateCategoriesAsync();
                model.Role = isWorker ? UserRole.Worker : UserRole.Customer;
                return View(model);
            }

            if (isWorker)
            {
                var worker = await _unitOfWork.Workers.GetByIdAsync(userId);
                if (worker == null) return RedirectToAction("LoginAr", "Account");

                if (model.CategoryId is null or <= 0)
                {
                    ModelState.AddModelError("CategoryId", "يرجى اختيار القسم/الفئة.");
                    await PopulateCategoriesAsync();
                    model.Role = UserRole.Worker;
                    return View(model);
                }
                if (model.ServicePrice is null or <= 0)
                {
                    ModelState.AddModelError("ServicePrice", "يرجى إدخال سعر الخدمة.");
                    await PopulateCategoriesAsync();
                    model.Role = UserRole.Worker;
                    return View(model);
                }

                worker.FullName     = model.FullName.Trim();
                worker.Email        = model.Email?.Trim().ToLowerInvariant() ?? worker.Email;
                worker.PhoneNumber  = model.PhoneNumber.Trim();
                worker.Age          = model.Age;
                worker.Address      = model.Address?.Trim() ?? string.Empty;
                worker.CategoryId   = model.CategoryId.Value;
                worker.ServicePrice = model.ServicePrice.Value;

                if (model.ProfilePictureFile?.Length > 0)
                    worker.ProfilePicture = await UploadFileAsync(model.ProfilePictureFile, "profile_pics");

                if (model.PortfolioFile?.Length > 0)
                    worker.Portfolio = await UploadFileAsync(model.PortfolioFile, "portfolios");

                _unitOfWork.Workers.Update(worker);
                await _unitOfWork.SaveChangesAsync();

                // Re-issue auth cookie (Arabic profile save)
                await AccountController.IssueAuthCookieAsync(
                    HttpContext,
                    userId:      worker.UserId.ToString(),
                    userName:    worker.UserName ?? worker.Email ?? "Unknown",
                    email:       worker.Email ?? string.Empty,
                    fullName:    worker.FullName,
                    role:        "Worker",
                    profilePic:  worker.ProfilePicture ?? string.Empty);

                await PopulateCategoriesAsync();
                model.Portfolio       = worker.Portfolio;
                model.ProfilePicture  = worker.ProfilePicture;
                model.Role            = UserRole.Worker;
                model.SuccessMessage  = "تم تحديث الملف الشخصي بنجاح!";
                return View(model);
            }
            else
            {
                var customer = await _unitOfWork.Customers.GetByIdAsync(userId);
                if (customer == null) return RedirectToAction("LoginAr", "Account");

                customer.FullName    = model.FullName.Trim();
                customer.Email       = model.Email?.Trim().ToLowerInvariant() ?? customer.Email;
                customer.PhoneNumber = model.PhoneNumber.Trim();
                customer.Age         = model.Age;
                customer.Address     = model.Address?.Trim() ?? string.Empty;

                if (model.ProfilePictureFile?.Length > 0)
                    customer.ProfilePicture = await UploadFileAsync(model.ProfilePictureFile, "profile_pics");

                _unitOfWork.Customers.Update(customer);
                await _unitOfWork.SaveChangesAsync();

                // Re-issue auth cookie (Arabic profile save)
                await AccountController.IssueAuthCookieAsync(
                    HttpContext,
                    userId:      customer.UserId.ToString(),
                    userName:    customer.UserName ?? customer.Email ?? "Unknown",
                    email:       customer.Email ?? string.Empty,
                    fullName:    customer.FullName,
                    role:        "Customer",
                    profilePic:  customer.ProfilePicture ?? string.Empty);

                model.ProfilePicture = customer.ProfilePicture;
                model.Role           = UserRole.Customer;
                model.SuccessMessage = "تم تحديث الملف الشخصي بنجاح!";
                return View(model);
            }
        }

        // ── Helpers ──────────────────────────────────────────────────────────────────

        private static ProfileViewModel BuildCustomerModel(Customer c) => new()
        {
            UserId = c.UserId, Role = UserRole.Customer, FullName = c.FullName,
            Email = c.Email ?? string.Empty, PhoneNumber = c.PhoneNumber ?? string.Empty,
            Age = c.Age, Address = c.Address, ProfilePicture = c.ProfilePicture
        };

        private static ProfileViewModel BuildWorkerModel(Worker w) => new()
        {
            UserId = w.UserId, Role = UserRole.Worker, FullName = w.FullName,
            Email = w.Email ?? string.Empty, PhoneNumber = w.PhoneNumber ?? string.Empty,
            Age = w.Age, Address = w.Address, ProfilePicture = w.ProfilePicture,
            CategoryId = w.CategoryId, CategoryName = w.Category?.Name ?? string.Empty,
            ServicePrice = w.ServicePrice, Portfolio = w.Portfolio
        };

        private async Task<string> UploadFileAsync(IFormFile file, string folder)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", folder);
            Directory.CreateDirectory(uploadsFolder);
            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath       = Path.Combine(uploadsFolder, uniqueFileName);
            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
            return $"/uploads/{folder}/{uniqueFileName}";
        }
    }
}
