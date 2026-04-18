using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain_layer.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty; 
        public string IconUrl { get; set; } = string.Empty;

  
        public virtual ICollection<Worker> Workers { get; set; } = new List<Worker>();

    }
}
