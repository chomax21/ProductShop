﻿using Microsoft.AspNetCore.Mvc;
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
        private readonly IQuantityProduct<ProductQuantityInCart> _quantity;

        public ShoppingController(IProductRepository<Product> repository,
            UserManager<IdentityUser> userManager,
            IOrderRepository<Order> orderRepository,
            IShoppingCart<ShopingCart> shoppingCartService,
            IQuantityProduct<ProductQuantityInCart> quantity)
        {
            _db = (SQLProductRepository)repository;
            _userManager = userManager;
            _order = orderRepository;
            _shoppingCart = shoppingCartService;
            _quantity = quantity;
        }

        [Authorize]
        public IActionResult GetShoppingCart()
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
                    return View(shopingCart); // Возвращаем корзину на страницу.
                }
                else
                {
                    shopingCart.Order = order;
                    _shoppingCart.UpdateShoppingCartInDb(shopingCart);
                    return View(shopingCart);
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
                    var newProduct = _db.GetProductById(ProductId);
                    var checkProduct = CheckingQuantityProduct(ProductId, shopingCart);
                    if (checkProduct.Item1)
                    {
                        var oldProduct = shopingCart.Order.Products.Find(x => x.Id == checkProduct.Item2);
                        var quantity = _quantity.GetQuantity(oldProduct.Id, shopingCart.Id);
                        _quantity.SetQuantity(oldProduct.Id,shopingCart.Id, quantity++);
                        shopingCart.Order.TotalSum += oldProduct.Price;
                    }
                    if (!checkProduct.Item1)
                    {
                        var quantity = _quantity.GetQuantity(newProduct.Id, shopingCart.Id);
                        _quantity.SetQuantity(newProduct.Id, shopingCart.Id, quantity++);
                        shopingCart.Order.TotalSum += newProduct.Price;
                        shopingCart.Order.Products.Add(newProduct);
                    }
                    _shoppingCart.UpdateShoppingCartInDb(shopingCart);
                    _order.UpdateOrder(shopingCart.Order);
                    _db.Save();
                    ShoppingCartViewModel cartViewModel = new ShoppingCartViewModel()
                    {
                        Id = shopingCart.Id,
                        UserId = shopingCart.UserId,
                        IsDone = shopingCart.IsDone
                        
                    };
                    //cartViewModel.Order = shopingCart.Order
                    return View("GetShoppingCart", shopingCart);
                }
                else
                {
                    Order newOrder = new Order(UserId);
                    var newProduct = _db.GetProductById(ProductId);
                    shopingCart.Order = newOrder;
                    shopingCart.Order.Products.Add(newProduct);
                    _order.UpdateOrder(shopingCart.Order);
                    _shoppingCart.AddShoppingCartInDb(shopingCart);
                    _db.Save();
                    return View("GetShoppingCart", shopingCart);
                }
            }
            return View("GetShoppingCart", shopingCart);
        }

        private (bool, int) CheckingQuantityProduct(int id, ShopingCart shopingCart) // Метод проверки наличия данного товара в корзине.
        {
            var result = shopingCart.Order.Products.FirstOrDefault(x => x.Id == id); // Ищем есть ли в корзине продукт с таким же ID.

            if (result != null)
            {
                return (true, result.Id); // Если есть, то возвращаем кортеж из булевого значения(true) и ID этого продукта. 
            }
            return (false, 0);
        }

        [Authorize]
        public IActionResult DeleteProductInCart(int id)
        {
            string UserId = _userManager.GetUserId(User);
            var shopingCart = _shoppingCart.GetShoppingCart(UserId);
            var addedProduct = _db.GetProductById(id);
            var checkProduct = CheckingQuantityProduct(id, shopingCart);
            var oldProduct = shopingCart.Order.Products.Find(x => x.Id == checkProduct.Item2);
            if (checkProduct.Item1 && oldProduct.CountInShoppingcart > 0)
            {
                oldProduct.CountInShoppingcart--;
                shopingCart.Order.TotalSum -= oldProduct.Price;
                _shoppingCart.UpdateShoppingCartInDb(shopingCart);
                _db.Save();
            }
            if (addedProduct != null && oldProduct.CountInShoppingcart == 0)
            {
                shopingCart.Order.Products.Remove(addedProduct);
                if (shopingCart.Order.Products.Count == 0)
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
