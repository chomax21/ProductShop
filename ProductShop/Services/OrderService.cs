using Microsoft.EntityFrameworkCore;
using ProductShop.Data;
using ProductShop.Models;
using ProductShop.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductShop.Services
{
    public class OrderService : IOrderRepository<Order>
    {
        private readonly ApplicationDbContext _db;

        public OrderService(ApplicationDbContext context)
        {
            _db = context;
        }
        public async Task<bool> CreateOrder(Order v)
        {
            if (v != null)
            {
                await Task.Run(() => _db.Orders.AddAsync(v));
                return true;
            }
            return false;
        }

        public bool DeleteOrder(string id)
        {
              var deleteOrder = _db.Orders.FirstOrDefaultAsync(x => x.UserId == id);           
            return true;
        }

        public Order GetOrderForShoppingCart(string id)
        {
            return _db.Orders.FirstOrDefault(x => x.UserId == id && x.isDone == false);
        }

        public IEnumerable<Order> GetOrders(string id)
        {
            return _db.Orders.Where(x => x.UserId == id).Include(x => x.VMProducts);
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
