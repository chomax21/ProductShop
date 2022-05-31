using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductShop.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int OrderNumber { get; set; }
        public DateTime OrderDateTime { get; set; } = DateTime.Now;
        public string OrderList { get; set; }
        public int TotalSum { get; set; }

    }
}
