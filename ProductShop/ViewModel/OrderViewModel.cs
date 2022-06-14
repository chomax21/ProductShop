using ProductShop.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductShop.ViewModel
{
    public class OrderViewModel
    {
        public OrderViewModel() { }
        public OrderViewModel(string Id)
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
        public List<ViewModelProduct> Products { get; set; } = new List<ViewModelProduct>();

        public static explicit operator OrderViewModel(List<Product> v)
        {
            throw new NotImplementedException();
        }
    }
}
