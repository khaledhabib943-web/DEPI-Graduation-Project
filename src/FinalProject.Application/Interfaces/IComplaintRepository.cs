using FinalProject.Domain.Entities;
using FinalProject.Domain.Enums;

namespace FinalProject.Application.Interfaces
{
    public interface IComplaintRepository : IRepository<Complaint>
    {
        Task<IEnumerable<Complaint>> GetComplaintsByCustomerAsync(int customerId);
        Task<IEnumerable<Complaint>> GetComplaintsByStatusAsync(ComplaintStatus status);
        Task<IEnumerable<Complaint>> GetPendingComplaintsAsync();
    }
}
