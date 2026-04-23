using System;

namespace Application.DTOs
{
    public class CreateServiceRequestDto
    {
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime ScheduledDate { get; set; }
        public int CustomerId { get; set; }
        public int WorkerId { get; set; }
    }
}
