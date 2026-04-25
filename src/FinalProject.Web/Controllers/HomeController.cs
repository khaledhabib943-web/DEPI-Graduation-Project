using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult IndexAr()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
