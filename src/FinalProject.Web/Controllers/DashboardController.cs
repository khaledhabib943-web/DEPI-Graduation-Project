using FinalProject.Application.Interfaces;
using FinalProject.Domain.Enums;
using FinalProject.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinalProject.Web.Controllers
{
    [Authorize(Roles = "Customer")]
    public class DashboardController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly INotificationService _notificationService;

        public DashboardController(ICustomerService customerService, INotificationService notificationService)
        {
            _customerService = customerService;
            _notificationService = notificationService;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            var requests = await _customerService.GetServiceRequestsAsync(userId);
            var unread = await _notificationService.GetUnreadCountAsync(userId);
            var favorites = await _customerService.GetFavoritesAsync(userId);

            var model = new DashboardViewModel
            {
                CustomerName = User.FindFirst("FullName")?.Value ?? "",
                ActiveRequestsCount = requests.Count(r => r.Status == RequestStatus.Pending || r.Status == RequestStatus.InProgress),
                CompletedRequestsCount = requests.Count(r => r.Status == RequestStatus.Completed),
                FavoritesCount = favorites.Count(),
                UnreadNotifications = unread,
                RecentRequests = requests.Take(5).ToList()
            };

            return View(model);
        }

        public async Task<IActionResult> IndexAr()
        {
            var userId = GetUserId();
            var requests = await _customerService.GetServiceRequestsAsync(userId);
            var unread = await _notificationService.GetUnreadCountAsync(userId);
            var favorites = await _customerService.GetFavoritesAsync(userId);

            var model = new DashboardViewModel
            {
                CustomerName = User.FindFirst("FullName")?.Value ?? "",
                ActiveRequestsCount = requests.Count(r => r.Status == RequestStatus.Pending || r.Status == RequestStatus.InProgress),
                CompletedRequestsCount = requests.Count(r => r.Status == RequestStatus.Completed),
                FavoritesCount = favorites.Count(),
                UnreadNotifications = unread,
                RecentRequests = requests.Take(5).ToList()
            };

            return View(model);
        }
    }
}
