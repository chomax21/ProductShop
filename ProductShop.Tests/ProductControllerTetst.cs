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
    public class ProductControllerTetst
    {
        [Fact]
        public void CreateProductTest()
        {
            ProductController productController = new ProductController(null,null);

            Task<IActionResult> viewResult = productController.CreateProduct();

            Assert.NotNull(viewResult);
        }

        [Fact]
        public async Task CreateProductReturnViewTest()
        {
            ProductController productController = new ProductController(null, null);

            var viewResult = await productController.CreateProduct() as ViewResult;

            Assert.Equal("CreateProduct", viewResult?.ViewName);
        }
    }
}
