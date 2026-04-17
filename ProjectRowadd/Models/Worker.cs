using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectRowadd.Models
{
    /// <summary>
    /// Represents a worker who provides home services.
    /// Inherits shared identity fields from User.
    /// </summary>
    public class Worker : User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int WorkerId { get; set; }

        [Required(ErrorMessage = "Personal image is required.")]
        [StringLength(300, ErrorMessage = "Image URL cannot exceed 300 characters.")]
        [Display(Name = "Personal Image")]
        public string PersonalImageUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "Front ID image is required.")]
        [StringLength(300, ErrorMessage = "Image URL cannot exceed 300 characters.")]
        [Display(Name = "Front ID Image")]
        public string FrontIdImageUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "Back ID image is required.")]
        [StringLength(300, ErrorMessage = "Image URL cannot exceed 300 characters.")]
        [Display(Name = "Back ID Image")]
        public string BackIdImageUrl { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Portfolio URL cannot exceed 500 characters.")]
        [Display(Name = "Portfolio")]
        public string? PortfolioUrl { get; set; }

        [Required(ErrorMessage = "Service price is required.")]
        [Range(0.01, 100000, ErrorMessage = "Price must be a positive value.")]
        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Service Price")]
        public decimal ServicePrice { get; set; }

        [Display(Name = "Is Available")]
        public bool IsAvailable { get; set; } = true;

        // ——— Foreign Keys ———————————————————————————

        [Display(Name = "Category")]
        public int? CategoryId { get; set; }

        // ——— Navigation Properties ———————————————————

        /// <summary>
        /// The service category this worker belongs to. (Worker → Category)
        /// </summary>
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        /// <summary>
        /// Service requests assigned to this worker.
        /// </summary>
        public virtual ICollection<ServiceRequest> ServiceRequests { get; set; }
            = new List<ServiceRequest>();

        /// <summary>
        /// Reviews received by this worker from customers.
        /// </summary>
        public virtual ICollection<Review> Reviews { get; set; }
            = new List<Review>();

        /// <summary>
        /// Customers who added this worker to their favorites.
        /// </summary>
        public virtual ICollection<Favorite> Favorites { get; set; }
            = new List<Favorite>();

        /// <summary>
        /// Notifications sent to this worker.
        /// </summary>
        public virtual ICollection<Notification> Notifications { get; set; }
            = new List<Notification>();
    }
}