using Microsoft.AspNetCore.Mvc;

  public class WorkerController : Controller
    {
        private List<WorkerViewModel> workersList = new List<WorkerViewModel>
        {
            new WorkerViewModel
            {
                Id = 1,
                FullName = "محمد السباك",
                Bio = "فني سباكة متخصص في التركيبات",
                HourlyRate = 150.0,
                CategoryName = "سباكة",
                IsVerified = true,
                Availability = "Available"
            },
            new WorkerViewModel
            {
                Id = 2,
                FullName = "أحمد الكهربائي",
                Bio = "مهندس كهرباء للصيانة المنزلية",
                HourlyRate = 120.0,
                CategoryName = "كهرباء",
                IsVerified = false,
                Availability = "PendingApproval"
            }
        };

        
        public IActionResult Index()
        {
            return View(workersList);
        }
        public IActionResult Details(int id)
        {
            var worker = workersList.FirstOrDefault(w => w.Id == id);

            if (worker == null)
            {
                return NotFound();
            }

            return View(worker);
        }
    }
