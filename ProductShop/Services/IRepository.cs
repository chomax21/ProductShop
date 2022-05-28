﻿using System.Collections.Generic;
using System.Linq;

namespace ProductShop.Services
{
    public interface IRepository<T,V>
    {
        T GetProductById(int id);
        IEnumerable<T> GetProducts();
        IEnumerable<T> GetProductsIsDeleted();
        IEnumerable<T> GetProductByName(string name);
        IEnumerable<T> GetProductByCategory(string category);
        IEnumerable<T> GetProductByManufacturer(string manufacturer);
        bool CreateProduct(T item);
        bool UpateProduct(T item);
        bool DeleteProduct(int? id);
        void Save();
        IEnumerable<V> GetOrders(string id);
        bool CreateOrder(string id);
        bool DeleteOrder(string id);
    }
}
