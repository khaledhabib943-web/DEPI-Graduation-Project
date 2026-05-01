using FinalProject.Application.DTOs;
using FinalProject.Application.Interfaces;
using FinalProject.Domain.Enums;

namespace FinalProject.Application.Services
{
    public class WorkerService : IWorkerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public WorkerService(IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task<WorkerDto?> GetWorkerByIdAsync(int workerId)
        {
            var worker = await _unitOfWork.Workers.GetWorkerWithReviewsAsync(workerId);
            if (worker == null) return null;

            return MapToDto(worker);
        }

        public async Task<bool> UpdateProfileAsync(int workerId, UpdateWorkerProfileDto dto)
        {
            var worker = await _unitOfWork.Workers.GetByIdAsync(workerId);
            if (worker == null) return false;

            if (!string.IsNullOrWhiteSpace(dto.FullName))
                worker.FullName = dto.FullName;

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
                worker.PhoneNumber = dto.PhoneNumber;

            if (!string.IsNullOrWhiteSpace(dto.ProfilePicture))
                worker.ProfilePicture = dto.ProfilePicture;

            if (dto.Portfolio != null)
                worker.Portfolio = dto.Portfolio;

            if (dto.ServicePrice.HasValue)
                worker.ServicePrice = dto.ServicePrice.Value;

            _unitOfWork.Workers.Update(worker);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetAvailabilityAsync(int workerId, AvailabilityStatus status)
        {
            var worker = await _unitOfWork.Workers.GetByIdAsync(workerId);
            if (worker == null) return false;

            worker.AvailabilityStatus = status;
            _unitOfWork.Workers.Update(worker);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetPriceAsync(int workerId, decimal price)
        {
            var worker = await _unitOfWork.Workers.GetByIdAsync(workerId);
            if (worker == null) return false;

            worker.ServicePrice = price;
            _unitOfWork.Workers.Update(worker);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AcceptRequestAsync(int workerId, int requestId)
        {
            var request = await _unitOfWork.ServiceRequests.GetByIdAsync(requestId);
            if (request == null || request.WorkerId != workerId)
                return false;

            if (request.Status != RequestStatus.Pending)
                return false;

            request.Status = RequestStatus.InProgress;
            request.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.ServiceRequests.Update(request);
            await _unitOfWork.SaveChangesAsync();

            // Notify the customer
            await _notificationService.SendNotificationAsync(
                request.CustomerId,
                "Request Accepted",
                "Your service request has been accepted by the worker.",
                NotificationType.RequestAccepted,
                requestId);

            return true;
        }

        public async Task<bool> RejectRequestAsync(int workerId, int requestId)
        {
            var request = await _unitOfWork.ServiceRequests.GetByIdAsync(requestId);
            if (request == null || request.WorkerId != workerId)
                return false;

            if (request.Status != RequestStatus.Pending)
                return false;

            request.Status = RequestStatus.Cancelled;
            request.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.ServiceRequests.Update(request);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateRequestStatusAsync(int workerId, int requestId, RequestStatus status)
        {
            var request = await _unitOfWork.ServiceRequests.GetByIdAsync(requestId);
            if (request == null || request.WorkerId != workerId)
                return false;

            request.Status = status;
            request.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.ServiceRequests.Update(request);
            await _unitOfWork.SaveChangesAsync();

            // Notify customer if service is completed
            if (status == RequestStatus.Completed)
            {
                await _notificationService.SendNotificationAsync(
                    request.CustomerId,
                    "Service Completed",
                    "Your service request has been completed.",
                    NotificationType.ServiceCompleted,
                    requestId);
            }

            return true;
        }

        public async Task<IEnumerable<ServiceRequestDto>> GetServiceRequestsAsync(int workerId)
        {
            var requests = await _unitOfWork.ServiceRequests.GetRequestsByWorkerAsync(workerId);

            return requests.Select(sr => new ServiceRequestDto
            {
                RequestId = sr.RequestId,
                CustomerId = sr.CustomerId,
                CustomerName = sr.Customer?.FullName ?? string.Empty,
                WorkerId = sr.WorkerId,
                WorkerName = sr.Worker?.FullName ?? string.Empty,
                CategoryId = sr.CategoryId,
                CategoryName = sr.Category?.Name ?? string.Empty,
                LocationDetails = sr.LocationDetails,
                ScheduledDate = sr.ScheduledDate,
                ScheduledTime = sr.ScheduledTime,
                Status = sr.Status,
                Description = sr.Description,
                CreatedAt = sr.CreatedAt,
                UpdatedAt = sr.UpdatedAt
            });
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsAsync(int workerId)
        {
            var reviews = await _unitOfWork.Reviews.GetReviewsByWorkerAsync(workerId);

            return reviews.Select(r => new ReviewDto
            {
                ReviewId = r.ReviewId,
                CustomerId = r.CustomerId,
                CustomerName = r.Customer?.FullName ?? string.Empty,
                WorkerId = r.WorkerId,
                WorkerName = string.Empty,
                RequestId = r.RequestId,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            });
        }

        // ---- Private Mapping ----

        private static WorkerDto MapToDto(Domain.Entities.Worker worker)
        {
            return new WorkerDto
            {
                UserId = worker.UserId,
                FullName = worker.FullName,
                Email = worker.Email ?? string.Empty,
                PhoneNumber = worker.PhoneNumber ?? string.Empty,
                NationalId = worker.NationalId,
                Age = worker.Age,
                Username = worker.Username,
                Role = worker.Role,
                IsActive = worker.IsActive,
                CreatedAt = worker.CreatedAt,
                CategoryId = worker.CategoryId,
                CategoryName = worker.Category?.Name ?? string.Empty,
                ProfilePicture = worker.ProfilePicture,
                Portfolio = worker.Portfolio,
                ServicePrice = worker.ServicePrice,
                AvailabilityStatus = worker.AvailabilityStatus,
                AverageRating = worker.AverageRating,
                IsValidated = worker.IsValidated
            };
        }
    }
}
