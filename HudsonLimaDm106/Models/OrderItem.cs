using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HudsonLimaDm106.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int quantidade { get; set; }

        // Foreign Key
        public int ProductId { get; set; }
        
        // Foreign Key
        public int OrderId { get; set; }

        // Navigation property -> Referência do produto em si
        public virtual Product Product { get; set; }

        
    }
}