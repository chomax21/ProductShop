using ProductShop.Data;
using ProductShop.Models;
using ProductShop.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProductShop.Services
{
    public class SQLProductRepository : IRepository<Product>
    {
        private ApplicationDbContext _db;
        public SQLProductRepository(ApplicationDbContext context)
        {
            _db = context;
        }
        public bool CreateProduct(Product item)
        {
            if (item != null)
            {
                _db.Add(item);
                return true;
            }
            return false;
           
        }

        public bool DeleteProduct(int? id)
        {
            if (id.HasValue)
            {
                var product = _db.Products.Find(id.Value);
                product.IsDeleted = true;
                return true;
            }
            return false;
        }

        public IEnumerable<Product> GetProductByCategory(string category)
        {
            var getProducts = from x in _db.Products
                          where x.Category.Contains(category)
                          select x;
            return getProducts;
        }

        public Product GetProductById(int id)
        {
            Product getProduct = _db.Products.Find(id);
            return getProduct;
        }

        public Product GetProductByManufacturer(string manufacturer)
        {
            var getProduct = from x in _db.Products
                             where x.Manufacturer.Contains(manufacturer)
                             select x;
            return (Product)getProduct;
        }

        public IEnumerable<Product> GetProductByName(string name)
        {
            var searchProduct = _db.Products
                .Where(x => x.Name.Contains(name))
                .Select(x => x);
                
            return searchProduct;
        }

        public IEnumerable<Product> GetProducts()
        {
            var searchResult = from x in _db.Products
                               where !x.IsDeleted
                               select x;
            return searchResult.ToList();
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public bool UpateProduct(Product item)
        {
            if (item != null)
            {
                _db.Products.Update(item);
                _db.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                return true;
            }
            return false;
        }

    }
}
