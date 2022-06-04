using System.Collections.Generic;

namespace ProductShop.Services
{
    public interface IOrderRepository<T>
    {
        IEnumerable<T> GetOrders(string id);
        T GetOrderForShoppingCart(string id);
        bool CreateOrder(T v);
        bool DeleteOrder(string id);
    }
}
