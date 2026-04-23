using Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IComplaintService
    {
        Task<ComplaintDto> CreateComplaintAsync(CreateComplaintDto dto);
        Task<IEnumerable<ComplaintDto>> GetCustomerComplaintsAsync(int customerId);
    }
}
