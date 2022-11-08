using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductShop.Data;
using ProductShop.Models;
using ProductShop.Services;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProductShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _originDataBase;
        private readonly IProductRepository<Product> _db;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, ApplicationDbContext dataBase, IProductRepository<Product> db)
        {
            _logger = logger;
            _userManager = userManager;
            _originDataBase = dataBase;
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories = _db.GetValuesInCategoryList();
            ViewBag.Category = categories.Result;
            var userName = await _userManager.GetUserAsync(User);
            if (userName != null)
            {
                ViewData["Begin"] = $"Привет {userName.FirstName} вернулся за нашими вкусными кексами...?";
            }
            else
            {
                ViewData["Begin"] = $"Привет ты еще не попробовал наши вкусные кексы...?";
            }
            
            return View("Index");           
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
       
    }
}
