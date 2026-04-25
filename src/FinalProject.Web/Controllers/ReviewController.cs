using FinalProject.Application.DTOs;
using FinalProject.Application.Interfaces;
using FinalProject.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinalProject.Web.Controllers
{
    [Authorize(Roles = "Customer")]
    public class ReviewController : Controller
    {
        private readonly ICustomerService _customerService;

        public ReviewController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public IActionResult Create(int requestId, int workerId, string workerName)
        {
            return View(new ReviewViewModel { RequestId = requestId, WorkerId = workerId, WorkerName = workerName });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReviewViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var dto = new CreateReviewDto { WorkerId = model.WorkerId, RequestId = model.RequestId, Rating = model.Rating, Comment = model.Comment };
            await _customerService.RateWorkerAsync(GetUserId(), dto);
            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public IActionResult CreateAr(int requestId, int workerId, string workerName)
        {
            return View(new ReviewViewModel { RequestId = requestId, WorkerId = workerId, WorkerName = workerName });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAr(ReviewViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var dto = new CreateReviewDto { WorkerId = model.WorkerId, RequestId = model.RequestId, Rating = model.Rating, Comment = model.Comment };
            await _customerService.RateWorkerAsync(GetUserId(), dto);
            return RedirectToAction("IndexAr", "Dashboard");
        }
    }
}
