using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProductShop.Models;
using ProductShop.Services;
using ProductShop.ViewModel;
using System.Collections.Generic;

namespace ProductShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly SQLProductRepository _db;
        public ProductController(ILogger<ProductController> logger, IRepository<Product> repository)
        {
            _logger = logger;
            _db = (SQLProductRepository)repository;
        }
        [HttpGet]
        public IActionResult CreateProduct()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateProduct(ViewModelProduct viewModelProduct)
        {
            Product product = new Product() 
            {
                Id = viewModelProduct.Id,
                Name = viewModelProduct.Name,
                Category = viewModelProduct.Category,
                Description = viewModelProduct.Description,
                Manufacturer = viewModelProduct.Manufacturer,
                ProductComposition = viewModelProduct.ProductComposition
            };
            _db.CreateProduct(product);
            _db.Save();
            ViewData["Message"] = $"Продукт {viewModelProduct.Name} добавлен!";
            return View();
        }
        [HttpGet]
        public IActionResult UpdateProduct(int id)
        {
            var product = _db.GetProductById(id);
            ViewModelProduct viewModelProduct = new ViewModelProduct()
            {
                Id=product.Id,
                Name=product.Name,
                Category = product.Category,
                Description=product.Description,
                Manufacturer = product.Manufacturer,
                ProductComposition =product.ProductComposition
            };
            return View(viewModelProduct);
        }
        [HttpPost]
        public IActionResult UpdateProduct(ViewModelProduct viewModelProduct)
        {
            Product product = new Product()
            {
                Id = viewModelProduct.Id,
                Name = viewModelProduct.Name,
                Category = viewModelProduct.Category,
                Description = viewModelProduct.Description,
                Manufacturer = viewModelProduct.Manufacturer,
                ProductComposition = viewModelProduct.ProductComposition
            };
            bool resultOperation = _db.UpateProduct(product);
            _db.Save();
            ViewData["Message"] = $"Продукт {viewModelProduct.Name} отредактирован!";
            if (resultOperation)
            {
                return View();
            }
            return RedirectToAction("Error","Home");
        }
        [HttpPost]
        public IActionResult DeleteProduct(int? id)
        {
            if (id.HasValue)
            {
                _db.DeleteProduct(id.Value);
                var product = _db.GetProductById(id.Value);
                _db.Save();
                ViewData["Message"] = $"Продукт {product.Name} удален!";
                return RedirectToAction("Index","Home");
            }
            return RedirectToAction("Error", "Home");
        }

        [HttpGet]
        public IActionResult GetProductByName(string name)
        {
            var resultByName = _db.GetProductByName(name);
            return View(resultByName);
        }
        [HttpGet]
        public IActionResult GetProductById(int id)
        {
            var resultById = _db.GetProductById(id);
            return View(resultById);
        }
        [HttpGet]
        public IActionResult GetProductByManufacturer(string manufacturer)
        {
            var resultByManufacturer = _db.GetProductByName(manufacturer);
            return View(resultByManufacturer);
        }
        [HttpGet]
        public IActionResult GetProductByCategory(string category)
        {
            var resultByCategory = _db.GetProductByName(category);
            return View(resultByCategory);
        }
        [HttpGet]
        public IActionResult GetAllProducts()
        {
            IEnumerable<Product> products;
            products = _db.GetProducts();
            return View(products); 
        }

    }
}
