using Microsoft.AspNetCore.Mvc;

    public class CategoryController : Controller
    {
        private List<CategoryViewModel> categories = new List<CategoryViewModel>
        {
            new CategoryViewModel
            {
                Id = 1,
                Name = "سباكة",
                Description = "كل ما يخص أعمال السباكة والصرف الصحي",
                IconUrl = "plumbing_icon.png"
            },
            new CategoryViewModel
            {
                Id = 2,
                Name = "كهرباء",
                Description = "تصليح وتركيب الدوائر الكهربائية والإضاءة",
                IconUrl = "electric_icon.png"
            }
        };

        public IActionResult Index()
        {
            return View(categories);
        }

        public IActionResult Details(int id)
        {
            var category = categories.FirstOrDefault(c => c.Id == id);

            if (category == null)
                return NotFound();

            return View(category);
        }
    }
