﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
                var category = await GetValueInCategory(System.Convert.ToInt32(viewModelProduct.Category));
                await _db.CreateProduct(MapViewModelToProduct(viewModelProduct));
                await _db.Save();
                TempData["SuccesMessage"] = $"Продукт {viewModelProduct.Name} добавлен!";
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Error", "Home");
        }

        private async Task<string> GetValueInCategory(int id)
        {
            return await _db.GetOneValueInCategory(id);
        }


        [HttpGet]
        [Authorize("AdminRights")]
        public async Task<IActionResult> UpdateProduct(int? id)
        {
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
                //var category = await GetValueInCategory(System.Convert.ToInt32(viewModelProduct.Category));
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
            string message = MapProductToViewModel(searchProdict).Name;
            bool result = await _db.DeleteProduct(id);
            await _db.Save();
            TempData["SuccesMessage"] = $"Продукт <{message}> удален!";
            if (result)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Error", "Home");
        }


        [Authorize("AdminRights")]
        public async Task<IActionResult> GetProductByName(SearchVIewModel search)
        {
            SearchVIewModel model = new SearchVIewModel();
            model.Products = await _db.GetProductByName(search.SearchString);
            return View(model);
        }

        [HttpGet]
        [ActionName("SearchByName")]
        public IActionResult GetProductByName()
        {
            return View("GetProductByName");
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
            var category = _db.GetOneValueInCategory(product.Id);
            product.Category = category.Result;

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
                HaveDiscount = product.HaveDiscount                
            };

            return model;
        }

        private Product MapViewModelToProduct(ProductViewModel viewModelProduct) // Преобразуем класс ViewModelProduct в Product
        {
            
            Product model = new Product()
            {
                Id = viewModelProduct.Id,
                Name = viewModelProduct.Name,
                Category = viewModelProduct.Category,
                Description = viewModelProduct.Description,
                Manufacturer = viewModelProduct.Manufacturer,
                ProductComposition = viewModelProduct.ProductComposition,
                IsDeleted = viewModelProduct.IsDeleted,
                Price = viewModelProduct.Price,
                Count = viewModelProduct.Count,
                Discount = viewModelProduct.Discount,
                HaveDiscount = viewModelProduct.HaveDiscount
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
    }
}
