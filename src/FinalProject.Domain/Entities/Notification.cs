using FinalProject.Domain.Enums;

namespace FinalProject.Domain.Entities
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int? RelatedRequestId { get; set; }

        // Navigation Properties
        public virtual User User { get; set; } = null!;
        public virtual ServiceRequest? RelatedRequest { get; set; }
    }
}
