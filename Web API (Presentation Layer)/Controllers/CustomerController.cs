using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Salahly.Presentation.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IHomeService _homeService;

        public CustomerController(ICustomerService customerService, IHomeService homeService)
        {
            _customerService = customerService;
            _homeService = homeService;
        }

        public async Task<IActionResult> Dashboard()
        {
            int customerId = TempData.Peek("UserId") as int? ?? 101; // Mock logged in user
            var data = await _homeService.GetLandingPageDataAsync(customerId);
            return View(data);
        }

        public async Task<IActionResult> DashboardEn()
        {
            int customerId = TempData.Peek("UserId") as int? ?? 101;
            var data = await _homeService.GetLandingPageDataAsync(customerId);
            return View(data);
        }

        // Action to view and search workers
        public async Task<IActionResult> Index(WorkerSearchDto searchDto)
        {
            var pagedWorkers = await _customerService.GetFilteredWorkersAsync(searchDto);
            
            // Pass the search DTO to the view to maintain state
            ViewData["SearchParameters"] = searchDto;

            return View(pagedWorkers);
        }

        public async Task<IActionResult> Details(int id)
        {
            var worker = await _customerService.GetWorkerByIdAsync(id);
            if (worker == null)
                return NotFound();

            return View(worker);
        }
    }
}