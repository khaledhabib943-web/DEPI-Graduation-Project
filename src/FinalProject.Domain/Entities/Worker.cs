using FinalProject.Domain.Enums;

namespace FinalProject.Domain.Entities
{
    public class Worker : User
    {
        public int CategoryId { get; set; }
        public string ProfilePicture { get; set; } = string.Empty;
        public string IdFrontImage { get; set; } = string.Empty;
        public string IdBackImage { get; set; } = string.Empty;
        public string? Portfolio { get; set; }
        public decimal ServicePrice { get; set; }
        public AvailabilityStatus AvailabilityStatus { get; set; } = AvailabilityStatus.Offline;
        public float AverageRating { get; set; }
        public bool IsValidated { get; set; }

        // Navigation Properties
        public virtual Category Category { get; set; } = null!;
        public virtual ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
        public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    }
}
