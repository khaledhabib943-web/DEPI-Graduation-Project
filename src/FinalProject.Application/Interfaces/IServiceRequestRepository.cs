using FinalProject.Domain.Entities;
using FinalProject.Domain.Enums;

namespace FinalProject.Application.Interfaces
{
    public interface IServiceRequestRepository : IRepository<ServiceRequest>
    {
        Task<IEnumerable<ServiceRequest>> GetRequestsByCustomerAsync(int customerId);
        Task<IEnumerable<ServiceRequest>> GetRequestsByWorkerAsync(int workerId);
        Task<IEnumerable<ServiceRequest>> GetRequestsByStatusAsync(RequestStatus status);
    }
}
