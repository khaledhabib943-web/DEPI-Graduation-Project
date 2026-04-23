using Domain_layer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain_layer.Entities
{
    public class Worker : User
    {
        public string Bio { get; set; } = string.Empty; // نبذة عن الفني
        public double HourlyRate { get; set; } // سعر الساعة

        // صور للتوثيق (مطلوبة في الـ PDF صفحة 4)
        public string NationalIdImageFront { get; set; } = string.Empty;
        public string NationalIdImageBack { get; set; } = string.Empty;
        public string CriminalRecordImage { get; set; } = string.Empty; // فيش وتشبيه

        public string PortfolioVideoUrl { get; set; } = string.Empty;
        public bool IsVerified { get; set; } = false; // هل الأدمن وافق عليه؟

        // الربط مع القسم (الكهرباء، السباكة..)
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;

        // ─── العلاقات ─────────────────────────────────────
        public virtual ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        // The state starts at 0 (PendingApproval) automatically
        public AvailabilityStatus Availability { get; set; } = AvailabilityStatus.PendingApproval;
        // For the automatic "Offline" logic 
        public DateTime LastSeen { get; set; } = DateTime.UtcNow;
    }
}
