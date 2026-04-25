using FinalProject.Application.Interfaces;
using FinalProject.Infrastructure.DbContext;

namespace FinalProject.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        private ICustomerRepository? _customers;
        private IWorkerRepository? _workers;
        private IAdminRepository? _admins;
        private ICategoryRepository? _categories;
        private IServiceRequestRepository? _serviceRequests;
        private IReviewRepository? _reviews;
        private INotificationRepository? _notifications;
        private IComplaintRepository? _complaints;
        private IFavoriteRepository? _favorites;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public ICustomerRepository Customers =>
            _customers ??= new CustomerRepository(_context);

        public IWorkerRepository Workers =>
            _workers ??= new WorkerRepository(_context);

        public IAdminRepository Admins =>
            _admins ??= new AdminRepository(_context);

        public ICategoryRepository Categories =>
            _categories ??= new CategoryRepository(_context);

        public IServiceRequestRepository ServiceRequests =>
            _serviceRequests ??= new ServiceRequestRepository(_context);

        public IReviewRepository Reviews =>
            _reviews ??= new ReviewRepository(_context);

        public INotificationRepository Notifications =>
            _notifications ??= new NotificationRepository(_context);

        public IComplaintRepository Complaints =>
            _complaints ??= new ComplaintRepository(_context);

        public IFavoriteRepository Favorites =>
            _favorites ??= new FavoriteRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
