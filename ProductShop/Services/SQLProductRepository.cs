using ProductShop.Data;
using ProductShop.Models;
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
                _db.Remove(id);
                return true;
            }
            return false;
        }

        public Product GetProductByCategory(string category)
        {
            var getProduct = from x in _db.Products
                          where x.Category.Contains(category)
                          select x;
            return (Product)getProduct;
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

        public Product GetProductByName(string name)
        {
            var getProduct = from x in _db.Products
                             where x.Name.Contains(name)
                             select x;
            return (Product)getProduct;
        }

        public IEnumerable<Product> GetProducts()
        {
            return _db.Products.ToList();
        }

        public void Save()
        {
            _db.SaveChangesAsync();
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
