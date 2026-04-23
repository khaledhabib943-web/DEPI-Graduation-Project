using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Salahly.Presentation.Controllers
{
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet]
        public async Task<IActionResult> WorkerReviews(int workerId, int pageNumber = 1)
        {
            var pagedReviews = await _reviewService.GetWorkerReviewsAsync(workerId, pageNumber);
            ViewBag.WorkerId = workerId;
            return View(pagedReviews);
        }

        [HttpGet]
        public IActionResult Create(int workerId, int customerId = 101, int? serviceRequestId = null)
        {
            var dto = new CreateReviewDto
            {
                WorkerId = workerId,
                CustomerId = customerId,
                ServiceRequestId = serviceRequestId ?? 0
            };
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateReviewDto dto)
        {
            await _reviewService.CreateReviewAsync(dto);
            return RedirectToAction(nameof(WorkerReviews), new { workerId = dto.WorkerId });
        }
    }
}
