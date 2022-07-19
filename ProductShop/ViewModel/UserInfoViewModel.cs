using ProductShop.Models;
using System.Collections.Generic;

namespace ProductShop.ViewModel
{
    public class UserInfoViewModel
    {
        public List<Order> Order { get; set; }
        public List<ShopingCart> ShopingCart { get; set; }
    }
}
