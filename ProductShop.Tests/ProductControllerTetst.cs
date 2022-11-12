using Castle.Core.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using ProductShop.Controllers;
using ProductShop.Models;
using ProductShop.Services;
using ProductShop.ViewModel;
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
            var mock = new Mock<IProductRepository<Product>>();
            var mockLogger = new Mock<ILogger<ProductController>>();
            var mockWebHostEnv = new Mock<IWebHostEnvironment>();
            var productController = new ProductController(mockLogger.Object, mock.Object, mockWebHostEnv.Object);

            var result = productController.CreateProduct();            

            Assert.NotNull(result);
        }

        [Fact]
        public void CreateProductTestWithArgument_AndHaveError_InModelState()
        {
            var mock = new Mock<IProductRepository<Product>>();            
            var mockLogger = new Mock<ILogger<ProductController>>();
            var mockWebHostEnv = new Mock<IWebHostEnvironment>();
            var productController = new ProductController(mockLogger.Object, mock.Object, mockWebHostEnv.Object);
            var viewModel = TestCreateProduct();

            productController.ModelState.AddModelError("Invalid","InvalidInput");
            var result = productController.CreateProduct(viewModel);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result.Result);
            Assert.Equal("Error", redirectToActionResult.ActionName);
            Assert.NotNull(result);
        }

        [Fact]
        public void CreateProductTestWithArgumentHas_NoErrors_InModelState()
        {
            ITempDataProvider tempDataProvider = Mock.Of<ITempDataProvider>();
            TempDataDictionaryFactory tempDataDictionaryFactory = new TempDataDictionaryFactory(tempDataProvider);
            ITempDataDictionary tempData = tempDataDictionaryFactory.GetTempData(new DefaultHttpContext());
            var mock = new Mock<IProductRepository<Product>>();
            //var mockLogger = new Mock<ILogger<ProductController>>();
            //var mockWebHostEnv = new Mock<IWebHostEnvironment>();
            var productController = new ProductController(null, mock.Object, null);
            var viewModel = TestCreateProduct();
            productController.TempData = tempData;            

            var result = productController.CreateProduct(viewModel);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result.Result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.NotNull(result);
        }



        private ProductViewModel TestCreateProduct()
        {
            ProductViewModel viewModel = new ProductViewModel()
            {
                Name = "Кекс",
                Category = "1",
                Count = 10,
                Description = "Вкусно",
                Discount = 1M,
                HaveDiscount = false,
                Id = 1,
                IsDeleted = false,
                DiscountedPrice = 1M,
                Manufacturer = "Хлебозавод",
                OrderId = 1,
                Orders = new List<Order>(),
                OriginProductId = 1,
                Photo = null,
                PhotoPath = null,
                Price = 0M,
                stringDiscount = "0,01",
                ProductComposition = string.Empty,
                ProductCount = 10,
                ShoppingCartId = 1,
                stringPrice = "100,99"
            };
            return viewModel;
        }
    }
}
