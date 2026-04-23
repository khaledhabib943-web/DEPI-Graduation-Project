using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Salahly.Presentation.Controllers
{
    public class ComplaintController : Controller
    {
        private readonly IComplaintService _complaintService;

        public ComplaintController(IComplaintService complaintService)
        {
            _complaintService = complaintService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int customerId = 101)
        {
            var complaints = await _complaintService.GetCustomerComplaintsAsync(customerId);
            return View(complaints);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateComplaintDto dto)
        {
            await _complaintService.CreateComplaintAsync(dto);
            return RedirectToAction(nameof(Index), new { customerId = dto.CustomerId });
        }
    }
}
