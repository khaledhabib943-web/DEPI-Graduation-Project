using Application.DTOs;
using Application.Wrappers;
using Domain_layer.Enums;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IServiceRequestService
    {
        Task<ServiceRequestDto> CreateRequestAsync(CreateServiceRequestDto dto);
        Task<PagedResult<ServiceRequestDto>> GetCustomerRequestsAsync(int customerId, int pageNumber = 1, int pageSize = 10);
        Task<bool> UpdateRequestStatusAsync(int requestId, RequestStatus status);
    }
}
