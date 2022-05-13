using ProductShop.Models;
using System.Collections.Generic;

namespace ProductShop.ViewModel
{
    public class SearchByNameVIewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public string SearchString { get; set; } = "Empty";
    }
}
