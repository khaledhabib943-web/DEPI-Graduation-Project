namespace FinalProject.Domain.Entities
{
    public class Review
    {
        public int ReviewId { get; set; }
        public int CustomerId { get; set; }
        public int WorkerId { get; set; }
        public int RequestId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual Customer Customer { get; set; } = null!;
        public virtual Worker Worker { get; set; } = null!;
        public virtual ServiceRequest Request { get; set; } = null!;
    }
}
