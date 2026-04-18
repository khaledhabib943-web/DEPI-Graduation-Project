using Domain_layer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain_layer.Entities
{
    public class ServiceRequest
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty; 
        public string Address { get; set; } = string.Empty;

        // دههه ال enum 
        public RequestStatus Status { get; set; } = RequestStatus.Pending;

        public DateTime ScheduledDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

      
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; } = null!;

    
        public int? WorkerId { get; set; } 
        public virtual Worker? Worker { get; set; }

     
        public virtual Review? Review { get; set; }
    }
}
