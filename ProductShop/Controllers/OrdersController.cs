using Microsoft.AspNetCore.Mvc;
using ProductShop.Models;
using ProductShop.Services;
using Microsoft.AspNetCore.Identity;

namespace ProductShop.Controllers
{
    public class OrdersController : Controller
    {
        private readonly SQLProductRepository _db;
        public OrdersController(IProductRepository<Product> repository) 
        {
            _db = (SQLProductRepository)repository;
        }

        public void CreateOrder(Order orders)
        {
            if (ModelState.IsValid)
            {
                
                var order = _db.CreateOrder(orders);
            }            
        }
    }
}
