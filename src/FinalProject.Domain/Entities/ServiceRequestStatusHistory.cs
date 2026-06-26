using FinalProject.Domain.Enums;

namespace FinalProject.Domain.Entities
{
    public class ServiceRequestStatusHistory
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public RequestStatus Status { get; set; }
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
        public string? Note { get; set; }

        // Navigation
        public virtual ServiceRequest Request { get; set; } = null!;
    }
}
