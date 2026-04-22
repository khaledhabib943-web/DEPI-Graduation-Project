using Microsoft.AspNetCore.Mvc;


  public class AdminController : Controller
    {
      private List<AdminViewModel> admins = new List<AdminViewModel>
        {
            new AdminViewModel
            {
                Id = 1,
                FullName = "saraa",
                Email = "sara@salahly.com",
                PhoneNumber = "0123456789",
                Role = "Admin",
                IsActive = true
            },
            new AdminViewModel
            {
                Id = 2,
                FullName = "Kholouddd",
                Email = "Kholoud@salahly.com",
                PhoneNumber = "0100000000",
                Role = "Admin",
                IsActive = true
            }
        };
        public IActionResult Index()
        {
            return View(admins);
        }

        public IActionResult Details(int id)
        {
            var admin = admins.FirstOrDefault(a => a.Id == id);

            if (admin == null)
                return NotFound();

            return View(admin);
        }
    }
