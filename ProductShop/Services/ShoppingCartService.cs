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
    public class ShoppingCartService : IShoppingCart<ShopingCart>
    {
        private readonly ApplicationDbContext _db;

        public ShoppingCartService(ApplicationDbContext context)
        {
            _db = context;
        }
      

        public void DeleteSgoppingCart(string id)
        {
            var deleteCart = _db.ShopingCarts.FirstOrDefault(x => x.UserId == id);
            if (deleteCart != null)
            {
                _db.Remove(deleteCart);
            }
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

        public ShopingCart GetShoppingCart(string id)
        {
            var oldCart = _db.ShopingCarts.Include(x => x.Order).ThenInclude(x => x.VMProducts);
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
    }
}
