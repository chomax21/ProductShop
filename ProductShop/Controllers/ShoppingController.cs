using Microsoft.AspNetCore.Mvc;
using ProductShop.Models;
using ProductShop.Services;
using Microsoft.AspNetCore.Identity;
using ProductShop.Interfaces;
using ProductShop.ViewModel;

namespace ProductShop.Controllers
{
    public class ShoppingController : Controller
    {
        private readonly SQLProductRepository _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IOrderRepository<Order> _order;
        private readonly IShoppingCart<ShopingCart> _shoppingCart;

        public ShoppingController(IProductRepository<Product> repository,
            UserManager<IdentityUser> userManager,
            IOrderRepository<Order> orderRepository,
            IShoppingCart<ShopingCart> shoppingCartService) 
        {
            _db = (SQLProductRepository)repository;
            _userManager = userManager;
            _order = orderRepository;
            _shoppingCart = shoppingCartService;
        }

        public IActionResult GetShoppingCart()
        {
            string UserId = _userManager.GetUserId(User);
            ShopingCart shopingCart = _shoppingCart.GetShoppingCart(UserId);
            Order order = _order.GetOrderForShoppingCart(UserId);
            shopingCart.Order.Products = order.Products;
            if (shopingCart != null)
            {
                return View(shopingCart);
            }
            return BadRequest();            
        }

        public IActionResult AddProductInCart(ShoppingCartViewModel shoppingCart, int id)
        {
            var addedProduct = _db.GetProductById(id);
            if (addedProduct != null)
            {
                shoppingCart.Order.Products.Add(addedProduct);
            }
            return View(shoppingCart);
        }

        public IActionResult DeleteProductInCart(ShoppingCartViewModel shoppingCart, int id)
        {
            var addedProduct = _db.GetProductById(id);
            if (addedProduct != null)
            {
                shoppingCart.Order.Products.Remove(addedProduct);
            }
            return View(shoppingCart);
        }
        public void CreateOrder(Order orders)
        {
            if (ModelState.IsValid)
            {
                var UserId = _userManager.GetUserId(User);
            }

            
        }
    }
}
