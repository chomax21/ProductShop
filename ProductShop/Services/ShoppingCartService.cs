using ProductShop.Models;
using ProductShop.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ProductShop.Data;
using System.Linq;

namespace ProductShop.Services
{
    public class ShoppingCartService : IOrderRepository<Order>, IShoppingCart<ShopingCart>
    {
        private readonly ApplicationDbContext _db;

        public ShoppingCartService(ApplicationDbContext context)
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

        public void DeleteSgoppingCart(string id)
        {
            var deleteCart = _db.ShopingCarts.FirstOrDefault(x => x.UserId == id);
            if (deleteCart != null)
            {
                _db.Remove(deleteCart);
            }
        }

        public Order GetOrderForShoppingCart(string id)
        {
            return _db.Orders.FirstOrDefault(x => x.UserId == id && x.isDone == false);
        }

        public IEnumerable<Order> GetOrders(string id)
        {
            return _db.Orders.Where(x => x.UserId == id);
        }

        public ShopingCart GetShoppingCart(string id)
        {
            var newCart = _db.ShopingCarts.FirstOrDefault(x => x.UserId == id && x.IsDone == false);
            if (newCart != null)
            {
                return newCart;
            }
            return new ShopingCart(id);
        }       
    }
}
