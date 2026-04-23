using Application.DTOs;
using Application.Interfaces;
using Domain_layer.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Salahly.Presentation.Controllers
{
    public class ServiceRequestController : Controller
    {
        private readonly IServiceRequestService _requestService;
        private readonly INotificationService _notificationService;

        public ServiceRequestController(IServiceRequestService requestService, INotificationService notificationService)
        {
            _requestService = requestService;
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int customerId = 101, int pageNumber = 1, string status = "") // Using 101 as mock logged-in customer
        {
            var pagedRequests = await _requestService.GetCustomerRequestsAsync(customerId, pageNumber);
            
            // Filter by status if provided
            if (!string.IsNullOrEmpty(status))
            {
                if (Enum.TryParse<RequestStatus>(status, out var statusEnum))
                {
                    var filteredItems = pagedRequests.Items.Where(r => r.Status == statusEnum).ToList();
                    pagedRequests = new Application.Wrappers.PagedResult<Application.DTOs.ServiceRequestDto>
                    {
                        Items = filteredItems,
                        TotalCount = filteredItems.Count,
                        PageNumber = 1,
                        PageSize = filteredItems.Count
                    };
                }
            }
            
            ViewData["CurrentStatus"] = status;
            return View(pagedRequests);
        }

        [HttpGet]
        public IActionResult Create(int workerId, int customerId = 101)
        {
            var dto = new CreateServiceRequestDto
            {
                WorkerId = workerId,
                CustomerId = customerId
            };
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateServiceRequestDto dto)
        {
            var result = await _requestService.CreateRequestAsync(dto);
            // Notify worker
            if(dto.WorkerId > 0)
                await _notificationService.CreateNotificationAsync(dto.WorkerId, $"You have a new service request from Customer #{dto.CustomerId}");

            return RedirectToAction(nameof(Index), new { customerId = dto.CustomerId });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int requestId, RequestStatus status)
        {
            await _requestService.UpdateRequestStatusAsync(requestId, status);
            return RedirectToAction(nameof(Index)); // Needs customerId in a real app
        }
    }
}
