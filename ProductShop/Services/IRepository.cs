using System.Collections.Generic;
using System.Linq;

namespace ProductShop.Services
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetProducts();
        T GetProductById(int id);
        IEnumerable<T> GetProductByName(string name);
        T GetProductByCategory(string category);
        T GetProductByManufacturer(string manufacturer);
        bool CreateProduct(T item);
        bool UpateProduct(T item);
        bool DeleteProduct(int? id);
        void Save();
    }
}
