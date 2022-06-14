using ProductShop.Models;

namespace ProductShop.ViewModel
{
    public class ShoppingCartViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public OrderViewModel Order { get; set; }
        public bool IsDone { get; set; }

    }
}
