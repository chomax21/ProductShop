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

        [HttpGet]
        [Authorize("AdminRights")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await Task.Run(() => _userManager.Users.ToList()); 
            if (users!=null)
            {
                return View(users);
            }
            return View(null);
        }

        [HttpGet]
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

        [HttpGet]
        [Authorize("AdminRights")]
        public async Task<IActionResult> GetUserByDate(string start, string end)
        {
            var orders = await Task.Run(() => _order.GetOrdersByDate(start, end).ToList());
            if (orders != null)
            {
                return View(orders);
            }

            return RedirectToAction("Error");
        }

        [HttpGet]
        public  IActionResult GetUserOrderByName()
        {
            return View();
        }

        
        [Authorize("AdminRights")]
        public async Task<IActionResult> GetUserOrderByName(string firstName, string middleName, string lastName)
        {
            if (ModelState.IsValid)
            {
                UserInfoViewModel userInfo = new();
                 userInfo.Order = await Task.Run(() => _order.GetOrderByCustomerName(firstName, middleName, lastName).ToList());
                return View(userInfo);
            }
            return RedirectToAction("Error");
        }
    }
}
