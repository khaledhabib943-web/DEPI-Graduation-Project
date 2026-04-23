using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Salahly.Presentation.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // Action to view and search workers
        public async Task<IActionResult> Index(WorkerSearchDto searchDto)
        {
            var pagedWorkers = await _customerService.GetFilteredWorkersAsync(searchDto);
            
            // Pass the search DTO to the view to maintain state
            ViewData["SearchParameters"] = searchDto;

            return View(pagedWorkers);
        }

        public IActionResult Details(int id)
        {
            // Placeholder for viewing a specific worker
            return View();
        }
    }
}