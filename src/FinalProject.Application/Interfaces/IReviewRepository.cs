using FinalProject.Domain.Entities;

namespace FinalProject.Application.Interfaces
{
    public interface IReviewRepository : IRepository<Review>
    {
        Task<IEnumerable<Review>> GetReviewsByWorkerAsync(int workerId);
        Task<IEnumerable<Review>> GetReviewsByCustomerAsync(int customerId);
        Task<double> GetAverageRatingForWorkerAsync(int workerId);
    }
}
