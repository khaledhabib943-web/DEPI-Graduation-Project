using FinalProject.Application.Interfaces;
using FinalProject.Domain.Entities;
using FinalProject.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Infrastructure.Repositories
{
    public class FavoriteRepository : GenericRepository<Favorite>, IFavoriteRepository
    {
        public FavoriteRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Favorite>> GetFavoritesByCustomerAsync(int customerId)
        {
            return await _context.Favorites
                .Include(f => f.Worker)
                    .ThenInclude(w => w.Category)
                .Where(f => f.CustomerId == customerId)
                .OrderByDescending(f => f.AddedAt)
                .ToListAsync();
        }

        public async Task<bool> IsFavoriteAsync(int customerId, int workerId)
        {
            return await _context.Favorites
                .AnyAsync(f => f.CustomerId == customerId && f.WorkerId == workerId);
        }

        public async Task<Favorite?> GetFavoriteAsync(int customerId, int workerId)
        {
            return await _context.Favorites
                .FirstOrDefaultAsync(f => f.CustomerId == customerId && f.WorkerId == workerId);
        }
    }
}
