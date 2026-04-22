using Microsoft.AspNetCore.Mvc;

    public class ComplaintController : Controller
    {
       
        private static List<ComplaintViewModel> complaints = new List<ComplaintViewModel>
        {
            new ComplaintViewModel
            {
                Id = 1,
                Title = "تأخير في الموعد",
                Content = "الفني تأخر عن موعده المتفق عليه بساعتين",
                Status = "Pending",
                CreatedAt = DateTime.Now.ToShortDateString(),
                CustomerName = "سارة محمود",
                ServiceRequestId = 101
            },
            new ComplaintViewModel
            {
                Id = 2,
                Title = "سوء معاملة",
                Content = "طريقة كلام الفني كانت غير لائقة",
                Status = "Resolved",
                CreatedAt = DateTime.Now.AddDays(-1).ToShortDateString(),
                CustomerName = "أحمد علي",
                ServiceRequestId = null // شكوى عامة
            }
        };

        public IActionResult Index()
        {
            return View(complaints);
        }

        public IActionResult Details(int id)
        {
            var complaint = complaints.FirstOrDefault(c => c.Id == id);
            if (complaint == null) return NotFound();
            return View(complaint);
        }
    }
