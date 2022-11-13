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
                TempData["SuccesMessage"] = ("Продукт {0} добавлен!", viewModelProduct.Name);
                return RedirectToAction("Index", "Home");
            }          
            return RedirectToAction("Error", "Home");
        }

        private async Task<string> UploaderPhotoFile(IFormFile file) // Метод отвечающий за сохранений изображений.
        {
            string uniqueFileName = null;

            if (file != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvirement.WebRootPath, "images"); // Определяем дирректорию/папку места нахождения изображений.
                uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName; // Генерируем уникальное имя файла.
                string filePath = Path.Combine(uploadsFolder, uniqueFileName); // Собираем все вместе, и опрделеяем полный путь к файлу.

                using (var fs = new FileStream(filePath, FileMode.Create)) // Используем using для последующего закрытия FileStream.
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
                // Проверяем указан ли путь для файла. Если да, значит мы его хотим обновить. Создаем путь на основании пришедших данных и местоположения приложения, удаляем файл.
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
            var searchResult  = await _db.GetProductByName(search.SearchString);
            if (searchResult != null)
            {
                foreach (var originProduct in searchResult)
                {
                    model.Products.Add(MapProductToViewModel(originProduct));
                }
            }
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
            SearchVIewModel model = new();
            var searchResult = await _db.GetProductByCategory(category.SearchString);
            if (searchResult != null)
            {
                foreach (var originProduct in searchResult)
                {
                    model.Products.Add(MapProductToViewModel(originProduct));
                }
            }           
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {

            SearchVIewModel model = new();
            var newProducts = await _db.GetProducts();
            if (newProducts != null)
            {
                foreach (var item in newProducts)
                {
                    model.Products.Add(MapProductToViewModel(item));
                }
            }
            return View(model);
        }

        //[HttpGet]
        //public async Task<IActionResult> GetAllProductsInCategory(string category)
        //{

        //    SearchVIewModel model = new();
        //    var newProducts = await _db.GetProductByCategory(category);
        //    if (newProducts != null)
        //    {
        //        foreach (var item in newProducts)
        //        {
        //            model.Products.Add(MapProductToViewModel(item));
        //        }
        //    }
        //    var categories = _db.GetValuesInCategoryList();
        //    ViewBag.Category = categories.Result;
        //    return View("GetAllProducts",model);
        //}

        [HttpGet]
        [Authorize("AdminRights")]
        public async Task<IActionResult> GetAllProductsIsDeleted()
        {
            SearchVIewModel model = new();
            var newProducts = await _db.GetProductsIsDeleted();
            if (newProducts != null)
            {
                foreach (var item in newProducts)
                {
                    model.Products.Add(MapProductToViewModel(item));
                }
            }           
            return View("GetAllProducts", model);
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
            var searchResult = await _db.GetProductByManufacturer(manufacturer.SearchString);
            if (searchResult != null)
            {
                foreach (var originProduct in searchResult)
                {
                    model.Products.Add(MapProductToViewModel(originProduct));
                }
            }           
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
            ProductCategoryViewModel categoryViewModel = new();
            var category = await _db.GetValuesInCategoryList();
            if(category == null)
            {
                TempData["Error"] = "Ошибка отсутсвуют какие-либо категории!";
                return View("SetValueInCategory", categoryViewModel);
            }
            categoryViewModel.productCategories = category.ToList();

            if (!string.IsNullOrEmpty(setValue))
            {
                await _db.SetValueInCategoryList(setValue);
                await _db.Save();
                return RedirectToAction("GetValuesInCategory", "Product");
            }
            TempData["Error"] = "Нужно ввести категорию продуктов!";
            return RedirectToAction("GetValuesInCategory", "Product");
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
