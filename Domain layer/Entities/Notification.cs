using Domain_layer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain_layer.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int UserId { get; set; }
        public virtual User User { get; set; } = null!;

        public int? RelatedRequestId { get; set; }
        public virtual ServiceRequest? RelatedRequest { get; set; }
    }
}
