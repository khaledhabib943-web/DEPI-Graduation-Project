using FinalProject.Domain.Entities;

namespace FinalProject.Application.Interfaces
{
    public interface IFavoriteRepository : IRepository<Favorite>
    {
        Task<IEnumerable<Favorite>> GetFavoritesByCustomerAsync(int customerId);
        Task<bool> IsFavoriteAsync(int customerId, int workerId);
        Task<Favorite?> GetFavoriteAsync(int customerId, int workerId);
    }
}
