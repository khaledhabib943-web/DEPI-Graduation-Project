using FinalProject.Domain.Entities;

namespace FinalProject.Application.Interfaces
{
    public interface INotificationRepository : IRepository<Notification>
    {
        Task<IEnumerable<Notification>> GetNotificationsByUserAsync(int userId);
        Task<IEnumerable<Notification>> GetUnreadNotificationsByUserAsync(int userId);
        Task<int> GetUnreadCountAsync(int userId);
    }
}
