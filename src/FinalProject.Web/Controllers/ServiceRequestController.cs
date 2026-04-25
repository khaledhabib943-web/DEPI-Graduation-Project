using FinalProject.Application.DTOs;
using FinalProject.Application.Interfaces;
using FinalProject.Domain.Enums;
using FinalProject.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinalProject.Web.Controllers
{
    [Authorize(Roles = "Customer")]
    public class ServiceRequestController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly ICategoryService _categoryService;
        private readonly IUnitOfWork _unitOfWork;

        public ServiceRequestController(ICustomerService customerService, ICategoryService categoryService, IUnitOfWork unitOfWork)
        {
            _customerService = customerService;
            _categoryService = categoryService;
            _unitOfWork = unitOfWork;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // ===== Step 1: Select Service =====
        public async Task<IActionResult> SelectService()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return View(new SelectServiceViewModel { Categories = categories.ToList() });
        }

        public async Task<IActionResult> SelectServiceAr()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return View(new SelectServiceViewModel { Categories = categories.ToList() });
        }

        // ===== Step 2: Select Professional with sorting/filtering/pagination =====
        public async Task<IActionResult> SelectProfessional(int categoryId, string? sortBy, decimal? minPrice, decimal? maxPrice, float? minRating, int page = 1)
        {
            var category = await _categoryService.GetCategoryByIdAsync(categoryId);
            if (category == null) return RedirectToAction("SelectService");

            var filters = new SearchFilterDto { CategoryId = categoryId, MinPrice = minPrice, MaxPrice = maxPrice, MinRating = minRating };
            var allWorkers = (await _customerService.SearchWorkersAsync(filters)).ToList();

            // Sorting
            allWorkers = sortBy switch
            {
                "rating" => allWorkers.OrderByDescending(w => w.AverageRating).ToList(),
                "price_asc" => allWorkers.OrderBy(w => w.ServicePrice).ToList(),
                "price_desc" => allWorkers.OrderByDescending(w => w.ServicePrice).ToList(),
                "name" => allWorkers.OrderBy(w => w.FullName).ToList(),
                _ => Shuffle(allWorkers) // default: random order
            };

            // Pagination
            int pageSize = 6;
            int totalCount = allWorkers.Count;
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            var pagedWorkers = allWorkers.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var model = new SelectProfessionalViewModel
            {
                CategoryId = categoryId,
                CategoryName = category.Name,
                Workers = pagedWorkers,
                SortBy = sortBy,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                MinRating = minRating,
                CurrentPage = page,
                TotalPages = totalPages,
                TotalCount = totalCount,
                PageSize = pageSize
            };

            return View(model);
        }

        public async Task<IActionResult> SelectProfessionalAr(int categoryId, string? sortBy, decimal? minPrice, decimal? maxPrice, float? minRating, int page = 1)
        {
            var category = await _categoryService.GetCategoryByIdAsync(categoryId);
            if (category == null) return RedirectToAction("SelectServiceAr");

            var filters = new SearchFilterDto { CategoryId = categoryId, MinPrice = minPrice, MaxPrice = maxPrice, MinRating = minRating };
            var allWorkers = (await _customerService.SearchWorkersAsync(filters)).ToList();

            allWorkers = sortBy switch
            {
                "rating" => allWorkers.OrderByDescending(w => w.AverageRating).ToList(),
                "price_asc" => allWorkers.OrderBy(w => w.ServicePrice).ToList(),
                "price_desc" => allWorkers.OrderByDescending(w => w.ServicePrice).ToList(),
                "name" => allWorkers.OrderBy(w => w.FullName).ToList(),
                _ => Shuffle(allWorkers)
            };

            int pageSize = 6;
            var model = new SelectProfessionalViewModel
            {
                CategoryId = categoryId, CategoryName = category.Name,
                Workers = allWorkers.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                SortBy = sortBy, MinPrice = minPrice, MaxPrice = maxPrice, MinRating = minRating,
                CurrentPage = page, TotalPages = (int)Math.Ceiling((double)allWorkers.Count / pageSize),
                TotalCount = allWorkers.Count, PageSize = pageSize
            };

            return View(model);
        }

        // ===== Step 3: Service Details =====
        [HttpGet]
        public async Task<IActionResult> ServiceDetails(int workerId, int categoryId)
        {
            var category = await _categoryService.GetCategoryByIdAsync(categoryId);
            var workers = await _categoryService.GetWorkersByCategoryAsync(categoryId);
            var worker = workers.FirstOrDefault(w => w.UserId == workerId);

            if (worker == null || category == null) return RedirectToAction("SelectService");

            return View(new ServiceDetailsViewModel
            {
                WorkerId = workerId, WorkerName = worker.FullName,
                CategoryId = categoryId, CategoryName = category.Name,
                ServicePrice = worker.ServicePrice
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ServiceDetails(ServiceDetailsViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = GetUserId();
            var dto = new CreateServiceRequestDto
            {
                WorkerId = model.WorkerId, CategoryId = model.CategoryId,
                LocationDetails = model.LocationDetails, ScheduledDate = model.ScheduledDate,
                ScheduledTime = model.ScheduledTime, Description = model.Description
            };

            var result = await _customerService.CreateServiceRequestAsync(userId, dto);

            // AUTO-ACCEPT: Simulate worker accepting the request immediately
            await AutoAcceptRequest(result.RequestId);

            return RedirectToAction("Confirmation", new { requestId = result.RequestId });
        }

        [HttpGet]
        public async Task<IActionResult> ServiceDetailsAr(int workerId, int categoryId)
        {
            var category = await _categoryService.GetCategoryByIdAsync(categoryId);
            var workers = await _categoryService.GetWorkersByCategoryAsync(categoryId);
            var worker = workers.FirstOrDefault(w => w.UserId == workerId);
            if (worker == null || category == null) return RedirectToAction("SelectServiceAr");

            return View(new ServiceDetailsViewModel
            {
                WorkerId = workerId, WorkerName = worker.FullName,
                CategoryId = categoryId, CategoryName = category.Name,
                ServicePrice = worker.ServicePrice
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ServiceDetailsAr(ServiceDetailsViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = GetUserId();
            var dto = new CreateServiceRequestDto
            {
                WorkerId = model.WorkerId, CategoryId = model.CategoryId,
                LocationDetails = model.LocationDetails, ScheduledDate = model.ScheduledDate,
                ScheduledTime = model.ScheduledTime, Description = model.Description
            };
            var result = await _customerService.CreateServiceRequestAsync(userId, dto);

            // AUTO-ACCEPT
            await AutoAcceptRequest(result.RequestId);

            return RedirectToAction("ConfirmationAr", new { requestId = result.RequestId });
        }

        // ===== Step 4: Confirmation =====
        public async Task<IActionResult> Confirmation(int requestId)
        {
            var requests = await _customerService.GetServiceRequestsAsync(GetUserId());
            var request = requests.FirstOrDefault(r => r.RequestId == requestId);
            if (request == null) return RedirectToAction("SelectService");
            return View(new ConfirmationViewModel { Request = request });
        }

        public async Task<IActionResult> ConfirmationAr(int requestId)
        {
            var requests = await _customerService.GetServiceRequestsAsync(GetUserId());
            var request = requests.FirstOrDefault(r => r.RequestId == requestId);
            if (request == null) return RedirectToAction("SelectServiceAr");
            return View(new ConfirmationViewModel { Request = request });
        }

        // ===== Step 5: Track Request =====
        public async Task<IActionResult> TrackRequest(int requestId)
        {
            var requests = await _customerService.GetServiceRequestsAsync(GetUserId());
            var request = requests.FirstOrDefault(r => r.RequestId == requestId);
            if (request == null) return RedirectToAction("Index", "Dashboard");
            return View(new TrackRequestViewModel { Request = request });
        }

        public async Task<IActionResult> TrackRequestAr(int requestId)
        {
            var requests = await _customerService.GetServiceRequestsAsync(GetUserId());
            var request = requests.FirstOrDefault(r => r.RequestId == requestId);
            if (request == null) return RedirectToAction("IndexAr", "Dashboard");
            return View(new TrackRequestViewModel { Request = request });
        }

        // ===== Simulate Completion (for testing) =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SimulateComplete(int requestId)
        {
            var sr = await _unitOfWork.ServiceRequests.GetByIdAsync(requestId);
            if (sr != null && sr.CustomerId == GetUserId())
            {
                sr.Status = RequestStatus.Completed;
                sr.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.ServiceRequests.Update(sr);
                await _unitOfWork.SaveChangesAsync();
            }
            return RedirectToAction("TrackRequest", new { requestId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SimulateCompleteAr(int requestId)
        {
            var sr = await _unitOfWork.ServiceRequests.GetByIdAsync(requestId);
            if (sr != null && sr.CustomerId == GetUserId())
            {
                sr.Status = RequestStatus.Completed;
                sr.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.ServiceRequests.Update(sr);
                await _unitOfWork.SaveChangesAsync();
            }
            return RedirectToAction("TrackRequestAr", new { requestId });
        }

        // ===== Cancel =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int requestId)
        {
            await _customerService.CancelRequestAsync(GetUserId(), requestId);
            return RedirectToAction("Index", "Dashboard");
        }

        // ===== Helpers =====
        private async Task AutoAcceptRequest(int requestId)
        {
            var sr = await _unitOfWork.ServiceRequests.GetByIdAsync(requestId);
            if (sr != null)
            {
                sr.Status = RequestStatus.InProgress;
                sr.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.ServiceRequests.Update(sr);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        private static List<T> Shuffle<T>(List<T> list)
        {
            var rng = new Random();
            int n = list.Count;
            var shuffled = new List<T>(list);
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (shuffled[k], shuffled[n]) = (shuffled[n], shuffled[k]);
            }
            return shuffled;
        }
    }
}
