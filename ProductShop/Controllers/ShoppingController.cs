using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProductShop.Interfaces;
using ProductShop.Models;
using ProductShop.Services;
using ProductShop.ViewModel;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ProductShop.Controllers
{
    public class ShoppingController : Controller
    {
        private readonly SQLProductRepository _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOrderRepository<Order> _order;
        private readonly IShoppingCart<ShopingCart> _shoppingCart;
        private readonly ISaleService _saleServie;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ShoppingController> _logger;

        private decimal finalPrice { get; set; } // Используется для определения финальной цены продукта в корзине.

        public ShoppingController(IProductRepository<Product> repository,
            UserManager<ApplicationUser> userManager,
            IOrderRepository<Order> orderRepository,
            IShoppingCart<ShopingCart> shoppingCartService,
            ISaleService saleService,
            IEmailSender emailSender,
            ILogger<ShoppingController> logger)
        {            
            _db = (SQLProductRepository)repository;
            _userManager = userManager;
            _order = orderRepository;
            _shoppingCart = shoppingCartService;
            _saleServie = saleService;
            _emailSender = emailSender;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetCart() // Get  Метод предосталяющий корзину покупателя.
        {
            var cart = await _shoppingCart.GetShoppingCart(_userManager.GetUserId(User)); // Через UserManager определяем Id пользователя, и далее по этому Id ищем корзину покупателя.
            if (cart.Order != null) // Если в корзине уже есть заказ, отображаем его. И все продукты находящиеся в нем. Если же нет, то просто пустую корзину.
            {
                IFormatProvider format = CultureInfo.GetCultureInfo("en-Us"); // Используется для корректного отображения значений decimal приведенного к строке с применением метода ToString(). Вместо запятой - точка.
                cart.Order.TotalSum = 0; // Тут начинается пересчет заказа на случай изменения в цене или присвоения скидки этому товару. Для этого общая цена обнуляется для дальнейшенго подсчета.
                foreach (var product in cart.Order.VMProducts) // Проходим по всем продуктам в заказе.
                {
                    var price = _saleServie.GetDiscountInProduct(product.Id); // Узнаем есть ли данного товара скидка и получаем итоговую цену.
                    var originProduct = _db.GetProductById(product.Id); // Получаем объект оригинального продукта. Объект обернут в Task.
                    product.HaveDiscount = originProduct.Result.HaveDiscount; // Если вдруг на момент получения карты цена изменилась, отмечаем это для правильного вывода.
                    product.Category = originProduct.Result.Category; // Проверяется категория продукта, вдруг была имзенена.
                    product.stringPrice = product.Price.ToString(format); // Для вывода цены используется переменная типа String.
                    product.stringDiscount = product.HaveDiscount ? price.ToString(format) : "---"; // Если продукт имеет скидку, то отобразим цену со скидкой. Иначе прочерки.

                    cart.Order.TotalSum += price * product.ProductCount; // Здесь расчитывается итоговая цена всего заказа.
                }
                cart.Order.TotalSumString = cart.Order.TotalSum.ToString(format); // Для вывода общей цены так же используется специальное свойство типа String.
                return View("GetShoppingCart", cart);
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

                if (shopingCart.Order == null)
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
                    if (shopingCart.Order.VMProducts.Count == 0) // Если из корзины были удалены все продукты, то фактически она обновлется.
                    {
                        shopingCart.Order.OrderDateTime = DateTime.UtcNow; // Поэтому при предоставлении корзины изменяем дату создания заказа.
                    }
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
            Product originProduct = await _db.GetProductById(ProductId);
            if (originProduct.Count == 0)
            {
                TempData["Error"] = $"К сожалению товара {originProduct.Name} в данный момент нет в наличии.";
                return RedirectToAction("Index","Home");
            }
            string UserId = _userManager.GetUserId(User); // Ищем идентифиатор юзера выполнившего запрос.
            var shopingCart = await _shoppingCart.GetShoppingCart(UserId); // Ищем не оконченную корзину, если такая есть выводим ее, если нет выводим Новую корзину.
            if (shopingCart != null)
            {
                if (shopingCart.Order != null)
                {
                    
                    if (originProduct.HaveDiscount) // Проверяем есть ли скидка у этого товара.
                    {
                        var discountPrice = _saleServie.GetDiscountInProduct(originProduct.Id); // Если она есть, то узнаем, какого она размера и пересчитываем цену на этот продукт.
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
                        checkProduct.HaveDiscount = originProduct.HaveDiscount;
                        checkProduct.DiscountedPrice = checkProduct.HaveDiscount ? finalPrice : 0;
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



        private ProductViewModel MapProduct(Product product)
        {
            IFormatProvider format = CultureInfo.GetCultureInfo("en-Us");

            var price = _saleServie.GetDiscountInProduct(product.Id);

            ProductViewModel model = new ProductViewModel()
            {
                Id = product.Id,
                Name = product.Name,
                Category = product.Category,
                Description = product.Description,
                Manufacturer = product.Manufacturer,
                ProductComposition = product.ProductComposition,
                IsDeleted = product.IsDeleted,
                Price = product.Price,
                DiscountedPrice = price,
                Count = product.Count,
                Discount = product.Discount,
                HaveDiscount = product.HaveDiscount,
                stringPrice = product.Price.ToString(format),
                stringDiscount = price.ToString(format)

            };

            return model;
        }

        private Product MapViewProduct(ProductViewModel viewModelProduct)
        {
            //CultureInfo culture = new CultureInfo("en-US"); // Англ локализация, для использования точки в паарметрах цен и скидок.
            //var resulParsePrice = Convert.ToDecimal(viewModelProduct.stringPrice, culture); // Парсим из строки значение цены в дробное число, Decimal.
            //var resultParseDiscount = Convert.ToDecimal(viewModelProduct.stringDiscount, culture); // Парсим из строки значение скидки в дробное число, Decimal.

            //var category = _db.GetOneValueInCategory(Convert.ToInt32(viewModelProduct.Category));
            //var rebuildStrPrice = Convert.ToDecimal((viewModelProduct.Price).ToString().Replace(',', '.'), CultureInfo.GetCultureInfo("en-Us"));

            Product model = new Product()
            {
                Id = viewModelProduct.Id,
                Name = viewModelProduct.Name,
                Category = viewModelProduct.Category,
                Description = viewModelProduct.Description,
                Manufacturer = viewModelProduct.Manufacturer,
                ProductComposition = viewModelProduct.ProductComposition,
                IsDeleted = viewModelProduct.IsDeleted,
                Price = viewModelProduct.Price,
                Count = viewModelProduct.Count, 
                Discount = viewModelProduct.Discount,
                HaveDiscount = viewModelProduct.HaveDiscount
            };

            return model;
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
            shopingCart.Order.isPayed = true;
            shopingCart.IsDone = true;
            foreach (var product in shopingCart.Order.VMProducts)
            {
                var resultGetProduct = await _db.GetProductById(product.Id);
                product.Count = resultGetProduct.Count - product.ProductCount; // Вычитаем количество этого продукта в корзине из общего количества продуктов в наличии.
                await _db.UpateProduct(MapViewProduct(product));
            }
            await _shoppingCart.UpdateShoppingCartInDb(shopingCart);
            await _db.Save();
            var user = _userManager.GetUserAsync(User);
            //await _emailSender.SendEmailAsync(user.Result.Email,"ChoShop",$"Магазин ChoShop привествует вас.\nВы совершили покупку на сумму {shopingCart.Order.TotalSumString}.\nСпасибо, что выбрали нас! Ждем вас снова!");
            _logger.LogInformation($"Пользователь {user.Result.MiddleName} {user.Result.FirstName} совершил покупку на сумму {shopingCart.Order.TotalSum}.");
            TempData["SuccesMessage"] = $"Спасибо за покупку!";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPaymentPage()
        {
            var shopingCart = await _shoppingCart.GetShoppingCart(_userManager.GetUserId(User));
            if (shopingCart != null)
            {
                shopingCart.Order.TotalSum = default;
                IFormatProvider format = CultureInfo.GetCultureInfo("en-Us");

                foreach (var product in shopingCart.Order.VMProducts)
                {
                    var originProduct = _db.GetProductById(product.Id);

                    if (!originProduct.Result.HaveDiscount) // Проверяем наличие скидок на продукты и их финальные цены. Если цена которая пришла соответвствует оригинальной цене, оставляем этот продукт и идем дальше.
                    {
                        shopingCart.Order.TotalSum += originProduct.Result.Price;
                        product.stringPrice = originProduct.Result.Price.ToString(format);
                    }
                    else
                    {
                        product.DiscountedPrice = _saleServie.GetDiscountInProduct(product.Id); // Если цена этого продутка отличается, заменяем значение цены, в зависимости от наличия скидок и их размеров.                     
                        product.stringDiscount = product.DiscountedPrice.ToString(format);
                        product.stringPrice = originProduct.Result.Price.ToString(format);
                        shopingCart.Order.TotalSum += product.DiscountedPrice;
                    }

                }

                shopingCart.Order.TotalSumString = shopingCart.Order.TotalSum.ToString(format);

            }
            return View("PaymentPage", shopingCart);
        }
    }
}
