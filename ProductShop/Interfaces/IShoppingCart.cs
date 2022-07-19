using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductShop.Interfaces
{
    public interface IShoppingCart<T>
    {
        public Task<T> GetShoppingCart(string id);
        public Task DeleteSgoppingCart(string id);
        public Task<bool> AddShoppingCartInDb(T t);
        public Task<bool> UpdateShoppingCartInDb(T t);
        public Task<List<T>> GetUserAllShoppingCarts(string id);


    }
}
