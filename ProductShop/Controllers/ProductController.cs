using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using ProductShop.Models;
using ProductShop.Services;
using ProductShop.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProductShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductRepository<Product> _db;
        private readonly IWebHostEnvironment _webHostEnvirement;

        public ProductController(ILogger<ProductController> logger, IProductRepository<Product> repository, IWebHostEnvironment webHostEnvirement)
        {
            _logger = logger;
            _db = repository;
            _webHostEnvirement = webHostEnvirement;
        }


        [HttpGet]
        [Authorize("AdminRights")]
        public async Task<IActionResult> CreateProduct()
        {
            ProductCategory product = new ProductCategory();
            var category = await _db.GetValuesInCategoryList();
            if (category != null)
            {
                ViewBag.Category = new SelectList(category, "Id", "Category", product.Category);
            }
            return View();
        }



        [HttpPost]
        [Authorize("AdminRights")]
        public async Task<IActionResult> CreateProduct(ProductViewModel viewModelProduct)
        {            
            if (ModelState.IsValid)
            {
                if (viewModelProduct.Photo != null)
                {
                    viewModelProduct.PhotoPath = UploaderPhotoFile(viewModelProduct.Photo).Result;
                }
                await _db.CreateProduct(MapViewModelToProduct(viewModelProduct));
                await _db.Save();
                TempData["SuccesMessage"] = $"Продукт {viewModelProduct.Name} добавлен!";
                return RedirectToAction("Index", "Home");
            }          
            return RedirectToAction("Error", "Home");
        }

        private async Task<string> UploaderPhotoFile(IFormFile file)
        {
            string uniqueFileName = null;

            if (file != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvirement.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fs);
                }
                return uniqueFileName;
            } 
            return null;
        }

        private async Task<string> GetValueInCategory(int id)
        {
            return await _db.GetOneValueInCategory(id);
        }


        [HttpGet]
        [Authorize("AdminRights")]
        public async Task<IActionResult> UpdateProduct(int? id)
        {
            ProductCategory productCategory = new ProductCategory();
            var category = await _db.GetValuesInCategoryList();
            if (category != null)
            {
                ViewBag.Category = new SelectList(category, "Id", "Category", productCategory.Category);
            }

            if (id.HasValue)
            {
                var product = await _db.GetProductById(id.Value);
                return View(MapProductToViewModel(product));
            }
            return View(null);
        } 


        [HttpPost]
        [Authorize("AdminRights")]
        public async Task<IActionResult> UpdateProduct(ProductViewModel viewModelProduct)
        {
            if (ModelState.IsValid)
            {
                if (viewModelProduct.PhotoPath != null)
                {
                    string filePath = Path.Combine(_webHostEnvirement.WebRootPath, "images", viewModelProduct.PhotoPath);
                    System.IO.File.Delete(filePath);
                }
                if (viewModelProduct.Photo != null)
                {
                    var path = UploaderPhotoFile(viewModelProduct.Photo).Result;
                    viewModelProduct.PhotoPath = path;
                }

                bool resultOperation = await _db.UpateProduct(MapViewModelToProduct(viewModelProduct));
                await _db.Save();
                TempData["SuccesMessage"] = $"Продукт <{viewModelProduct.Name}> отредактирован!";
                if (resultOperation)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Error", "Home");
        }


        [HttpGet]
        [Authorize("AdminRights")]
        public async Task<IActionResult> DeleteProduct(int? id)
        {
            if (id.HasValue)
            {
                Product searchProdict = await _db.GetProductById(id.Value);

                return View(MapProductToViewModel(searchProdict));
            }
            return RedirectToAction("Error", "Home");

        }


        [HttpPost]
        [Authorize("AdminRights")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            Product searchProdict = await _db.GetProductById(id);
            string message = searchProdict.Name;
            bool result = await _db.DeleteProduct(id);
            await _db.Save();
            TempData["SuccesMessage"] = $"Продукт <{message}> удален!";
            if (result)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Error", "Home");
        }

        [HttpGet] 
        public IActionResult GetProductByName()
        {
            return View();
        }
  
        [HttpPost]
        public async Task<IActionResult> GetProductByName(SearchVIewModel search)
        {
            SearchVIewModel model = new SearchVIewModel();
            model.Products = await _db.GetProductByName(search.SearchString);
            return View(model);
        }

        [HttpGet]
        [Authorize("AdminRights")]
        public IActionResult GetProductById()
        {
            return View();
        }

        [Authorize("AdminRights")]
        public async Task<IActionResult> GetProductById(int id) // Если вдруг такого продукта не окажется, будем пеернаправлять на главную страницу с сообщением об отсуствии данного продукта.
        {
            var resultById = await _db.GetProductById(id);
            if (resultById != null)
            {
                return View(MapProductToViewModel(resultById));
            }
            TempData["Error"] = "Продукта с таким идентификатором не существует!";
            return RedirectToAction("Index", "Home");
        }
      

        [HttpGet]
        public IActionResult GetProductByCategory()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetProductByCategory(SearchVIewModel category)
        {
            SearchVIewModel model = new SearchVIewModel();
            model.Products = await _db.GetProductByCategory(category.SearchString);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {

            List<ProductViewModel> viewProducts = new List<ProductViewModel>();
            var newProducts = await _db.GetProducts();
            foreach (var item in newProducts)
            {               
                viewProducts.Add(MapProductToViewModel(item));
            }
            return View(viewProducts);
        }

        [HttpGet]
        [Authorize("AdminRights")]
        public async Task<IActionResult> GetAllProductsIsDeleted()
        {
            List<ProductViewModel> viewProducts = new List<ProductViewModel>();
            var newProducts = await _db.GetProductsIsDeleted();
            foreach (var item in newProducts)
            {                
                viewProducts.Add(MapProductToViewModel(item));
            }
            return View("GetAllProducts", viewProducts);
        }

        [HttpGet]
        public IActionResult GetProductByManufacturer()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetProductByManufacturer(SearchVIewModel manufacturer)
        {
            SearchVIewModel model = new SearchVIewModel();
            model.Products = await _db.GetProductByManufacturer(manufacturer.SearchString);
            return View(model);
        }

        [Authorize("AdminRights")]
        public async Task<IActionResult> RestoreProduct(int id)
        {
            var restoreProduct = await _db.GetProductById(id);
            restoreProduct.IsDeleted = false;
            await _db.Save();
            return RedirectToAction("GetAllProducts", "Product");
        }

        private ProductViewModel MapProductToViewModel(Product product) // Преобразуем класс Product в ViewModelProduct
        {                      
            IFormatProvider format = CultureInfo.GetCultureInfo("en-Us");

            ProductViewModel model = new ProductViewModel()
            {
                Id = product.Id,
                Name = product.Name,
                Category = product.Category,
                Description = product.Description,
                Manufacturer = product.Manufacturer,
                ProductComposition = product.ProductComposition,
                IsDeleted = product.IsDeleted,
                Price = product.Price,
                Count = product.Count,
                Discount = product.Discount,
                HaveDiscount = product.HaveDiscount,
                stringPrice = product.Price.ToString(format),
                stringDiscount = (product.Discount * 100).ToString(format),
                PhotoPath = product.PhotoPath

            };

            return model;
        }

        private Product MapViewModelToProduct(ProductViewModel viewModelProduct) // Преобразуем класс ViewModelProduct в Product
        {
            CultureInfo culture = new CultureInfo("en-US"); // Англ локализация, для использования точки в паарметрах цен и скидок.
            var resulParsePrice = Convert.ToDecimal(viewModelProduct.stringPrice, culture); // Парсим из строки значение цены в дробное число, Decimal.
            var resultParseDiscount = Convert.ToDecimal(viewModelProduct.stringDiscount, culture); // Парсим из строки значение скидки в дробное число, Decimal.

            var category = _db.GetOneValueInCategory(Convert.ToInt32(viewModelProduct.Category));

            Product model = new Product()
            {
                Id = viewModelProduct.Id,
                Name = viewModelProduct.Name,
                Category = category.Result,
                Description = viewModelProduct.Description,
                Manufacturer = viewModelProduct.Manufacturer,
                ProductComposition = viewModelProduct.ProductComposition,
                IsDeleted = viewModelProduct.IsDeleted,
                Price = resulParsePrice,
                Count = viewModelProduct.Count,
                Discount = resultParseDiscount,
                HaveDiscount = viewModelProduct.HaveDiscount,
                PhotoPath = viewModelProduct.PhotoPath
            };

            return model;
        }

        [HttpPost]
        [Authorize("AdminRights")]
        public async Task<IActionResult> SetValueInCategory(string setValue)
        {
            if (!string.IsNullOrEmpty(setValue))
            {
                await _db.SetValueInCategoryList(setValue);
                await _db.Save();
                ProductCategoryViewModel categoryViewModel = new();
                var category = await _db.GetValuesInCategoryList();
                categoryViewModel.productCategories = category.ToList();
                return View("SetValueInCategory", categoryViewModel);
            }
            return View("Error","Home");
        }

        [HttpGet]
        [Authorize("AdminRights")]
        public async Task<IActionResult> GetValuesInCategory()
        {
            ProductCategoryViewModel categoryViewModel = new();
            var category = await _db.GetValuesInCategoryList();
            categoryViewModel.productCategories = category.ToList();
            if (categoryViewModel != null)
            {
                return View("SetValueInCategory", categoryViewModel);
            }
            return View("SetValueInCategory", Enumerable.Empty<ProductCategory>());
        }
        
        [Authorize("AdminRights")]
        public async Task<IActionResult> DeleteProductCategory(string categoryName)
        {
            ProductCategoryViewModel categoryViewModel = new();
            var result = await _db.DeleteValuesInCategoryList(categoryName);
            await _db.Save();
            var category = await _db.GetValuesInCategoryList();
            categoryViewModel.productCategories = category.ToList();
            if (result)
            {
                return View("SetValueInCategory", categoryViewModel);
            }
            TempData["Error"] = $"Категория <<<{categoryName}>>> не существует!";
            return RedirectToAction("Index", "Home");
        }
    }
}
