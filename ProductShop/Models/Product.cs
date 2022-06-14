using ProductShop.ViewModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductShop.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string ProductComposition { get; set; }
        public string Manufacturer { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public bool IsDeleted { get; set; }
        public int Count { get; set; }
        public int CountInShoppingcart { get; set; }
        public List<Order> Orders { get; set; } = new List<Order>();

        public static implicit operator ViewModelProduct(Product product)
        {
            return new ViewModelProduct
            {
                Id = product.Id,
                Name = product.Name,
                Category = product.Category,
                Description = product.Description,
                ProductComposition = product.ProductComposition,
                Manufacturer = product.Manufacturer,
                Price = product.Price,
                IsDeleted = product.IsDeleted,
                Count = product.Count
                
            };
        }

    }
}
