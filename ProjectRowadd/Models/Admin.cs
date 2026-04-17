using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectRowadd.Models
{
    /// <summary>
    /// Represents a system administrator.
    /// Inherits shared identity fields from User.
    /// </summary>
    public class Admin : User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AdminId { get; set; }

        // ─── Navigation Properties ─────────────────────────────────────

        /// <summary>
        /// Categories created and managed by this admin.
        /// </summary>
        public virtual ICollection<Category> ManagedCategories { get; set; }
            = new List<Category>();

        /// <summary>
        /// Complaints reviewed and resolved by this admin.
        /// </summary>
        public virtual ICollection<Complaint> ResolvedComplaints { get; set; }
            = new List<Complaint>();
    }
}