using FinalProject.Application.Services;
using FinalProject.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Web.Controllers
{
    public class TestController : Controller
    {
        private readonly IAuthCookieRefreshService _refreshService;
        private readonly UserManager<User> _userManager;

        public TestController(IAuthCookieRefreshService refreshService, UserManager<User> userManager)
        {
            _refreshService = refreshService;
            _userManager = userManager;
        }

        public async Task<IActionResult> TestCookieRefresh()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return Content("User not authenticated");
            }

            var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Content("User ID not found in claims");
            }

            var result = await _refreshService.RefreshAsync(userId);
            
            return Content($"Cookie refresh result: {result}");
        }
    }
}
