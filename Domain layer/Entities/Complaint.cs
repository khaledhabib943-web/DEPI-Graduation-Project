using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain_layer.Entities
{
    public class Complaint
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

   
        public bool IsResolved { get; set; } = false;

        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; } = null!;
        // momken tkon from customer (just)
        public int? ServiceRequestId { get; set; }
        public virtual ServiceRequest? ServiceRequest { get; set; }

    }
}
 