using ProductShop.Models;
using System.Collections.Generic;

namespace ProductShop.ViewModel
{
    public class ProductCategoryViewModel
    {
        public ProductCategory Category { get; set; }
        public List<ProductCategory> productCategories { get; set; }
        public string setValue { get; set; }
    }
}
