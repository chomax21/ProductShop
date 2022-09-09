using ProductShop.Models;
using ProductShop.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ProductShop.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ProductShop.Services
{
    public class ShoppingCartService : IShoppingCart<ShopingCart>
    {
        private readonly ApplicationDbContext _db;

        public ShoppingCartService(ApplicationDbContext context)
        {
            _db = context;
        }


        public async Task DeleteSgoppingCart(string id)
        {
            var deleteCart = await _db.ShopingCarts.FirstOrDefaultAsync(x => x.UserId == id);
            if (deleteCart != null)
            {
                await Task.Run(() => _db.ShopingCarts.Remove(deleteCart));
            }
        }

        public async Task<bool> AddShoppingCartInDb(ShopingCart shopingCart)
        {
            if (shopingCart != null)
            {
                await _db.ShopingCarts.AddAsync(shopingCart);
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateShoppingCartInDb(ShopingCart shopingCart)
        {
            if (shopingCart != null)
            {
                var findShoppingCart = await _db.ShopingCarts.FindAsync(shopingCart.Id);
                if (findShoppingCart != null)
                {
                    findShoppingCart.UserId = shopingCart.UserId;
                    findShoppingCart.Order = shopingCart.Order;
                    findShoppingCart.IsDone = shopingCart.IsDone;
                    findShoppingCart.ProductId = shopingCart.ProductId;
                    await _db.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }

        public async Task<ShopingCart> GetShoppingCart(string userId)
        {
            var oldCart = _db.ShopingCarts.Include(x => x.Order).ThenInclude(x => x.VMProducts); // В начале подгружаем ВСЕ корзины и их связанные данные.
            var newCard = await Task<ShopingCart>.Factory.StartNew(() => oldCart.FirstOrDefault(x => x.UserId == userId && x.IsDone == false)); // Потом в списке корзин ищем корзину по Id и не законченную(т.е не закрытую).
            if (newCard != null)
            {
                return newCard;
            }
            ShopingCart newEmptyCart = new ShopingCart(userId); // Если по Id пользователя корзина не находится, создаем новую присваивая ей Id этого польхователя.
            await _db.ShopingCarts.AddAsync(newEmptyCart);
            await _db.SaveChangesAsync();
            return newEmptyCart;
        }

        public async Task<List<ShopingCart>> GetUserAllShoppingCarts(string userId)
        {
            return await _db.ShopingCarts.Where(x => x.UserId == userId).ToListAsync();
        }
    }
}
