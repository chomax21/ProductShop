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
            if (deleteOrder != null)
            {
                _db.Orders.Remove(deleteOrder.Result);
                return true;
            }
            return false;
        }

        public Order GetOrderForShoppingCart(string id)
        {
            return _db.Orders.FirstOrDefault(x => x.UserId == id && x.isDone == false);
        }

        public IEnumerable<Order> GetOrders(string id)
        {
            return _db.Orders.Where(x => x.UserId == id).Include(x => x.VMProducts);
        }

        public UserInfoViewModel GetOrdersByDateOfPurchase(string start, string end)
        {
            var result = _db.Orders.Where(x => x.OrderDateTime >= Convert.ToDateTime(start) && x.OrderDateTime <= Convert.ToDateTime(end));
            UserInfoViewModel userInfo = new UserInfoViewModel();
            foreach (var order in result)
            {
                userInfo.Order.Add(order);
            }
            return userInfo;
        }

        public UserInfoViewModel GetOrderByCustomerName(string FirstName, string MiddleName, string LastName)
        {
            var users = from x in _db.Users
                        where x.FirstName.Contains(FirstName) || x.MiddleName.Contains(MiddleName) || x.LastName.Contains(LastName)
                        //where x.MiddleName.Contains(MiddleName)
                        //where x.LastName.Contains(LastName)
                        select x;

            List<string> idUsers = new List<string>();
            UserInfoViewModel userInfoView = new();
            foreach (var user in users.Distinct())
            {
                idUsers.Add(user.Id);
            }
            for (int i = 0; i <= idUsers.Count - 1; i++)
            {
                var searchOrders = _db.Orders.Where(x => x.UserId == idUsers[i]).Include(x=> x.VMProducts);
                var fullNameUser = _db.Users.FirstOrDefault(x => x.Id == idUsers[i]);
                foreach (var item in searchOrders)
                {
                    userInfoView.Order.Add(item);                    
                }
                userInfoView.UserFullName.FirstName = fullNameUser.FirstName;
                userInfoView.UserFullName.MiddleName = fullNameUser.MiddleName;
                userInfoView.UserFullName.LastName = fullNameUser.LastName;
            }
                return userInfoView;        
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
