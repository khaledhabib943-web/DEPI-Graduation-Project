using FinalProject.Application.DTOs;

namespace FinalProject.Application.Interfaces
{
    public interface INotificationService
    {
        Task<NotificationDto> SendNotificationAsync(int userId, string title, string message, Domain.Enums.NotificationType type, int? relatedRequestId = null);
        Task<bool> MarkAsReadAsync(int notificationId);
        Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(int userId);
        Task<int> GetUnreadCountAsync(int userId);
    }
}
