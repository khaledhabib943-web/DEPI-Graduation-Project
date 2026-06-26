using FinalProject.Application.Interfaces;
using FinalProject.Domain.Enums;
using FinalProject.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinalProject.Web.Controllers
{
    [Authorize(Roles = "Worker")]
    public class WorkerDashboardController : Controller
    {
        private readonly IWorkerService _workerService;
        private readonly INotificationService _notificationService;

        public WorkerDashboardController(IWorkerService workerService, INotificationService notificationService)
        {
            _workerService = workerService;
            _notificationService = notificationService;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        private async Task<WorkerDashboardViewModel> BuildDashboardViewModelAsync()
        {
            var workerId = GetUserId();
            var worker = await _workerService.GetWorkerByIdAsync(workerId);
            var requests = await _workerService.GetServiceRequestsAsync(workerId);
            var reviews = await _workerService.GetReviewsAsync(workerId);
            var unreadCount = await _notificationService.GetUnreadCountAsync(workerId);

            var requestsList = requests.ToList();
            var reviewsList = reviews.ToList();

            return new WorkerDashboardViewModel
            {
                WorkerName = worker?.FullName ?? User.FindFirst("FullName")?.Value ?? string.Empty,
                AvailabilityStatus = worker?.AvailabilityStatus ?? AvailabilityStatus.Offline,
                ServicePrice = worker?.ServicePrice ?? 0,
                AverageRating = worker?.AverageRating ?? 0,
                CategoryName = worker?.CategoryName,
                ProfilePicture = worker?.ProfilePicture,
                
                PendingRequestsCount = requestsList.Count(r => r.Status == RequestStatus.Pending),
                ActiveRequestsCount = requestsList.Count(r => r.Status == RequestStatus.InProgress),
                CompletedRequestsCount = requestsList.Count(r => r.Status == RequestStatus.Completed),
                CancelledRequestsCount = requestsList.Count(r => r.Status == RequestStatus.Cancelled),
                UnreadNotificationsCount = unreadCount,
                
                RecentRequests = requestsList.OrderByDescending(r => r.CreatedAt).Take(10).ToList(),
                RecentReviews = reviewsList.OrderByDescending(r => r.CreatedAt).Take(5).ToList()
            };
        }

        public async Task<IActionResult> Index()
        {
            var model = await BuildDashboardViewModelAsync();
            return View(model);
        }

        public async Task<IActionResult> IndexAr()
        {
            var model = await BuildDashboardViewModelAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAvailability(AvailabilityStatus status, bool isArabic = false)
        {
            var workerId = GetUserId();
            await _workerService.SetAvailabilityAsync(workerId, status);
            return RedirectToAction(isArabic ? nameof(IndexAr) : nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePrice(decimal price, bool isArabic = false)
        {
            if (price <= 0)
            {
                TempData["Error"] = isArabic ? "يجب أن يكون السعر أكبر من الصفر." : "Price must be greater than zero.";
                return RedirectToAction(isArabic ? nameof(IndexAr) : nameof(Index));
            }
            var workerId = GetUserId();
            await _workerService.SetPriceAsync(workerId, price);
            TempData["Success"] = isArabic ? "تم تحديث سعر الخدمة بنجاح." : "Service price updated successfully.";
            return RedirectToAction(isArabic ? nameof(IndexAr) : nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptRequest(int requestId, bool isArabic = false)
        {
            var workerId = GetUserId();
            var success = await _workerService.AcceptRequestAsync(workerId, requestId);
            if (success)
            {
                TempData["Success"] = isArabic ? "تم قبول الطلب بنجاح." : "Request accepted successfully.";
            }
            else
            {
                TempData["Error"] = isArabic ? "فشل في قبول الطلب." : "Failed to accept request.";
            }
            return RedirectToAction(isArabic ? nameof(IndexAr) : nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectRequest(int requestId, bool isArabic = false)
        {
            var workerId = GetUserId();
            var success = await _workerService.RejectRequestAsync(workerId, requestId);
            if (success)
            {
                TempData["Success"] = isArabic ? "تم رفض الطلب." : "Request rejected.";
            }
            else
            {
                TempData["Error"] = isArabic ? "فشل في رفض الطلب." : "Failed to reject request.";
            }
            return RedirectToAction(isArabic ? nameof(IndexAr) : nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkArrived(int requestId, bool isArabic = false)
        {
            var workerId = GetUserId();
            var success  = await _workerService.MarkArrivedAsync(workerId, requestId);
            if (success)
                TempData["Success"] = isArabic ? "تم إرسال إشعار وصولك للعميل." : "Arrival notification sent to customer.";
            else
                TempData["Error"] = isArabic ? "تعذّر تسجيل الوصول." : "Could not mark arrival.";
            return RedirectToAction(isArabic ? nameof(IndexAr) : nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteRequest(int requestId, bool isArabic = false)
        {
            var workerId = GetUserId();
            var success  = await _workerService.UpdateRequestStatusAsync(workerId, requestId, RequestStatus.Completed);
            if (success)
                TempData["Success"] = isArabic ? "تم إكمال الطلب بنجاح." : "Request completed successfully.";
            else
                TempData["Error"] = isArabic ? "فشل في إكمال الطلب. يجب أن يؤكد العميل إتمام العمل أولاً." : "Cannot complete. The customer must confirm work is done first.";
            return RedirectToAction(isArabic ? nameof(IndexAr) : nameof(Index));
        }
    }
}
