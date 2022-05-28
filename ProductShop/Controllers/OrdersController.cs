using Microsoft.AspNetCore.Mvc;
using ProductShop.Models;
using ProductShop.Services;

namespace ProductShop.Controllers
{
    public class OrdersController : Controller
    {
        public OrdersController(IRepository<Product, Orders> repository) 
        {

        }
    }
}
