using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain_layer.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public int Rating { get; set; } // from 1:5
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

       // هنربط بال customer
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; } = null!;

        // كده هو هيروح لفين
        public int WorkerId { get; set; }
        public virtual Worker Worker { get; set; } = null!;

        
        public int ServiceRequestId { get; set; }
        public virtual ServiceRequest ServiceRequest { get; set; } = null!;
    }
}
