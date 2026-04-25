using FinalProject.Application.Interfaces;
using FinalProject.Domain.Entities;
using FinalProject.Domain.Enums;
using FinalProject.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Infrastructure.Repositories
{
    public class ComplaintRepository : GenericRepository<Complaint>, IComplaintRepository
    {
        public ComplaintRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Complaint>> GetComplaintsByCustomerAsync(int customerId)
        {
            return await _context.Complaints
                .Include(c => c.Worker)
                .Where(c => c.CustomerId == customerId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Complaint>> GetComplaintsByStatusAsync(ComplaintStatus status)
        {
            return await _context.Complaints
                .Include(c => c.Customer)
                .Include(c => c.Worker)
                .Where(c => c.Status == status)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Complaint>> GetPendingComplaintsAsync()
        {
            return await GetComplaintsByStatusAsync(ComplaintStatus.Pending);
        }
    }
}
