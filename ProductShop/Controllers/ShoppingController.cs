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
using System;
using System.Globalization;

namespace ProductShop.Controllers
{
    public class ShoppingController : Controller
    {
        private readonly SQLProductRepository _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOrderRepository<Order> _order;
        private readonly IShoppingCart<ShopingCart> _shoppingCart;
        private readonly ISaleService _saleServie;

        private decimal finalPrice { get; set; }

        public ShoppingController(IProductRepository<Product> repository,
            UserManager<ApplicationUser> userManager,
            IOrderRepository<Order> orderRepository,
            IShoppingCart<ShopingCart> shoppingCartService,
            ISaleService saleService)
        {
            _db = (SQLProductRepository)repository;
            _userManager = userManager;
            _order = orderRepository;
            _shoppingCart = shoppingCartService;
            _saleServie = saleService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetCart()
        {
            var cart = await _shoppingCart.GetShoppingCart(_userManager.GetUserId(User));
            cart.Order.TotalSum = 0;
            foreach (var product in cart.Order.VMProducts)
            {
                var price = _saleServie.GetDiscountInProduct(product.Id);
                product.Price = price;
                cart.Order.TotalSum += price * product.ProductCount;
            }
            return View("GetShoppingCart", cart);
        }

        [HttpPost]
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
                    shopingCart.Order = nOrder; // Помещаем созданный заказ в корзину покупателя(пользователя).
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
                if (shopingCart.Order != null)
                {                   
                    Product originProduct = await _db.GetProductById(ProductId);
                    originProduct.Category = await _db.GetOneValueInCategory(ProductId);
                    if (originProduct.HaveDiscount) // Проверяем есть ли скидка у этого товара.
                    {
                        var discountPrice  = _saleServie.GetDiscountInProduct(originProduct.Id); // Если она есть, то узнаем, какого она размера и пересчитываем цену на этот продукт.
                        if (discountPrice == -1)
                        {
                            throw new Exception("Ошибка в сервисе подсчета скидок!");
                        }
                        finalPrice = discountPrice;
                    }
                    else
                    {
                        finalPrice = originProduct.Price;
                    }
                    ProductViewModel checkProduct = CheckingQuantityProduct(ProductId, shopingCart); // Проверяем есть ли в корзине покупок товар с таким Id.
                    if (checkProduct != null) // Если такой товар уже есть. 
                    {
                        checkProduct.DiscountedPrice = finalPrice;
                        checkProduct.ProductCount++; // То просто увеличиваем его количество на 1.
                        shopingCart.Order.TotalSum += finalPrice; // Увеличиваем общую цену заказа.
                    }
                    else
                    {
                        ProductViewModel newProduct = MapProduct(originProduct);
                        shopingCart.Order.TotalSum += finalPrice;
                        newProduct.ShoppingCartId = shopingCart.Id;
                        newProduct.OrderId = shopingCart.Order.Id;
                        newProduct.ProductCount++;
                        newProduct.DiscountedPrice = newProduct.HaveDiscount ? finalPrice : 0;
                        shopingCart.Order.VMProducts.Add(newProduct);
                    }

                    await _shoppingCart.UpdateShoppingCartInDb(shopingCart);
                    await _db.Save();
                    return RedirectToAction("GetCart");
                }
                else
                {
                    return RedirectToAction("Error", "Home");
                }
            }
            return RedirectToAction("GetCart");
        }

        //private async Task<ShopingCart> CreatingAndConfiguringShoppingCart(ShopingCart shopingCart, int prodictId)
        //{
        //    //shopingCart.Order.Products = order.Products; // Присваиваем список продуктов из не завершенного заказа в корзину.
        //    Product originProduct = await _db.GetProductById(prodictId);
        //    if (originProduct.HaveDiscount) // Проверяем есть ли скидка у этого товара.
        //    {
        //        originProduct.Discount = _saleServie.GetDiscount(originProduct.Price, originProduct.Discount); // Если она есть, то узнаем, какого она размера и пересчитываем цену на этот продукт.
        //    }
        //    ProductViewModel checkProduct = CheckingQuantityProduct(prodictId, shopingCart); // Проверяем есть ли в корзине покупок товар с таким Id.
        //    if (checkProduct != null) // Если такой товар уже есть. 
        //    {
        //        checkProduct.ProductCount++; // То просто увеличиваем его количество на 1.
        //        shopingCart.Order.TotalSum += originProduct.Price; // Увеличиваем общую цену заказа.
        //    }
        //    else
        //    {
        //        ProductViewModel newProduct = MapProduct(originProduct);
        //        shopingCart.Order.TotalSum += newProduct.Price;
        //        newProduct.ShoppingCartId = shopingCart.Id;
        //        newProduct.OrderId = shopingCart.Order.Id;
        //        newProduct.ProductCount++;
        //        shopingCart.Order.VMProducts.Add(newProduct);
        //    }

