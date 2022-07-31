using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProductShop.Data;
using ProductShop.Models;
using ProductShop.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductShop.Services
{
    public class OrderService : IOrderRepository<Order>
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _db = context;
            _userManager = userManager;
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

        public IEnumerable<Order> GetOrdersByDate(string start, string end)
        {
            return  _db.Orders.Where(x => x.OrderDateTime >= Convert.ToDateTime(start) && x.OrderDateTime <= Convert.ToDateTime(end));
        }

        public IEnumerable<Order> GetOrderByCustomerName(string firstName ="", string middleName = "", string lastName = "")
        {
            var users = _db.Users.Where(x => x.FirstName == firstName || x.LastName == lastName || x.MiddleName == middleName);
            List<string> idUsers = new List<string>();
            List<Order> orders = new List<Order>();
            foreach (var user in users.Distinct())
            {
                idUsers.Add(user.Id);
            }
            for (int i = 0; i < idUsers.Count - 1; i++)
            {
                var searchOrders = _db.Orders.Where(x => x.UserId == idUsers[i]).Include(x=> x.VMProducts);
                foreach (var item in searchOrders)
                {
                    orders.Add(item);
                }
                
            }
                return orders.Distinct();        
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
