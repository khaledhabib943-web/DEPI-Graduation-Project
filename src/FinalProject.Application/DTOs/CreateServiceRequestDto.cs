namespace FinalProject.Application.DTOs
{
    public class CreateServiceRequestDto
    {
        public int WorkerId { get; set; }
        public int CategoryId { get; set; }
        public string LocationDetails { get; set; } = string.Empty;
        public DateTime ScheduledDate { get; set; }
        public TimeSpan ScheduledTime { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
