using ProductShop.ViewModel;
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
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime OrderDateTime { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalSum { get; set; }
        [NotMapped]
        public string TotalSumString { get; set; }
        public bool isDone { get; set; }
        public bool isPayed { get; set; }
        public List<ProductViewModel> VMProducts { get; set; } = new List<ProductViewModel>();

    }
}
