using ProductShop.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProductShop.ViewModel
{
    public class ProductCategoryViewModel
    {
        public ProductCategory Category { get; set; }
        public List<ProductCategory> productCategories { get; set; }
        [Required(ErrorMessage = "Нужно ввести новую категорию продукта!")]
        public string setValue { get; set; }
    }
}
