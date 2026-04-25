using FinalProject.Application.Interfaces;
using FinalProject.Domain.Entities;
using FinalProject.Domain.Enums;
using FinalProject.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Infrastructure.Repositories
{
    public class WorkerRepository : GenericRepository<Worker>, IWorkerRepository
    {
        public WorkerRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Worker>> GetWorkersByCategoryAsync(int categoryId)
        {
            return await _context.Workers
                .Include(w => w.Category)
                .Where(w => w.CategoryId == categoryId && w.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Worker>> GetAvailableWorkersAsync()
        {
            return await _context.Workers
                .Include(w => w.Category)
                .Where(w => w.AvailabilityStatus == AvailabilityStatus.Available
                         && w.IsActive
                         && w.IsValidated)
                .ToListAsync();
        }

        public async Task<Worker?> GetWorkerWithReviewsAsync(int workerId)
        {
            return await _context.Workers
                .Include(w => w.Reviews)
                    .ThenInclude(r => r.Customer)
                .Include(w => w.Category)
                .FirstOrDefaultAsync(w => w.UserId == workerId);
        }
    }
}
