using Microsoft.AspNetCore.Mvc;
using ProductShop.Models;
using ProductShop.Services;
using Microsoft.AspNetCore.Identity;
using ProductShop.Interfaces;
using ProductShop.ViewModel;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<IActionResult> GetShoppingCartAction()
        {
            string UserId = _userManager.GetUserId(User); // Ищем идентифиатор юзера выполнившего запрос.
            var shopingCart = await _shoppingCart.GetShoppingCart(UserId); // Ищем не оконченную корзину, если такая есть выводим ее, если нет выводим Новую корзину. 
            if (shopingCart != null)
            {
                Order order = _order.GetOrderForShoppingCart(UserId); // Ищем не завершенный заказ, который будет привязан к корзине найденной ранее. Такой у юзера должен быть только ОДИН.
                if (order == null)
                {
                    Order nOrder = new Order(); // Если у пользователя нет заказа, создаем новый.
                    nOrder.UserId = UserId; // Присваиваем заказу ID пользователя.
                    await _order.CreateOrder(nOrder); // Создаем сам заказ в базе данных для сохранения.
                    shopingCart.Order = nOrder; // Помещяем созданный заказ в корзину покупателя(пользователя).
                    await _shoppingCart.UpdateShoppingCartInDb(shopingCart); // Обновляем корзину покупателя в базе данных.
                    await _db.Save(); // Сохраняем все изменения.
                    return View("GetShoppingCart", shopingCart); // Возвращаем корзину на страницу.
                }
                else
                {
                    shopingCart.Order = order;
                    await _shoppingCart.UpdateShoppingCartInDb(shopingCart);
                    return View("GetShoppingCart", shopingCart);
                }

            }
            return BadRequest();
        }

        [Authorize]
        public async Task<IActionResult> AddProductInCart(int ProductId)
        {
            await GetShoppingCartAction();
            string UserId = _userManager.GetUserId(User); // Ищем идентифиатор юзера выполнившего запрос.
            var shopingCart = await _shoppingCart.GetShoppingCart(UserId); // Ищем не оконченную корзину, если такая есть выводим ее, если нет выводим Новую корзину.
            if (shopingCart != null)
            {
                //Order order = _order.GetOrderForShoppingCart(UserId); // Ищем не завершенный заказ, который будет привязан к корзине найденной ранее. Такой у юзера должен быть только ОДИН.
                if (shopingCart.Order != null)
                {
                    //shopingCart.Order.Products = order.Products; // Присваиваем список продуктов из не завершенного заказа в корзину.
                    Product originProduct = await _db.GetProductById(ProductId);

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
                    await _shoppingCart.UpdateShoppingCartInDb(shopingCart);
                    _order.UpdateOrder(shopingCart.Order);
                    await _db.Save();
                    return View("GetShoppingCart", shopingCart);
                }
                else
                {
                    return RedirectToAction("Error", "Home");
                }
            }
            return View("GetShoppingCart", shopingCart);
        }

        private ProductViewModel MapProduct(Product product)
        {
            var productViewModel = new ProductViewModel
            {
                Category = product.Category,
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
        public async Task<IActionResult> DeleteProductInCart(int id)
        {
            string UserId = _userManager.GetUserId(User);
            ShopingCart shopingCart = await _shoppingCart.GetShoppingCart(UserId);
            ProductViewModel checkProduct = CheckingQuantityProduct(id, shopingCart);
            ProductViewModel oldProduct = shopingCart.Order.VMProducts.Find(x => x.Id == id);
            if (checkProduct != null && oldProduct.ProductCount > 0)
            {
                oldProduct.ProductCount--;
                shopingCart.Order.TotalSum -= oldProduct.Price;
                await _shoppingCart.UpdateShoppingCartInDb(shopingCart);
                await _db.Save();
            }
            if (checkProduct != null && oldProduct.ProductCount == 0)
            {
                shopingCart.Order.VMProducts.Remove(checkProduct);
                if (shopingCart.Order.VMProducts.Count == 0)
                {
                    shopingCart.Order.TotalSum = default;
                }
                await _db.Save();
            }

            return View("GetShoppingCart", shopingCart);
        }

        public async Task<IActionResult> MakePurchase()
        {
            string userId = _userManager.GetUserId(User);
            ShopingCart shopingCart = await _shoppingCart.GetShoppingCart(userId);
            shopingCart.Order.isDone = true;
            shopingCart.IsDone = true;
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPaymentPage()
        {
            var userId = _userManager.GetUserId(User);
            var shopingCart = await _shoppingCart.GetShoppingCart(userId);
            return View("PaymentPage", shopingCart);
        }
    }
}
