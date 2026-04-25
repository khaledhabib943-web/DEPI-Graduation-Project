namespace FinalProject.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICustomerRepository Customers { get; }
        IWorkerRepository Workers { get; }
        IAdminRepository Admins { get; }
        ICategoryRepository Categories { get; }
        IServiceRequestRepository ServiceRequests { get; }
        IReviewRepository Reviews { get; }
        INotificationRepository Notifications { get; }
        IComplaintRepository Complaints { get; }
        IFavoriteRepository Favorites { get; }

        Task<int> SaveChangesAsync();
    }
}
