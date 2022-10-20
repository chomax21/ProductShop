using Microsoft.AspNetCore.Http;
using ProductShop.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductShop.ViewModel
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public int OriginProductId { get; set; }
        public int ShoppingCartId { get; set; }
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Введите кол-во единиц")]
        public int ProductCount { get; set; } // Количество продуктов в корзине.

        [Required]
        public string Name { get; set; }

        [Required(ErrorMessage = "Не выбрана категория продукта")]
        public string Category { get; set; }
        [Required(ErrorMessage = "Опишите продукт")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Опишите состав продукта")]
        public string ProductComposition { get; set; }
        [Required(ErrorMessage = "Кто производитель?")]
        public string Manufacturer { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountedPrice { get; set; }
        [Required(ErrorMessage = "Нужно указать цену")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [NotMapped]
        [RegularExpression(@"^(\d{1,})([.][0-9]{1,2})?$", ErrorMessage = "Разделителем должна быть точка.Указывать цену в сотых долях, например: 9999.99")]
        public string stringPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Discount { get; set; }

        [NotMapped]
        [RegularExpression(@"^\d{1}([.][0-9]{1,2})?$" , ErrorMessage = "Разделителем должна быть точка.Указывать размер скидки в сотых долях, например: 0.99")]

        public string stringDiscount { get; set; } 
        public bool HaveDiscount { get; set; }
        public int Count { get; set; } // Общее количество продуктов в наличии.

        public bool IsDeleted { get; set; }

        public List<Order> Orders { get; set; } = new List<Order>();

        [NotMapped]
        public IFormFile Photo { get; set; }
        public string PhotoPath { get; set; }
    }
}
