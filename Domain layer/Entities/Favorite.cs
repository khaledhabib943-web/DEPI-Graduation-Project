using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain_layer.Entities
{
    public class Favorite
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int WorkerId { get; set; }

        public virtual Customer Customer { get; set; } = null!;
        public virtual Worker Worker { get; set; } = null!;

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
