using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProductShop.Models;
using ProductShop.Services;
using ProductShop.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            if (ModelState.IsValid)
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
                TempData["SuccesMessage"] = $"Продукт {viewModelProduct.Name} добавлен!";
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Error", "Home");
           
        }
        [HttpGet]
        public IActionResult UpdateProduct(int? id)
        {
            if (id.HasValue)
            {
                var product = _db.GetProductById(id.Value);
                ViewModelProduct viewModelProduct = new ViewModelProduct()
                {
                    Id = product.Id,
                    Name = product.Name,
                    Category = product.Category,
                    Description = product.Description,
                    Manufacturer = product.Manufacturer,
                    ProductComposition = product.ProductComposition
                };
                return View(viewModelProduct);
            }
            return View(null);           
        }
        [HttpPost]
        public IActionResult UpdateProduct(ViewModelProduct viewModelProduct)
        {
            if (ModelState.IsValid)
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
                TempData["SuccesMessage"] = $"Продукт <{viewModelProduct.Name}> отредактирован!";
                if (resultOperation)
                {
                    return RedirectToAction("Index", "Home");
                }
            }           
            return RedirectToAction("Error","Home");
        }
        [HttpGet]
        public IActionResult DeleteProduct(int? id)
        {
            if (id.HasValue)
            {
                Product searchProdict = _db.GetProductById(id.Value);
                ViewModelProduct viewModelProduct = new ViewModelProduct()
                {
                    Id= searchProdict.Id,
                    Name = searchProdict.Name,
                    Category = searchProdict.Category,
                    Description=searchProdict.Description,
                    Manufacturer = searchProdict.Manufacturer,
                    ProductComposition = searchProdict.ProductComposition                    
                };
                return View(viewModelProduct);
            }
            return RedirectToAction("Error", "Home");
            
        }

        [HttpPost]
        public IActionResult DeleteProduct(int id)
        {
            Product searchProdict = _db.GetProductById(id);
            ViewModelProduct viewModelProduct = new ViewModelProduct()
            {
                Id = searchProdict.Id,
                Name = searchProdict.Name,
                Category = searchProdict.Category,
                Description = searchProdict.Description,
                Manufacturer = searchProdict.Manufacturer,
                ProductComposition = searchProdict.ProductComposition
            };
            string message = viewModelProduct.Name;
            bool result = _db.DeleteProduct(id);              
            _db.Save();
            TempData["SuccesMessage"] = $"Продукт <{message}> удален!";
            if (result)
            {
                return RedirectToAction("Index", "Home");
            }                
            return RedirectToAction("Error", "Home");
        }

      
        public IActionResult GetProductByName(SearchByNameVIewModel search)
        {
            SearchByNameVIewModel model = new SearchByNameVIewModel();
            model.Products = _db.GetProductByName(search.SearchString);
            return View(model);
        }

        [HttpGet]
        [ActionName("SearchByName")]
        public IActionResult GetProductByName()
        {
            return View("GetProductByName");
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
