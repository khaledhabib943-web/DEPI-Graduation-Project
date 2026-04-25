using FinalProject.Application.DTOs;
using FinalProject.Domain.Enums;

namespace FinalProject.Application.Interfaces
{
    public interface IWorkerService
    {
        Task<WorkerDto?> GetWorkerByIdAsync(int workerId);
        Task<bool> UpdateProfileAsync(int workerId, UpdateWorkerProfileDto dto);
        Task<bool> SetAvailabilityAsync(int workerId, AvailabilityStatus status);
        Task<bool> SetPriceAsync(int workerId, decimal price);
        Task<bool> AcceptRequestAsync(int workerId, int requestId);
        Task<bool> RejectRequestAsync(int workerId, int requestId);
        Task<bool> UpdateRequestStatusAsync(int workerId, int requestId, RequestStatus status);
        Task<IEnumerable<ServiceRequestDto>> GetServiceRequestsAsync(int workerId);
        Task<IEnumerable<ReviewDto>> GetReviewsAsync(int workerId);
    }
}
