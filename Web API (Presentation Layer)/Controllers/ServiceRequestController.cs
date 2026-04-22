using Microsoft.AspNetCore.Mvc;

    public class ServiceRequestController : Controller
    {
        private List<ServiceRequestViewModel> requests = new List<ServiceRequestViewModel>
        {
            new ServiceRequestViewModel
            {
                Id = 101,
                Description = "تصليح حنفية المطبخ",
                Address = "القاهرة - مدينة نصر",
                Status = "Pending",
                ScheduledDate = "2026-05-20",
                CustomerName = "أحمد علي",
                WorkerName = "لم يتم التحديد بعد"
            },
            new ServiceRequestViewModel
            {
                Id = 102,
                Description = "صيانة لوحة الكهرباء",
                Address = "الجيزة - الدقي",
                Status = "Accepted",
                ScheduledDate = "2026-05-22",
                CustomerName = "سارة محمد",
                WorkerName = "إبراهيم الكهربائي"
            }
        };

        public IActionResult Index()
        {
            return View(requests);
        }

        public IActionResult Details(int id)
        {
            var request = requests.FirstOrDefault(r => r.Id == id);
            if (request == null) return NotFound();
            return View(request);
        }
    }
