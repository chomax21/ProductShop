using Microsoft.AspNetCore.Mvc;
using ProductShop.Models;
using ProductShop.Services;
using Microsoft.AspNetCore.Identity;
using ProductShop.Interfaces;
using ProductShop.ViewModel;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

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

        [Authorize]
        public IActionResult GetShoppingCartAction()
        {
            string UserId = _userManager.GetUserId(User); // Ищем идентифиатор юзера выполнившего запрос.
            var shopingCart = _shoppingCart.GetShoppingCart(UserId); // Ищем не оконченную корзину, если такая есть выводим ее, если нет выводим Новую корзину. 
            if (shopingCart != null)
            {
                Order order = _order.GetOrderForShoppingCart(UserId); // Ищем не завершенный заказ, который будет привязан к корзине найденной ранее. Такой у юзера должен быть только ОДИН.
                if (order == null)
                {
                    Order nOrder = new Order(); // Если у пользователя нет заказа, создаем новый.
                    nOrder.UserId = UserId; // Присваиваем заказу ID пользователя.
                    _order.CreateOrder(nOrder); // Создаем сам заказ в базе данных для сохранения.
                    shopingCart.Order = nOrder; // Помещяем созданный заказ в корзину покупателя(пользователя).
                    _shoppingCart.UpdateShoppingCartInDb(shopingCart); // Обновляем корзину покупателя в базе данных.
                    _db.Save(); // Сохраняем все изменения.
                    return View("GetShoppingCart",shopingCart); // Возвращаем корзину на страницу.
                }
                else
                {
                    shopingCart.Order = order;
                    _shoppingCart.UpdateShoppingCartInDb(shopingCart);
                    return View("GetShoppingCart",shopingCart);
                }

            }
            return BadRequest();
        }

        [Authorize]
        public IActionResult AddProductInCart(int ProductId)
        {
            string UserId = _userManager.GetUserId(User); // Ищем идентифиатор юзера выполнившего запрос.
            var shopingCart = _shoppingCart.GetShoppingCart(UserId); // Ищем не оконченную корзину, если такая есть выводим ее, если нет выводим Новую корзину.
            if (shopingCart != null)
            {
                //Order order = _order.GetOrderForShoppingCart(UserId); // Ищем не завершенный заказ, который будет привязан к корзине найденной ранее. Такой у юзера должен быть только ОДИН.
                if (shopingCart.Order != null)
                {
                    //shopingCart.Order.Products = order.Products; // Присваиваем список продуктов из не завершенного заказа в корзину.
                    Product originProduct = _db.GetProductById(ProductId);
                    
                    ProductViewModel checkProduct = CheckingQuantityProduct(ProductId, shopingCart);
                    if (checkProduct != null)
                    {
                        checkProduct.ProductCount++;
                        shopingCart.Order.TotalSum += checkProduct.Price;
                    }
                    else
                    {
                        ProductViewModel newProduct = MapProduct(originProduct);
                        shopingCart.Order.TotalSum += newProduct.Price;
                        newProduct.ShoppingCartId = shopingCart.Id;
                        newProduct.OrderId = shopingCart.Order.Id;
                        newProduct.ProductCount++;
                        shopingCart.Order.VMProducts.Add(newProduct);
                    }
                    _shoppingCart.UpdateShoppingCartInDb(shopingCart);
                    _order.UpdateOrder(shopingCart.Order);
                    _db.Save();
                    return View("GetShoppingCart", shopingCart);
                }
                else
                {
                    //Order newOrder = new Order(UserId);
                    //var newProduct = _db.GetProductById(ProductId);
                    //shopingCart.Order = newOrder;
                    //_order.UpdateOrder(shopingCart.Order);
                    //_shoppingCart.AddShoppingCartInDb(shopingCart);
                    //_db.Save();
                    return RedirectToAction("Error","Home");
                }
            }
            return View("GetShoppingCart", shopingCart);
        }

        private ProductViewModel MapProduct(Product product)
        {
            var productViewModel = new ProductViewModel
            {
                Category = product.Category,
                Count = product.Count,
                Description = product.Description,
                IsDeleted = product.IsDeleted,
                Manufacturer = product.Manufacturer,
                Name = product.Name,
                Price = product.Price,
                ProductComposition = product.ProductComposition,                
            };

            return productViewModel;
        }
        private ProductViewModel CheckingQuantityProduct(int id, ShopingCart shopingCart) // Метод проверки наличия данного товара в корзине.
        {
            var result = shopingCart.Order.VMProducts.FirstOrDefault(x => x.Id == id); // Ищем есть ли в корзине продукт с таким же ID.

            if (result != null)
            {
                return result; 
            }
            return null;
        }

        [Authorize]
        public IActionResult DeleteProductInCart(int id)
        {
            string UserId = _userManager.GetUserId(User);
            ShopingCart shopingCart = _shoppingCart.GetShoppingCart(UserId);
            ProductViewModel checkProduct = CheckingQuantityProduct(id, shopingCart);
            ProductViewModel oldProduct = shopingCart.Order.VMProducts.Find(x => x.Id == id);
            if (checkProduct!=null && oldProduct.ProductCount > 0)
            {
                oldProduct.ProductCount--;
                shopingCart.Order.TotalSum -= oldProduct.Price;
                _shoppingCart.UpdateShoppingCartInDb(shopingCart);
                _db.Save();
            }
            if (checkProduct != null && oldProduct.ProductCount == 0)
            {
                shopingCart.Order.VMProducts.Remove(checkProduct);
                if (shopingCart.Order.VMProducts.Count == 0)
                {
                    shopingCart.Order.TotalSum = default;
                }
                _db.Save();
            }

            return View("GetShoppingCart", shopingCart);
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
