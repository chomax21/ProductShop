using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using ProductShop.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProductShop.Authorize
{
    public class AdminInitializer       
    {
        public IConfiguration _configuration { get; }

        public AdminInitializer(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        // Класс для инициализации первого пользователя Администратора. Будет вызываться в классе Program.
        // Заработало. Был пропущен оператор await в условии проверки наличии такого юзера в БД.


        public static async Task InnitializeAsync(UserManager<ApplicationUser> userManager)
        {
            string AdminEmail = "MainAdmin21@mail.ru";
            string AdminPassword = "_adminPass21";
            
            if (await userManager.FindByEmailAsync(AdminEmail) == null)
            {
                var user = new ApplicationUser { UserName = AdminEmail, Email = AdminEmail, FirstName = "Администратор" };
                var result = await userManager.CreateAsync(user,AdminPassword);
                if (result.Succeeded)
                {
                    var claim = new Claim("IsAdmin","true"); // Claim - Создание утверждения связанного с пользователем.
                    await userManager.AddClaimAsync(user, claim); // Через сервис UserManager<IdentityUser>, метод AddClaimAsync - добавляем для юзера, утверждение.
                }
            }
        }

    }
}
