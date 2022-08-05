using ProductShop.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductShop.Services
{
    public interface IProductRepository<T>
    {
        Task<T> GetProductById(int id);
        Task<IEnumerable<T>> GetProducts();
        Task<IEnumerable<T>> GetProductsIsDeleted();
        Task<IEnumerable<T>> GetProductByName(string name);
        Task<IEnumerable<T>> GetProductByCategory(string category);
        Task<IEnumerable<T>> GetProductByManufacturer(string manufacturer);
        Task<bool> CreateProduct(T item);
        Task<bool> UpateProduct(T item);
        Task<bool> DeleteProduct(int? id);
        Task Save();
        Task SetValueInCategoryList(string value);
        Task<IEnumerable<ProductCategory>> GetValuesInCategoryList();
        Task<string> GetOneValueInCategory(int id);
    }
}
