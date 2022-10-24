using ProductShop.Models;
using System.Collections.Generic;

namespace ProductShop.ViewModel
{
    public class SearchVIewModel
    {
        public List<ProductViewModel> Products { get; set; } = new List<ProductViewModel>();
        public string SearchString { get; set; } = "Empty";
        public int Id { get; set; }
    }
}
