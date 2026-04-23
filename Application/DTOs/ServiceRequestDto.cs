using Domain_layer.Enums;
using System;

namespace Application.DTOs
{
    public class ServiceRequestDto
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public RequestStatus Status { get; set; }
        public DateTime ScheduledDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CustomerId { get; set; }
        public int? WorkerId { get; set; }
        public string WorkerName { get; set; } = string.Empty;
    }
}
