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
using System.Collections.Generic;
using Xunit;

namespace ProductShop.Tests
{
    public class ProductControllerTetst
    {
        [Fact]
        public void CreateProductTest() // Тест Get-метода, CreateProduct.
        {
            var mock = new Mock<IProductRepository<Product>>();
            var mockLogger = new Mock<ILogger<ProductController>>();
            var mockWebHostEnv = new Mock<IWebHostEnvironment>();
            var productController = new ProductController(mockLogger.Object, mock.Object, mockWebHostEnv.Object);

            var result = productController.CreateProduct();

            Assert.NotNull(result.Result);
        }

        [Fact]
        public void CreateProductTestWithArgument_AndHaveError_InModelState() // Тест Post-метода, CreateProduct. С имитацией наличия ошибки в Model.State.
        {
            var mock = new Mock<IProductRepository<Product>>();
            var mockLogger = new Mock<ILogger<ProductController>>();
            var mockWebHostEnv = new Mock<IWebHostEnvironment>();
            var productController = new ProductController(mockLogger.Object, mock.Object, mockWebHostEnv.Object);
            var viewModel = TestCreateProduct();

            productController.ModelState.AddModelError("Invalid", "InvalidInput");
            var result = productController.CreateProduct(viewModel);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result.Result);
            Assert.Equal("Error", redirectToActionResult.ActionName);
            Assert.NotNull(result);
        }

        [Fact]
        public void CreateProductTestWithArgumentHas_NoErrors_InModelState() // Тест Post-метода, CreateProduct. С имитацией успешного выполнения.
        {
            ITempDataProvider tempDataProvider = Mock.Of<ITempDataProvider>();
            TempDataDictionaryFactory tempDataDictionaryFactory = new TempDataDictionaryFactory(tempDataProvider);
            ITempDataDictionary tempData = tempDataDictionaryFactory.GetTempData(new DefaultHttpContext());
            var mock = new Mock<IProductRepository<Product>>();
            var productController = new ProductController(null, mock.Object, null);
            var viewModel = TestCreateProduct();
            productController.TempData = tempData;

            var result = productController.CreateProduct(viewModel);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result.Result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.NotNull(result);
        }

        [Fact]
        public void UpdateProductTest_Success_Get() // Тест определяющий возвращается ли на страницу модель со значением null.
        {
            var mock = new Mock<IProductRepository<Product>>();
            var productController = new ProductController(null, mock.Object, null);
            int? intValue = null;

            var result = productController.UpdateProduct(intValue);
            var viewResult = result.Result as ViewResult;

            Assert.Null(viewResult.Model);
        }

        [Fact]
        public void UpdateProductTest_ViewModelInArgument_Post()
        {
            var mock = new Mock<IProductRepository<Product>>();
            var productController = new ProductController(null, mock.Object, null);

            var result = productController.UpdateProductPost(TestCreateProductForUpdateMethod());

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result.Result);
            Assert.Equal("Error", redirectToActionResult.ActionName);
            Assert.NotNull(result);
        }

        private ProductViewModel TestCreateProductForUpdateMethod()
        {
            ProductViewModel viewModel = new ProductViewModel()
            {
                Name = "Кекс",
                Category = "99999",
                Count = 10,
                Description = "Вкусно",
                Discount = 1M,
                HaveDiscount = false,
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
        } // Приватный метод для предоставления модели методу UpdateProduct.

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
        } // Приватный метод для предоставления модели методу CreateProduct.
    }
}
