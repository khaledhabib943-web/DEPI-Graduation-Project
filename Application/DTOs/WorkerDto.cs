using Domain_layer.Enums;

namespace Application.DTOs
{
    public class WorkerDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public double HourlyRate { get; set; }
        public bool IsVerified { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public AvailabilityStatus Availability { get; set; }
        public double AverageRating { get; set; }
    }
}
