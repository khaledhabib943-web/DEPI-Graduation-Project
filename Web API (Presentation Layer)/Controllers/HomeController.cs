using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Salahly.Presentation.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeService _homeService;

        public HomeController(IHomeService homeService)
        {
            _homeService = homeService;
        }

        public async Task<IActionResult> Index()
        {
            int customerId = TempData.Peek("UserId") as int? ?? 101; // Mock logged in user
            var data = await _homeService.GetLandingPageDataAsync(customerId);
            return View(data);
        }

        public async Task<IActionResult> IndexEn()
        {
            int customerId = TempData.Peek("UserId") as int? ?? 101;
            var data = await _homeService.GetLandingPageDataAsync(customerId);
            return View(data);
        }
    }
}
