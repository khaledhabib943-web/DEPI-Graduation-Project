using FinalProject.Domain.Enums;

namespace FinalProject.Domain.Entities
{
    public class ServiceRequest
    {
        public int RequestId { get; set; }
        public int CustomerId { get; set; }
        public int WorkerId { get; set; }
        public int CategoryId { get; set; }
        public string LocationDetails { get; set; } = string.Empty;
        public DateTime ScheduledDate { get; set; }
        public TimeSpan ScheduledTime { get; set; }
        public RequestStatus Status { get; set; } = RequestStatus.Pending;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual Customer Customer { get; set; } = null!;
        public virtual Worker Worker { get; set; } = null!;
        public virtual Category Category { get; set; } = null!;
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public virtual Review? Review { get; set; }
    }
}
