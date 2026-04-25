using FinalProject.Application.Interfaces;
using FinalProject.Domain.Entities;
using FinalProject.Domain.Enums;
using FinalProject.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Infrastructure.Repositories
{
    public class ServiceRequestRepository : GenericRepository<ServiceRequest>, IServiceRequestRepository
    {
        public ServiceRequestRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ServiceRequest>> GetRequestsByCustomerAsync(int customerId)
        {
            return await _context.ServiceRequests
                .Include(sr => sr.Worker)
                .Include(sr => sr.Category)
                .Where(sr => sr.CustomerId == customerId)
                .OrderByDescending(sr => sr.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ServiceRequest>> GetRequestsByWorkerAsync(int workerId)
        {
            return await _context.ServiceRequests
                .Include(sr => sr.Customer)
                .Include(sr => sr.Category)
                .Where(sr => sr.WorkerId == workerId)
                .OrderByDescending(sr => sr.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ServiceRequest>> GetRequestsByStatusAsync(RequestStatus status)
        {
            return await _context.ServiceRequests
                .Include(sr => sr.Customer)
                .Include(sr => sr.Worker)
                .Include(sr => sr.Category)
                .Where(sr => sr.Status == status)
                .OrderByDescending(sr => sr.CreatedAt)
                .ToListAsync();
        }
    }
}
