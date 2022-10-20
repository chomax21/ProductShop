using ProductShop.Controllers;
using Xunit;

namespace ProductShop.Tests
{
    public class HomeControllerTests
    {
        [Fact]
        public void IndexViewDataMessage()
        {
            //ILogger<HomeController> logger;
            //UserManager<ApplicationUser> userManager;
            //ApplicationDbContext db;

            HomeController controller = new(null, null, null);

            //ViewResult result = controller.Index() as ViewResult;

            //Assert.Equal("Ммм вкусные кексы...", result?.ViewData["Begin"]);
        }

        [Fact]
        public void IndexViewResultNotNull()
        {
            HomeController controller = new(null, null, null);

            //ViewResult result = controller.Index() as ViewResult;

            //Assert.NotNull(result);
        }

        [Fact]
        public void IndexViewNameEqualIndex()
        {
            HomeController controller = new(null, null, null);

            //var result = controller.Index() as Task<ViewResult>;

            //Assert.Equal("Index", result?.ViewName);
        }
    }



}
