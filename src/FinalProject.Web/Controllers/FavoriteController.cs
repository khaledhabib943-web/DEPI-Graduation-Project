using FinalProject.Application.Interfaces;
using FinalProject.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinalProject.Web.Controllers
{
    [Authorize(Roles = "Customer")]
    public class FavoriteController : Controller
    {
        private readonly ICustomerService _customerService;

        public FavoriteController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        public async Task<IActionResult> Index()
        {
            var favorites = await _customerService.GetFavoritesAsync(GetUserId());
            return View(new FavoriteListViewModel { Favorites = favorites.ToList() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int workerId)
        {
            await _customerService.AddToFavoritesAsync(GetUserId(), workerId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int workerId)
        {
            await _customerService.RemoveFromFavoritesAsync(GetUserId(), workerId);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> IndexAr()
        {
            var favorites = await _customerService.GetFavoritesAsync(GetUserId());
            return View(new FavoriteListViewModel { Favorites = favorites.ToList() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAr(int workerId)
        {
            await _customerService.AddToFavoritesAsync(GetUserId(), workerId);
            return RedirectToAction("IndexAr");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveAr(int workerId)
        {
            await _customerService.RemoveFromFavoritesAsync(GetUserId(), workerId);
            return RedirectToAction("IndexAr");
        }
    }
}
