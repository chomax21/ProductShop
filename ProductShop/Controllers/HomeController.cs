using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductShop.Data;
using ProductShop.Models;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProductShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager, ApplicationDbContext db)
        {
            _logger = logger;
            _userManager = userManager;
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string message)
        {
            if (await _db.Users.FirstOrDefaultAsync(x=> x.Email == "Admin@mail.ru") == null)
            {
                await FirstInitialDb();
            }

            return View(message);
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

        private async Task FirstInitialDb()
        {
            string adminEmail = "Admin@mail.ru";
            string adminPassword = "_adminPass1";
            var user = new IdentityUser { UserName = adminEmail, Email = adminEmail };
            var result = await _userManager.CreateAsync(user, adminPassword);
            if (result.Succeeded)
            {
                var claim = new Claim("IsAdmin", "true"); // Claim - Создание утверждения связанного с пользователем.
                await _userManager.AddClaimAsync(user, claim); // Через сервис UserManager<IdentityUser>, метод AddClaimAsync - добавляем для юзера, утверждение.
            }
        }
    }
}
