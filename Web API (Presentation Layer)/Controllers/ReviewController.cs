using Microsoft.AspNetCore.Mvc;

    public class ReviewController : Controller
    {
        
        private List<ReviewViewModel> reviewsList = new List<ReviewViewModel>
        {
            new ReviewViewModel
            {
                Id = 1,
                Rating = 5,
                Comment = "خدمة ممتازة وفني محترم جداً",
                CreatedAt = DateTime.Now.ToShortDateString(),
                CustomerName = "سارة محمود",
                WorkerName = "محمد السباك",
                ServiceRequestId = 101
            },
            new ReviewViewModel
            {
                Id = 2,
                Rating = 4,
                Comment = "شغل نضيف بس اتأخر شوية عن الموعد",
                CreatedAt = DateTime.Now.ToShortDateString(),
                CustomerName = "أحمد علي",
                WorkerName = "إبراهيم الكهربائي",
                ServiceRequestId = 102
            }
        };

       
        public IActionResult Index()
        {
            return View(reviewsList);
        }

        public IActionResult Details(int id)
        {
            var review = reviewsList.FirstOrDefault(r => r.Id == id);
            if (review == null) return NotFound();
            return View(review);
        }
    }
