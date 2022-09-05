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
 
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Разделителем должна быть точка. Указывать цену в сотых долях, например : 9999.99")]
        [RegularExpression(@"^(\d{1,})([.][0-9]{1,2})?$")]
        public string stringPrice { get; set; } = string.Empty;

        public decimal Discount { get; set; }

        [Required (ErrorMessage = "Разделителем должна быть точка. Указывать цену в сотых долях, например : 9999.99")]
        [RegularExpression(@"^(\d{1,})([.][0-9]{1,2})?$")]
        public string stringDiscount { get; set; } = string.Empty;
        public bool HaveDiscount { get; set; }
        public int Count { get; set; }

        public bool IsDeleted { get; set; } = false;

        public List<Order> Orders { get; set; } = new List<Order>();
    }
}
