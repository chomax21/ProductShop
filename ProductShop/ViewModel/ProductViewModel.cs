using ProductShop.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductShop.ViewModel
{
    public class ProductViewModel
    {
        [Key]
        public int Id { get; set; }
        public int ShoppingCartId { get; set; }
        public int OrderId { get; set; }
        public int ProductCount { get; set; }
        
        [Required]
        public string Name { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string ProductComposition { get; set; }
        [Required]
        public string Manufacturer { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Discount { get; set; }
        public bool HaveDiscount { get; set; }
        public int Count { get; set; }

        public bool IsDeleted { get; set; } = false;

        public List<Order> Orders { get; set; } = new List<Order>();
    }
}
