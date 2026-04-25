using FinalProject.Domain.Enums;

namespace FinalProject.Domain.Entities
{
    public class Complaint
    {
        public int ComplaintId { get; set; }
        public int CustomerId { get; set; }
        public int WorkerId { get; set; }
        public int? RequestId { get; set; }
        public string Description { get; set; } = string.Empty;
        public ComplaintStatus Status { get; set; } = ComplaintStatus.Pending;
        public string AdminResponse { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ResolvedAt { get; set; }

        // Navigation Properties
        public virtual Customer Customer { get; set; } = null!;
        public virtual Worker Worker { get; set; } = null!;
        public virtual ServiceRequest? Request { get; set; }
    }
}
