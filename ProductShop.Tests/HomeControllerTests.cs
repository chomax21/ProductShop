using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProductShop.Controllers;
using ProductShop.Data;
using ProductShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            HomeController controller = new(null,null,null);

            ViewResult result = controller.Index() as ViewResult;

            Assert.Equal("Терпение и труд, все перетрут!!!", result?.ViewData["Begin"]);
        }

        [Fact]
        public void IndexViewResultNotNull()
        {
            HomeController controller = new(null, null, null);

            ViewResult result = controller.Index() as ViewResult;

            Assert.NotNull(result);
        }

        [Fact]
        public void IndexViewNameEqualIndex()
        {
            HomeController controller = new(null, null, null);

            ViewResult result = controller.Index() as ViewResult;

            Assert.Equal("Index", result?.ViewName);
        }
    }



}
