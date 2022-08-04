using ProductShop.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductShop.Services
{
    public interface IOrderRepository<T>
    {
        IEnumerable<T> GetOrders(string id);
        T GetOrderForShoppingCart(string id);
        bool UpdateOrder(T t);
        Task<bool> CreateOrder(T v);
        bool DeleteOrder(string id);
        UserInfoViewModel GetOrdersByDateOfPurchase(string start, string end);
        UserInfoViewModel GetOrderByCustomerName(string firstName, string middleName, string lastName);
    }
}
