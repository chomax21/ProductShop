using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProductShop.Models;
using ProductShop.Services;
using ProductShop.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly SQLProductRepository _db;

        public ProductController(ILogger<ProductController> logger, IProductRepository<Product> repository)
        {
            _logger = logger;
            _db = (SQLProductRepository)repository;
        }


        [HttpGet]
        [Authorize("AdminRights")]
        public IActionResult CreateProduct()
        {
            return View();            
        }



        [HttpPost]
        [Authorize("AdminRights")]
        public IActionResult CreateProduct(ViewModelProduct viewModelProduct)
        {
            if (ModelState.IsValid)
            {
                Product product = new Product()
                {
                    Id = viewModelProduct.Id,
                    Name = viewModelProduct.Name,
                    Price = viewModelProduct.Price,
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
        [Authorize("AdminRights")]
        public IActionResult UpdateProduct(int? id)
        {
            if (id.HasValue)
            {
                var product = _db.GetProductById(id.Value);               
                return View(MapProductToViewModel(product));
            }
            return View(null);           
        }


        [HttpPost]
        [Authorize("AdminRights")]
        public IActionResult UpdateProduct(ViewModelProduct viewModelProduct)
        {
            if (ModelState.IsValid)
            {
                Product product = new Product()
                {
                    Id = viewModelProduct.Id,
                    Name = viewModelProduct.Name,
                    Price= viewModelProduct.Price,
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
        [Authorize("AdminRights")]
        public IActionResult DeleteProduct(int? id)
        {
            if (id.HasValue)
            {
                Product searchProdict = _db.GetProductById(id.Value);
                
                return View(MapProductToViewModel(searchProdict));
            }
            return RedirectToAction("Error", "Home");
            
        }


        [HttpPost]
        [Authorize("AdminRights")]
        public IActionResult DeleteProduct(int id)
        {
            Product searchProdict = _db.GetProductById(id);            
            string message = MapProductToViewModel(searchProdict).Name;
            bool result = _db.DeleteProduct(id);              
            _db.Save();
            TempData["SuccesMessage"] = $"Продукт <{message}> удален!";
            if (result)
            {
                return RedirectToAction("Index", "Home");
            }                
            return RedirectToAction("Error", "Home");
        }


        [Authorize("AdminRights")]
        public IActionResult GetProductByName(SearchVIewModel search)
        {
            SearchVIewModel model = new SearchVIewModel();
            model.Products = _db.GetProductByName(search.SearchString);
            return View(model);
        }

        [HttpGet]
        [ActionName("SearchByName")]
        public IActionResult GetProductByName()
        {
            return View("GetProductByName");
        }

        [Authorize("AdminRights")]
        public IActionResult GetProductById(int id) // Если вдруг такого продукта не окажется, будем пеернаправлять на главную страницу с сообщением об отсуствии данного продукта.
        {
            var resultById = _db.GetProductById(id);
            if (resultById != null)
            {              
                return View(MapProductToViewModel(resultById));
            }
            TempData["Error"] = "Продутка с таким идентификатором не существует!";
            return RedirectToAction("Index","Home");             
        }
        [HttpGet]
        [Authorize("AdminRights")]
        public IActionResult GetProductById()
        {
            return View();
        }


        [HttpGet]
        public IActionResult GetProductByCategory()
        {
            return View();
        }
        
        public IActionResult GetProductByCategory(SearchVIewModel category)
        {
            SearchVIewModel model = new SearchVIewModel();
            model.Products = _db.GetProductByCategory(category.SearchString);
            return View(model);
        }

        [HttpGet]
        public IActionResult GetAllProducts()
        {
            List<ViewModelProduct> viewProducts = new List<ViewModelProduct>();
            var newProducts = _db.GetProducts().ToList();
            foreach (var item in newProducts)
            {
                ViewModelProduct viewModel = new ViewModelProduct();
                viewModel.Id = item.Id;
                viewModel.Name = item.Name;
                viewModel.Price = item.Price;
                viewModel.Category = item.Category;
                viewModel.Description = item.Description;
                viewModel.Manufacturer = item.Manufacturer;
                viewModel.ProductComposition = item.ProductComposition;
                viewModel.IsDeleted = item.IsDeleted;
                viewProducts.Add(viewModel);
            }
            return View(viewProducts); 
        }

        [HttpGet]
        [Authorize("AdminRights")]
        public IActionResult GetAllProductsIsDeleted()
        {
            List<ViewModelProduct> viewProducts = new List<ViewModelProduct>();
            var newProducts = _db.GetProductsIsDeleted().ToList();
            foreach (var item in newProducts)
            {
                ViewModelProduct viewModel = new ViewModelProduct();
                viewModel.Id = item.Id;
                viewModel.Name = item.Name;
                viewModel.Price= item.Price;
                viewModel.Category = item.Category;
                viewModel.Description = item.Description;
                viewModel.Manufacturer = item.Manufacturer;
                viewModel.ProductComposition = item.ProductComposition;
                viewModel.IsDeleted= item.IsDeleted;
                viewProducts.Add(viewModel);
            }
            return View("GetAllProducts", viewProducts);
        }

        [HttpGet]
        public IActionResult GetProductByManufacturer()
        {
            return  View();
        }

        public IActionResult GetProductByManufacturer(SearchVIewModel manufacturer)
        {
            SearchVIewModel model = new SearchVIewModel();
            model.Products = _db.GetProductByManufacturer(manufacturer.SearchString);
            return View(model);
        }

        [Authorize("AdminRights")]
        public IActionResult RestoreProduct(int id)
        {
            var restoreProduct = _db.GetProductById(id);
            restoreProduct.IsDeleted = false;
            _db.Save();
            return RedirectToAction("GetAllProducts","Product");
        }

        private ViewModelProduct MapProductToViewModel(Product product) // Преобразуем класс Product в ViewModelProduct
        {
            ViewModelProduct model = new ViewModelProduct()
            {
                Id = product.Id,
                Name = product.Name,
                Category = product.Category,
                Description = product.Description,
                Manufacturer = product.Manufacturer,
                ProductComposition = product.ProductComposition,
                IsDeleted = product.IsDeleted
            };

            return model;
        }
    }
}
