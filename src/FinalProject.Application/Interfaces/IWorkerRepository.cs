using FinalProject.Domain.Entities;
using FinalProject.Domain.Enums;

namespace FinalProject.Application.Interfaces
{
    public interface IWorkerRepository : IRepository<Worker>
    {
        Task<IEnumerable<Worker>> GetWorkersByCategoryAsync(int categoryId);
        Task<IEnumerable<Worker>> GetAvailableWorkersAsync();
        Task<Worker?> GetWorkerWithReviewsAsync(int workerId);
    }
}
