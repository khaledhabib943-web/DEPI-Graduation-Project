using FinalProject.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminDashboardController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public async Task<IActionResult> Index()
        {
            var complaints = await _adminService.GetAllComplaintsAsync();
            ViewBag.TotalComplaints = complaints.Count();
            return View();
        }

        public async Task<IActionResult> IndexAr()
        {
            var complaints = await _adminService.GetAllComplaintsAsync();
            ViewBag.TotalComplaints = complaints.Count();
            return View();
        }
    }
}
