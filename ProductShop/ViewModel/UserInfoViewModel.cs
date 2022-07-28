using ProductShop.Models;
using System.Collections.Generic;

namespace ProductShop.ViewModel
{
    public class UserInfoViewModel
    {
        public List<Order> Order { get; set; }
        public List<ShopingCart> ShopingCart { get; set; }

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
    }
}
