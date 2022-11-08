using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProductShop.Data;
using ProductShop.Interfaces;
using ProductShop.Models;
using ProductShop.Services;

namespace ProductShop
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; } // Свойство через которое происходит получение конфигурации приложения.

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options => // Настройка и регистрация DbContexta EFCore.
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<ApplicationUser>(options => { // Настройка Identity.
                options.User.RequireUniqueEmail = true; // Проверка на уникальность Email.
                options.SignIn.RequireConfirmedAccount = false; // Доступ к функциональности без подтверждения почты.                
            })
            .AddEntityFrameworkStores<ApplicationDbContext>();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            });
                       
            // Настраиваем авторизацию. Создаем политику по которой и будет проходить авторизация. Ищем утверждение у
            // пользователя ("IsAdmin", "true").
            // Если находим, даем выполнить метод контроллера к которому привзан атрибут [Authorize("AdminRights")].
            // В случаее если аутенфикация и последующая авторизация не прошла, нас перенаправляют на страницу входа/регистрации. 
            // Но это уже не сдесь и совсем другая история)
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminRights", policyBuilder => policyBuilder.RequireClaim("IsAdmin", "true"));
            });

            services.AddScoped<IProductRepository<Product>, SQLProductRepository>(); // Регистрируем сервис репозитория. Интерфейс для работы с Product.
            services.AddScoped<IOrderRepository<Order>, OrderService>(); // Регистрируем сервис репозитория. Интерфейс для работы с Orders.
            services.AddScoped<IShoppingCart<ShopingCart>, ShoppingCartService>(); // Регистрируем сервис репозитория. Интерфейс для работы с ShoppingCart.
            services.AddTransient<IEmailSender, EmailService>();
            services.AddTransient<ISaleService ,SaleService>(); // Регистрируем сервис скидок.
            services.AddSingleton<PayService>();

            services.AddControllersWithViews();

            services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;               
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
