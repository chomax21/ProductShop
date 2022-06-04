using System.Collections.Generic;

namespace ProductShop.Models
{
    public class ShopingCart
    {
        public ShopingCart()
        {
        }
        public ShopingCart(string NewUserId)
        {
            UserId = NewUserId;
        }
        public int Id { get; set; }
        public string UserId { get; set; }
        public Order Order { get; set; }
        public bool IsDone { get; set; }
    }
}
