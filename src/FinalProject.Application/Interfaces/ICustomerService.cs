using FinalProject.Application.DTOs;
using FinalProject.Domain.Enums;

namespace FinalProject.Application.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerDto?> GetCustomerByIdAsync(int customerId);
        Task<IEnumerable<WorkerDto>> SearchWorkersAsync(SearchFilterDto filters);
        Task<ServiceRequestDto> CreateServiceRequestAsync(int customerId, CreateServiceRequestDto dto);
        Task<RequestStatus?> TrackRequestStatusAsync(int customerId, int requestId);
        Task<ReviewDto> RateWorkerAsync(int customerId, CreateReviewDto dto);
        Task<ComplaintDto> SubmitComplaintAsync(int customerId, CreateComplaintDto dto);
        Task<bool> AddToFavoritesAsync(int customerId, int workerId);
        Task<bool> RemoveFromFavoritesAsync(int customerId, int workerId);
        Task<IEnumerable<FavoriteDto>> GetFavoritesAsync(int customerId);
        Task<bool> CancelRequestAsync(int customerId, int requestId);
        Task<IEnumerable<ServiceRequestDto>> GetServiceRequestsAsync(int customerId);
        Task<IEnumerable<ComplaintDto>> GetComplaintsAsync(int customerId);
    }
}
