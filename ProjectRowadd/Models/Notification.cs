namespace ProjectRowadd.Models
{
    public enum NotificationType
    {
        NewRequest,
        RequestAccepted,
        ServiceCompleted
    }
    public class Notification
    {
        public int NotificationId { get; private set; }
        public int UserId { get; private set; }
        public string Title { get; private set; }
        public string Message { get; private set; }
        public NotificationType Type { get; private set; }
        public bool IsRead { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public int? RelatedRequestId { get; private set; }

        // For ORM
        private Notification() { }

        // send → implemented as constructor
        public Notification(
            int userId,
            string title,
            string message,
            NotificationType type,
            int? relatedRequestId = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty.");

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message cannot be empty.");

            UserId = userId;
            Title = title;
            Message = message;
            Type = type;
            RelatedRequestId = relatedRequestId;
            CreatedAt = DateTime.UtcNow;
            IsRead = false;
        }

        // markAsRead
        public void MarkAsRead()
        {
            if (!IsRead)
            {
                IsRead = true;
            }
        }
    }
}
