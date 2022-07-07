using ProductShop.Data;
using ProductShop.Models;
using ProductShop.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; 

namespace ProductShop.Services
{
    public class SQLProductRepository : IProductRepository<Product>
    {
        private ApplicationDbContext _db;
        public SQLProductRepository(ApplicationDbContext context)
        {
            _db = context;
        }


        public async Task<bool> CreateProduct(Product item)
        {
            if (item != null)
            {
                await _db.Products.AddAsync(item);
                return true;
            }
            return false;
           
        }


        public async Task<bool> DeleteProduct(int? id)
        {
            if (id.HasValue)
            {
                var product = await _db.Products.FindAsync(id.Value);
                
                product.IsDeleted = true;
                return true;
            }
            return false;
        }


        public async Task<IEnumerable<Product>> GetProductByCategory(string category)
        {
            //var getProducts = from x in _db.Products
            //                  where x.Category.Contains(category)
            //                  select x;
            //return getProducts;

            return await Task<IEnumerable<Product>>.Factory.StartNew(() => _db.Products.Where(x => x.Category.Contains(category)));
        }
        
        public async Task<Product> GetProductById(int id)
        {
            Product getProduct = await _db.Products.FindAsync(id);
            return getProduct;
        }

        public async Task<IEnumerable<Product>> GetProductByManufacturer(string manufacturer)
        {
            //var getProduct = from x in _db.Products
            //                 where x.Manufacturer.Contains(manufacturer)
            //                 select x;
            return await Task<IEnumerable<Product>>.Factory.StartNew(() => _db.Products.Where(x => x.Category.Contains(manufacturer)));
        }

        public async Task<IEnumerable<Product>> GetProductByName(string name)
        {
            //var searchProduct = _db.Products
            //    .Where(x => x.Name.Contains(name))
            //    .Select(x => x);

            return await Task<IEnumerable<Product>>.Factory.StartNew(() => _db.Products.Where(x => x.Category.Contains(name)));
        }

        public async Task<IEnumerable<Product>> GetProducts() // Отобразить все продукты, кроме тех которые помеченны как удаленные.
        {
            //var searchResult = from x in _db.Products
            //                   where !x.IsDeleted
            //                   select x;
            return await Task<IEnumerable<Product>>.Factory.StartNew(() => _db.Products.Where(x => x.IsDeleted == false));
        }

        public async Task<IEnumerable<Product>> GetProductsIsDeleted() // Отобразить все продукты, как имеющиеся так и удаленные.
        {
            //var searchResult = from x in _db.Products                              
            //                   select x;
            return await Task<IEnumerable<Product>>.Factory.StartNew(() => _db.Products.Select(x=>x));
        }

        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }

        public async Task<bool> UpateProduct(Product item)
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
