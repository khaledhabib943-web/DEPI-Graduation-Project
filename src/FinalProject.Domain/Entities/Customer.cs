namespace FinalProject.Domain.Entities
{
    public class Customer : User
    {
        public string Address { get; set; } = string.Empty;

        // Navigation Properties
        public virtual ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
        public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    }
}
