using FinalProject.Domain.Enums;

namespace FinalProject.Application.DTOs
{
    public class WorkerDto : UserDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        // ProfilePicture is inherited from UserDto
        public string IdFrontImage { get; set; } = string.Empty;
        public string IdBackImage { get; set; } = string.Empty;
        public string? Portfolio { get; set; }
        public decimal ServicePrice { get; set; }
        public AvailabilityStatus AvailabilityStatus { get; set; }
        public float AverageRating { get; set; }
        public bool IsValidated { get; set; }
    }
}
