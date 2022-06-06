﻿using Microsoft.AspNetCore.Mvc;
using ProductShop.Models;
using ProductShop.Services;
using Microsoft.AspNetCore.Identity;
using ProductShop.Interfaces;
using ProductShop.ViewModel;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

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
        public IActionResult GetShoppingCart()
        {
            string UserId = _userManager.GetUserId(User); // Ищем идентифиатор юзера выполнившего запрос.
            var shopingCart = _shoppingCart.GetShoppingCart(UserId); // Ищем не оконченную корзину, если такая есть выводим ее, если нет выводим Новую корзину. 
            if (shopingCart != null)
            {
                Order order = _order.GetOrderForShoppingCart(UserId); // Ищем не завершенный заказ, который будет привязан к корзине найденной ранее. Такой у юзера должен быть только ОДИН.
                if (order == null)
                {
                    Order nOrder = new Order();
                    nOrder.UserId = UserId;
                    _order.CreateOrder(nOrder);
                    shopingCart.Order = nOrder;
                    _shoppingCart.UpdateShoppingCartInDb(shopingCart);
                    _db.Save();
                    return View(shopingCart);
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
                Order order = _order.GetOrderForShoppingCart(UserId); // Ищем не завершенный заказ, который будет привязан к корзине найденной ранее. Такой у юзера должен быть только ОДИН.
                if (order != null)
                {
                    shopingCart.Order.Products = order.Products; // Присваиваем список продуктов из не завершенного заказа в корзину.
                    var newProduct = _db.GetProductById(ProductId);
                    shopingCart.Order.Products.Add(newProduct);
                    _shoppingCart.UpdateShoppingCartInDb(shopingCart);
                    _order.UpdateOrder(shopingCart.Order);
                    _db.Save();
                    return View("GetShoppingCart", shopingCart);
                }
                else
                {
                    Order newOrder = new Order();
                    newOrder.UserId = UserId;
                    _order.CreateOrder(newOrder);
                    _db.Save();
                    var findOrder = _order.GetOrderForShoppingCart(UserId);
                    var newProduct = _db.GetProductById(ProductId);
                    shopingCart.Order = findOrder;
                    shopingCart.Order.Products.Add(newProduct);
                    _order.UpdateOrder(shopingCart.Order);
                    _shoppingCart.AddShoppingCartInDb(shopingCart);
                    _db.Save();
                    return View("GetShoppingCart", shopingCart);
                }
            }
            return View("GetShoppingCart", shopingCart);
        }

        [Authorize]
        public IActionResult DeleteProductInCart(int id)
        {
            string UserId = _userManager.GetUserId(User);
            var shopingCart = _shoppingCart.GetShoppingCart(UserId);
            var addedProduct = _db.GetProductById(id);
            if (addedProduct != null)
            {
                shopingCart.Order.Products.Remove(addedProduct);
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