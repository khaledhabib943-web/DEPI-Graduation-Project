using Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IFavoriteService
    {
        Task<FavoriteDto> AddFavoriteAsync(int customerId, int workerId);
        Task<bool> RemoveFavoriteAsync(int customerId, int workerId);
        Task<IEnumerable<FavoriteDto>> GetCustomerFavoritesAsync(int customerId);
    }
}
