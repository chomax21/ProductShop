using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductShop.Models
{
    public class Order
    {
        public Order() { }
        public Order(string Id)
        {
            UserId = Id;
        }
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDateTime { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalSum { get; set; }
        public bool isDone { get; set; }
        public bool isPayed { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();

    }
}
