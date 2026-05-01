using FinalProject.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Domain.Entities
{
    public abstract class User : IdentityUser<int>
    {
        // ── Bridge properties (backward-compatible, not mapped to DB) ──
        [NotMapped]
        public int UserId { get => Id; set => Id = value; }

        [NotMapped]
        public string Username { get => UserName ?? string.Empty; set => UserName = value; }

        [NotMapped]
        public string PasswordHashLegacy { get => PasswordHash ?? string.Empty; set => PasswordHash = value; }

        // ── Custom properties not in IdentityUser ──
        public string FullName { get; set; } = string.Empty;
        public string NationalId { get; set; } = string.Empty;
        public int Age { get; set; }
        public UserRole Role { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}