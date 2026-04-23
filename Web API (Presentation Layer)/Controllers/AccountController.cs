using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Salahly.Presentation.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);
            if (result == null)
            {
                ModelState.AddModelError("", "Invalid credentials.");
                return View(loginDto);
            }

            // Simulate login success by setting tempdata
            TempData["UserToken"] = result.Token;
            TempData["UserName"] = result.FullName;
            TempData["UserId"] = result.Id;
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto);
            
            TempData["UserToken"] = result.Token;
            TempData["UserName"] = result.FullName;
            TempData["UserId"] = result.Id;
            return RedirectToAction("Index", "Home");
        }
    }
}
