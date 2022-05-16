using ProductShop.Models;
using System.Collections.Generic;

namespace ProductShop.ViewModel
{
    public class SearchVIewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public string SearchString { get; set; } = "Empty";
        public int Id { get; set; }
    }
}
