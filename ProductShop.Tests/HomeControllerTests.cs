using Microsoft.AspNetCore.Mvc;
using ProductShop.Controllers;
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
            HomeController controller = new();

            ViewResult result = controller.Index() as ViewResult;

            Assert.Equal("Терпение и труд, все перетрут!!!", result?.ViewData["Begin"]);
        }

        [Fact]
        public void IndexViewResultNotNull()
        {
            HomeController controller = new();

            ViewResult result = controller.Index() as ViewResult;

            Assert.NotNull(result);
        }

        [Fact]
        public void IndexViewNameEqualUndex()
        {
            HomeController controller = new();

            ViewResult result = controller.Index() as ViewResult;

            Assert.Equal("Index", result?.ViewName);
        }
    }

    

}
