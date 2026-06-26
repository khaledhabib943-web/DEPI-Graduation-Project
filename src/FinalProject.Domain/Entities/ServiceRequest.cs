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

        // Arrival confirmation workflow
        public bool IsWorkerArrived { get; set; } = false;
        public DateTime? WorkerArrivedAt { get; set; }
        public bool IsArrivalConfirmedByCustomer { get; set; } = false;
        public DateTime? ArrivalConfirmedAt { get; set; }

        // Work completion confirmation — customer confirms work is done before worker can mark Complete
        public bool IsWorkCompletedConfirmedByCustomer { get; set; } = false;
        public DateTime? WorkCompletionConfirmedAt { get; set; }

        // Price snapshot at time of booking (for admin reporting)
        public decimal PriceAtBooking { get; set; } = 0;

        // Navigation Properties
        public virtual Customer Customer { get; set; } = null!;
        public virtual Worker Worker { get; set; } = null!;
        public virtual Category Category { get; set; } = null!;
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public virtual Review? Review { get; set; }
        public virtual ICollection<ServiceRequestStatusHistory> StatusHistory { get; set; } = new List<ServiceRequestStatusHistory>();
    }
}
