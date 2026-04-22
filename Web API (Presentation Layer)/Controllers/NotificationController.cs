using Microsoft.AspNetCore.Mvc;

    public class NotificationController : Controller
    {
        private static List<NotificationViewModel> notifications = new List<NotificationViewModel>
        {
            new NotificationViewModel
            {
                Id = 1,
                Title = "طلب جديد",
                Message = "لديك طلب سباكة جديد في منطقة المعادي",
                Type = "NewRequest",
                IsRead = false,
                CreatedAt = DateTime.Now.ToString("g"),
                UserName = "محمد السباك",
                RelatedRequestId = 101
            },
            new NotificationViewModel
            {
                Id = 2,
                Title = "تم قبول الطلب",
                Message = "وافق الفني إبراهيم على طلب صيانة الكهرباء",
                Type = "RequestAccepted",
                IsRead = true,
                CreatedAt = DateTime.Now.AddMinutes(-30).ToString("g"),
                UserName = "سارة محمود",
                RelatedRequestId = 102
            }
        };

        public IActionResult Index()
        {
            return View(notifications);
        }

        public IActionResult Details(int id)
        {
            var notification = notifications.FirstOrDefault(n => n.Id == id);
            if (notification == null) return NotFound();

            notification.IsRead = true;

            return View(notification);
        }
    }
