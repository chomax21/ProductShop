using ProductShop.Models;
using ProductShop.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ProductShop.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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

        public bool AddShoppingCartInDb(ShopingCart shopingCart)
        {
            if (shopingCart != null)
            {
                _db.ShopingCarts.Add(shopingCart);
                return true;
            }
            return false;
        }

        public bool UpdateShoppingCartInDb(ShopingCart shopingCart)
        {
            if (shopingCart != null)
            {
                _db.ShopingCarts.Update(shopingCart);
                _db.Entry(shopingCart).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                return true;
            }
            return false;
        }

        public IEnumerable<Order> GetOrders(string id)
        {
            return _db.Orders.Where(x => x.UserId == id);
        }

        public ShopingCart GetShoppingCart(string id)
        {
            var oldCart = _db.ShopingCarts.Include(x => x.Order).ThenInclude(x => x.Products);
            //var oldCart = _db.ShopingCarts.FirstOrDefault(x => x.UserId == id && x.IsDone == false);
            var newCard = oldCart.FirstOrDefault(x => x.UserId == id && x.IsDone == false);
            if (newCard != null)
            {
                return newCard;
            }
            ShopingCart newEmptyCart = new ShopingCart(id);
            _db.ShopingCarts.Add(newEmptyCart);
            _db.SaveChanges();
            return newEmptyCart;
        }

        public bool UpdateOrder(Order t)
        {
            if(t != null)
            {
                _db.Orders.Update(t);
                _db.Entry(t).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                return true;
            }
            return false;
        }
    }
}
