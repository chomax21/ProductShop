using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductShop.Models;
using System.Linq;
using System.Threading.Tasks;

namespace ProductShop.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [Authorize("AdminRights")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await Task.Run(() => _userManager.Users.ToList()); 
            if (users!=null)
            {
                return View(users);
            }
            return View(null);
        }
    }
}
