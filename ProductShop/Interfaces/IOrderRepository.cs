using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductShop.Services
{
    public interface IOrderRepository<T>
    {
        IEnumerable<T> GetOrders(string id);
        IEnumerable<T> GetOrdersByDateOfPurchase(string start, string end);
        T GetOrderForShoppingCart(string id);
        bool UpdateOrder(T t);
        Task<bool> CreateOrder(T v);
        bool DeleteOrder(string id);
        IEnumerable<T> GetOrderByCustomerName(string firstName = "", string middleName = "", string lastName = "");
    }
}
