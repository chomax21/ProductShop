using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProductShop.Authorize
{
    public class AdminInitializer       
    {
        // Класс для инициализации первого пользователя Администратора. Будет вызываться в классе Program.
        // Планировалось использование этого пользователя для,
        // дальнейшей Аутентификации и Авторизации.
        // на 19 строке не проходит условие, не работает пока =(
        // Оставим пока это ненужный код. Будем искать альтернативы.
        // Надеюсь с опытом вернуться и разобраться.
        public static async Task InnitializeAsync(UserManager<IdentityUser> userManager)
        {
            string AdminEmail = "Admin21@mail.ru";
            string AdminPassword = "_adminPass21";
            if (userManager.FindByEmailAsync(AdminEmail) == null)
            {
                var user = new IdentityUser { UserName = AdminEmail, Email = AdminEmail };
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
