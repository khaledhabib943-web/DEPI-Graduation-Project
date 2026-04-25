using FinalProject.Domain.Enums;

namespace FinalProject.Application.DTOs
{
    public class ComplaintDto
    {
        public int ComplaintId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int WorkerId { get; set; }
        public string WorkerName { get; set; } = string.Empty;
        public int? RequestId { get; set; }
        public string Description { get; set; } = string.Empty;
        public ComplaintStatus Status { get; set; }
        public string AdminResponse { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }
}
