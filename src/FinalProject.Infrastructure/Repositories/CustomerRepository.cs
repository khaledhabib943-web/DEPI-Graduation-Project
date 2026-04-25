using FinalProject.Application.Interfaces;
using FinalProject.Domain.Entities;
using FinalProject.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Infrastructure.Repositories
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Customer?> GetCustomerWithFavoritesAsync(int customerId)
        {
            return await _context.Customers
                .Include(c => c.Favorites)
                    .ThenInclude(f => f.Worker)
                .FirstOrDefaultAsync(c => c.UserId == customerId);
        }

        public async Task<Customer?> GetCustomerWithServiceRequestsAsync(int customerId)
        {
            return await _context.Customers
                .Include(c => c.ServiceRequests)
                    .ThenInclude(sr => sr.Worker)
                .Include(c => c.ServiceRequests)
                    .ThenInclude(sr => sr.Category)
                .FirstOrDefaultAsync(c => c.UserId == customerId);
        }
    }
}
