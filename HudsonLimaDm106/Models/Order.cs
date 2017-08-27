using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HudsonLimaDm106.Models
{
    public class Order
    {
        public Order()
        {
            this.OrderItems = new HashSet<OrderItem>();
        }

        public int Id { get; set; }

        public string userEmail { get; set; }

        public DateTime dataPedido { get; set; }

        public DateTime dataEntrega { get; set; }

        public string  statusPedido { get; set; }

        public decimal precoPedido { get; set; }

        public decimal pesoPedido { get; set; }

        public decimal precoFrete { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}