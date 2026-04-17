using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectRowadd.Models
{
    /// <summary>
    /// Represents a service trade category (e.g. Electrician, Plumber).
    /// Managed exclusively by Admin; used to classify Workers and filter search results.
    /// </summary>
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Category name must be between 2 and 100 characters.")]
        [Display(Name = "Category Name")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Relative or absolute URL to the category's display icon.
        /// Example: "/icons/electrician.svg"
        /// </summary>
        [StringLength(300)]
        [Display(Name = "Icon URL")]
        public string IconUrl { get; set; } = string.Empty;

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ─── Navigation Properties ─────────────────────────────────────

        /// <summary>
        /// Workers who belong to this category. (Category 1──* Worker)
        /// </summary>
        public virtual ICollection<Worker> Workers { get; set; }
            = new List<Worker>();

        /// <summary>
        /// Service requests tagged with this category. (Category 1──* ServiceRequest)
        /// </summary>
        public virtual ICollection<ServiceRequest> ServiceRequests { get; set; }
            = new List<ServiceRequest>();
    }
}