        //    await _shoppingCart.UpdateShoppingCartInDb(shopingCart);
        //    _order.UpdateOrder(shopingCart.Order);
        //    await _db.Save();
        //}

        private ProductViewModel MapProduct(Product product)
        {
            var category = _db.GetOneValueInCategory(product.Id);
            product.Category = category.Result;
            string strPrice = product.Price.ToString();
            var rebuildStrPrice = Convert.ToDecimal(strPrice.Replace(',', '.'), CultureInfo.GetCultureInfo("en-Us"));

            var productViewModel = new ProductViewModel
            {
                Id = product.Id,
                Category = product.Category,
                Description = product.Description,
                IsDeleted = product.IsDeleted,
                Manufacturer = product.Manufacturer,
                Name = product.Name,
                Price = rebuildStrPrice,
                ProductComposition = product.ProductComposition,
                Discount = product.Discount,
                HaveDiscount = product.HaveDiscount,
                DiscountedPrice = product.DiscountedPrice
              
            };

            return productViewModel;
        }

        private Product MapViewProduct(ProductViewModel viewProduct)
        {
            var productViewModel = new Product
            {
                Id = viewProduct.Id,
                Category = viewProduct.Category,
                Description = viewProduct.Description,
                IsDeleted = viewProduct.IsDeleted,
                Manufacturer = viewProduct.Manufacturer,
                Name = viewProduct.Name,
                Price = viewProduct.Price,
                ProductComposition = viewProduct.ProductComposition,
                Discount = viewProduct.Discount,
                HaveDiscount = viewProduct.HaveDiscount
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

            if (oldProduct.HaveDiscount) // Проверяем есть ли скидка у этого товара.
            {
                var discountPrice = _saleServie.GetDiscountInProduct(oldProduct.Id); // Если она есть, то узнаем, какого она размера и пересчитываем цену на этот продукт.
                if (discountPrice == -1)
                {
                    throw new Exception("Ошибка в сервисе подсчета скидок!");
                }
                finalPrice = discountPrice;
            }
            else
            {
                finalPrice = oldProduct.Price;
            }

            if (checkProduct != null && oldProduct.ProductCount > 0)
            {
                oldProduct.ProductCount--;
                oldProduct.DiscountedPrice = finalPrice;
                shopingCart.Order.TotalSum -= finalPrice;
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

            return RedirectToAction("GetCart");
        }

        [Authorize]
        public async Task<IActionResult> MakePurchase() // Метод покупки.
        {
            ShopingCart shopingCart = await _shoppingCart.GetShoppingCart(_userManager.GetUserId(User)); // Метод в метод, так тоже работает, но читается ли?
            shopingCart.Order.isDone = true;
            shopingCart.IsDone = true;
            foreach (var product in shopingCart.Order.VMProducts)
            {
                var resultGetProduct = await _db.GetProductById(product.Id);
                product.Count = resultGetProduct.Count - product.ProductCount; // Вычитаем количество этого продукта в корзине из общего количества продуктов в наличии.
                await _db.UpateProduct(MapViewProduct(product));
            }
            await _shoppingCart.UpdateShoppingCartInDb(shopingCart);
            await _db.Save();
            TempData["SuccesMessage"] = $"Спасибо за покупку!";
            return RedirectToAction("Index","Home");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPaymentPage()
        {
            var shopingCart = await _shoppingCart.GetShoppingCart(_userManager.GetUserId(User));
            if (shopingCart !=null)
            {
                foreach (var product in shopingCart.Order.VMProducts) 
                {                    
                    if (_saleServie.GetDiscountInProduct(product.Id) == 0) // Проверяем наличие скидок на продукты и их финальные цены.
                    {
                        continue; 
                    }
                    else
                    {
                        var price = _saleServie.GetDiscountInProduct(product.Id); // Если цена этого продутка отличается, заменяем значение цены, в зависимости от наличия скидок и их размеров.                    
                        product.Price -= price;
                    }
                }
               
            }
            return View("PaymentPage", shopingCart);
        }
    }
}
