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
        private readonly IUnitOfWork _unitOfWork;

        public ReviewController(ICustomerService customerService, IUnitOfWork unitOfWork)
        {
            _customerService = customerService;
            _unitOfWork = unitOfWork;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<IActionResult> Create(int requestId, int workerId, string workerName)
        {
            // Check if already reviewed
            var alreadyReviewed = await _unitOfWork.Reviews.HasReviewForRequestAsync(GetUserId(), requestId);
            if (alreadyReviewed)
            {
                TempData["InfoMessage"] = "You have already reviewed this service.";
                return RedirectToAction("Index", "Dashboard");
            }
            return View(new ReviewViewModel { RequestId = requestId, WorkerId = workerId, WorkerName = workerName });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReviewViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var dto = new CreateReviewDto { WorkerId = model.WorkerId, RequestId = model.RequestId, Rating = model.Rating, Comment = model.Comment };
            await _customerService.RateWorkerAsync(GetUserId(), dto);
            TempData["SuccessMessage"] = "Thank you! Your review has been submitted.";
            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public async Task<IActionResult> CreateAr(int requestId, int workerId, string workerName)
        {
            // Check if already reviewed
            var alreadyReviewed = await _unitOfWork.Reviews.HasReviewForRequestAsync(GetUserId(), requestId);
            if (alreadyReviewed)
            {
                TempData["InfoMessageAr"] = "لقد قمت بتقييم هذه الخدمة من قبل.";
                return RedirectToAction("IndexAr", "Dashboard");
            }
            return View(new ReviewViewModel { RequestId = requestId, WorkerId = workerId, WorkerName = workerName });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAr(ReviewViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var dto = new CreateReviewDto { WorkerId = model.WorkerId, RequestId = model.RequestId, Rating = model.Rating, Comment = model.Comment };
            await _customerService.RateWorkerAsync(GetUserId(), dto);
            TempData["SuccessMessageAr"] = "شكراً! تم إرسال تقييمك بنجاح.";
            return RedirectToAction("IndexAr", "Dashboard");
        }
    }
}
