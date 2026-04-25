using FinalProject.Application.Interfaces;
using FinalProject.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinalProject.Web.Controllers
{
    [Authorize(Roles = "Customer")]
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        public async Task<IActionResult> Index()
        {
            var notifications = await _notificationService.GetUserNotificationsAsync(GetUserId());
            var unread = await _notificationService.GetUnreadCountAsync(GetUserId());
            return View(new NotificationListViewModel { Notifications = notifications.ToList(), UnreadCount = unread });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _notificationService.MarkAsReadAsync(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> IndexAr()
        {
            var notifications = await _notificationService.GetUserNotificationsAsync(GetUserId());
            var unread = await _notificationService.GetUnreadCountAsync(GetUserId());
            return View(new NotificationListViewModel { Notifications = notifications.ToList(), UnreadCount = unread });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsReadAr(int id)
        {
            await _notificationService.MarkAsReadAsync(id);
            return RedirectToAction("IndexAr");
        }
    }
}
