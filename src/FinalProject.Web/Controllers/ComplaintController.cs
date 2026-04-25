using FinalProject.Application.DTOs;
using FinalProject.Application.Interfaces;
using FinalProject.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinalProject.Web.Controllers
{
    [Authorize(Roles = "Customer")]
    public class ComplaintController : Controller
    {
        private readonly ICustomerService _customerService;

        public ComplaintController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        public async Task<IActionResult> Index()
        {
            var complaints = await _customerService.GetComplaintsAsync(GetUserId());
            return View(new ComplaintListViewModel { Complaints = complaints.ToList() });
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var requests = await _customerService.GetServiceRequestsAsync(GetUserId());
            return View(new CreateComplaintViewModel { CompletedRequests = requests.ToList() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateComplaintViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var requests = await _customerService.GetServiceRequestsAsync(GetUserId());
                model.CompletedRequests = requests.ToList();
                return View(model);
            }

            var dto = new CreateComplaintDto { WorkerId = model.WorkerId, Description = model.Description };
            await _customerService.SubmitComplaintAsync(GetUserId(), dto);
            return RedirectToAction("Index");
        }

        // Arabic
        public async Task<IActionResult> IndexAr()
        {
            var complaints = await _customerService.GetComplaintsAsync(GetUserId());
            return View(new ComplaintListViewModel { Complaints = complaints.ToList() });
        }

        [HttpGet]
        public async Task<IActionResult> CreateAr()
        {
            var requests = await _customerService.GetServiceRequestsAsync(GetUserId());
            return View(new CreateComplaintViewModel { CompletedRequests = requests.ToList() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAr(CreateComplaintViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var requests = await _customerService.GetServiceRequestsAsync(GetUserId());
                model.CompletedRequests = requests.ToList();
                return View(model);
            }

            var dto = new CreateComplaintDto { WorkerId = model.WorkerId, Description = model.Description };
            await _customerService.SubmitComplaintAsync(GetUserId(), dto);
            return RedirectToAction("IndexAr");
        }
    }
}
