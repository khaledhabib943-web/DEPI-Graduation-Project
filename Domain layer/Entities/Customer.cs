using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain_layer.Entities
{
    public class Customer : User
    {
        public string Address { get; set; } = string.Empty;

        public virtual ICollection<ServiceRequest> ServiceRequests { get; set; }
            = new List<ServiceRequest>();

        public virtual ICollection<Favorite> Favorites { get; set; }
            = new List<Favorite>();

        public virtual ICollection<Review> Reviews { get; set; }
            = new List<Review>();

        public virtual ICollection<Complaint> Complaints { get; set; }
            = new List<Complaint>();
    }
}
