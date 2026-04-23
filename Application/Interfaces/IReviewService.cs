using Application.DTOs;
using Application.Wrappers;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IReviewService
    {
        Task<ReviewDto> CreateReviewAsync(CreateReviewDto dto);
        Task<PagedResult<ReviewDto>> GetWorkerReviewsAsync(int workerId, int pageNumber = 1, int pageSize = 10);
    }
}
