using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectRowadd.Models
{
    /// <summary>
    /// Core transactional entity capturing a customer's request for a
    /// specific service. Tracks the full lifecycle from creation through
    /// completion or cancellation.
    /// </summary>
    public class ServiceRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RequestId { get; set; }

        public int CustomerId { get; set; }

        public int WorkerId { get; set; }

        public int CategoryId { get; set; }

        public string LocationDetails { get; set; }

        public DateTime ScheduledDate { get; set; }

        public TimeSpan ScheduledTime { get; set; }

        public RequestStatus Status { get; set; } = RequestStatus.Pending;

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // —— Navigation Properties ————————————————————

        /// <summary>
        /// The customer who created this request.
        /// </summary>
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        /// <summary>
        /// The worker assigned to this request.
        /// </summary>
        [ForeignKey("WorkerId")]
        public virtual Worker Worker { get; set; }

        /// <summary>
        /// The service category for this request.
        /// </summary>
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        /// <summary>
        /// Notifications triggered by events on this request.
        /// </summary>
        public virtual ICollection<Notification> Notifications { get; set; }
            = new List<Notification>();

        /// <summary>
        /// Complaints filed against this request.
        /// </summary>
        public virtual ICollection<Complaint> Complaints { get; set; }
            = new List<Complaint>();

        /// <summary>
        /// Review submitted after this request is completed.
        /// </summary>
        public virtual Review Review { get; set; }
    }
}