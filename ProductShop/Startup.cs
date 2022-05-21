using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProductShop.Data;
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

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>();


            // Ќастраиваем авторизацию. —оздаем политику по которой и будет проходить авторизаци€. »щем утверждение у
            // пользовател€ ("IsAdmin", "true").
            // ≈сли находим, даем выполнить метод контроллера к которому привзан атрибут [Authorize("AdminRights")].
            // ¬ случаее если аутенфикаци€ и последующа€ авторизаци€ не прошла, нас перенаправл€ют на страницу входа/регистрации.
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminRights", policyBuilder => policyBuilder.RequireClaim("IsAdmin", "true"));
            });

            services.AddScoped<IRepository<Product>, SQLProductRepository>(); // —ервис репозитори€.

            services.AddControllersWithViews();
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
