using ProductShop.Data;
using ProductShop.Models;
using ProductShop.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<string> GetOneValueInCategory(int id)
        {
            var category = await _db.ProductCategories.FindAsync(id);
            var result = category?.Category ?? "---";
            return result;
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
            return await Task<IEnumerable<Product>>.Factory.StartNew(() => _db.Products.Where(x => x.Manufacturer.Contains(manufacturer)));

        }

        public async Task<IEnumerable<Product>> GetProductByName(string name)
        {
            //var searchProduct = _db.Products
            //    .Where(x => x.Name.Contains(name))
            //    .Select(x => x);

            return await Task<IEnumerable<Product>>.Factory.StartNew(() => _db.Products.Where(x => x.Name.Contains(name)));
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
            return await Task<IEnumerable<Product>>.Factory.StartNew(() => _db.Products.Select(x => x));
        }

        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }

        public async Task<bool> UpateProduct(Product item)
        {
            if (item != null)
            {
                var product = await _db.Products.FindAsync(item.Id);

                if (product != null)
                {
                    product.Name = item.Name;
                    product.Price = item.Price;
                    product.Count = item.Count;
                    product.IsDeleted = item.IsDeleted;
                    product.Description = item.Description;
                    product.Category = item.Category;
                    product.Manufacturer = item.Manufacturer;
                    product.ProductComposition = item.ProductComposition;
                    product.Discount = item.Discount;
                    product.HaveDiscount = item.HaveDiscount;
                    product.PhotoPath = item.PhotoPath;
                    _db.Products.Update(product);
                    //await _db.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            return false;
        }

        public async Task SetValueInCategoryList(string value)
        {
            ProductCategory product = new();
            product.Category = value;
            await _db.ProductCategories.AddAsync(product);
        }


        public async Task<IEnumerable<ProductCategory>> GetValuesInCategoryList()
        {
            return await Task.Run(() => _db.ProductCategories.Select(x => x));
        }

        public async Task<bool> DeleteValuesInCategoryList(string value)
        {
            var product = _db.ProductCategories.Single(x => x.Category == value);
            if (product != null)
            {
                await Task.Run(() => _db.ProductCategories.Remove(product));
                return true;
            }
            return false;
            
        }
    }
}
