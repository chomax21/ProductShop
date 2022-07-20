using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductShop.Interfaces;
using ProductShop.Models;
using ProductShop.Services;
using ProductShop.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductShop.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOrderRepository<Order> _order;
        private readonly IShoppingCart<ShopingCart> _shoppingCart;

        public UserController(UserManager<ApplicationUser> userManager, IOrderRepository<Order> order, IShoppingCart<ShopingCart> shoppingCart)
        {
            _userManager = userManager;
            _order = order;
            _shoppingCart = shoppingCart;
        }

        [Authorize("AdminRights")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await Task.Run(() => _userManager.Users.ToList()); 
            if (users!=null)
            {
                return View(users);
            }
            return View(null);
        }

        [Authorize("AdminRights")]
        public async Task<IActionResult> GetUserInfo(string userId)
        {            
            var orders = await Task.Run(() => _order.GetOrders(userId));
            if (orders != null)
            {              
                return View(orders);
            }

            return RedirectToAction("Error");
          
        }
    }
}
