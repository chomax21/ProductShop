using ProductShop.Data;
using ProductShop.Models;
using System.Collections.Generic;
using System.Linq;

namespace ProductShop.Services
{
    public class OrderService : IOrderRepository<Order>
    {
        private readonly ApplicationDbContext _db;

        public OrderService(ApplicationDbContext context)
        {
            _db = context;
        }
        public bool CreateOrder(Order v)
        {
            var newOrder = _db.Orders.Add(v);
            return true;
        }

        public bool DeleteOrder(string id)
        {
            var deleteOrder = _db.Orders.FirstOrDefault(x => x.UserId == id);
            _db.Remove(deleteOrder);
            return true;
        }

        public Order GetOrderForShoppingCart(string id)
        {
            return _db.Orders.FirstOrDefault(x => x.UserId == id && x.isDone == false);
        }

        public IEnumerable<Order> GetOrders(string id)
        {
            return _db.Orders.Where(x => x.UserId == id);
        }

        public bool UpdateOrder(Order t)
        {
            if (t != null)
            {
                _db.Orders.Update(t);
                _db.Entry(t).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                return true;
            }
            return false;
        }
    }
}
