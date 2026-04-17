using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectRowadd.Models
{
    /// <summary>
    /// Stores a customer's rating and written feedback for a worker
    /// after a completed service.
    /// </summary>
    public class Review
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReviewId { get; set; }

        public int CustomerId { get; set; }

        public int WorkerId { get; set; }

        public int RequestId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // —— Navigation Properties ————————————————————

        /// <summary>
        /// The customer who wrote this review.
        /// </summary>
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        /// <summary>
        /// The worker being reviewed.
        /// </summary>
        [ForeignKey("WorkerId")]
        public virtual Worker Worker { get; set; }

        /// <summary>
        /// The service request this review is linked to.
        /// </summary>
        [ForeignKey("RequestId")]
        public virtual ServiceRequest ServiceRequest { get; set; }
    }
}