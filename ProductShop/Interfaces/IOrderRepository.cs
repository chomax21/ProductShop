using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductShop.Services
{
    public interface IOrderRepository<T>
    {
        Task<IEnumerable<T>> GetOrders(string id);
        Task<T> GetOrderForShoppingCart(string id);
        Task<bool> UpdateOrder(T t);
        Task<bool> CreateOrder(T v);
        Task<bool> DeleteOrder(string id);
    }
}
