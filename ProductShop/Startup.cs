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

        public IConfiguration Configuration { get; } // �������� ����� ������� ���������� ��������� ������������ ����������.

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options => // ��������� � ����������� DbContexta EFCore.
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<ApplicationUser>(options => { // ��������� Identity.
                options.User.RequireUniqueEmail = true; // �������� �� ������������ Email.
                options.SignIn.RequireConfirmedAccount = false; // ������ � ���������������� ��� ������������� �����.                
            })
            .AddEntityFrameworkStores<ApplicationDbContext>();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            });
                       
            // ����������� �����������. ������� �������� �� ������� � ����� ��������� �����������. ���� ����������� �
            // ������������ ("IsAdmin", "true").
            // ���� �������, ���� ��������� ����� ����������� � �������� ������� ������� [Authorize("AdminRights")].
            // � ������� ���� ������������ � ����������� ����������� �� ������, ��� �������������� �� �������� �����/�����������. 
            // �� ��� ��� �� ����� � ������ ������ �������)
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminRights", policyBuilder => policyBuilder.RequireClaim("IsAdmin", "true"));
            });

            services.AddScoped<IProductRepository<Product>, SQLProductRepository>(); // ������������ ������ �����������. ��������� ��� ������ � Product.
            services.AddScoped<IOrderRepository<Order>, OrderService>(); // ������������ ������ �����������. ��������� ��� ������ � Orders.
            services.AddScoped<IShoppingCart<ShopingCart>, ShoppingCartService>(); // ������������ ������ �����������. ��������� ��� ������ � ShoppingCart.
            services.AddTransient<IEmailSender, EmailService>();
            services.AddTransient<ISaleService ,SaleService>(); // ������������ ������ ������.
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
