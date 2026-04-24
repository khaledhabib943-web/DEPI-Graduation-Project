using System;

namespace Application.DTOs
{
    public class ComplaintDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int? ServiceRequestId { get; set; }
        public DateTime CreatedAt { get; set; }
        public Domain_layer.Enums.ComplaintStatus Status { get; set; }
    }
}
