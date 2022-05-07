using System.Collections.Generic;

namespace ProductShop.Services
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetProducts();
        T GetProductById(int id);
        T GetProductByName(string name);
        T GetProductByCategory(string category);
        T GetProductByManufacturer(string manufacturer);
        bool CreateProduct(T item);
        bool UpateProduct(T item);
        bool DeleteProduct(int? id);
        void Save();
    }
}
