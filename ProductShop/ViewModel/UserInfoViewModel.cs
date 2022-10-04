using ProductShop.Models;
using System.Collections.Generic;

namespace ProductShop.ViewModel
{
    public class UserInfoViewModel
    {
        public List<Order> Order { get; set; } = new List<Order>();
        public UserFullName UserFullName { get; set; } = new UserFullName();       
        public OrderDateTime OrderDateTime { get; set; } = new OrderDateTime();
    }

    public class UserFullName
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
    }

    public class OrderDateTime
    {
        public string DateStart { get; set; }
        public string DateEnd { get; set; }
    }
}
