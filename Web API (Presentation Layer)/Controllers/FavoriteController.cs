using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Salahly.Presentation.Controllers
{
    public class FavoriteController : Controller
    {
        private readonly IFavoriteService _favoriteService;

        public FavoriteController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int customerId = 101)
        {
            var favorites = await _favoriteService.GetCustomerFavoritesAsync(customerId);
            return View(favorites);
        }

        [HttpPost]
        public async Task<IActionResult> Add(int customerId, int workerId)
        {
            await _favoriteService.AddFavoriteAsync(customerId, workerId);
            return RedirectToAction(nameof(Index), new { customerId });
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int customerId, int workerId)
        {
            await _favoriteService.RemoveFavoriteAsync(customerId, workerId);
            return RedirectToAction(nameof(Index), new { customerId });
        }
    }
}
