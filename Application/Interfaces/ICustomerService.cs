using Application.DTOs;
using Application.Wrappers;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICustomerService
    {
        Task<PagedResult<WorkerDto>> GetFilteredWorkersAsync(WorkerSearchDto searchDto);
    }
}
