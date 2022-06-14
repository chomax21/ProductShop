using ProductShop.Models;

namespace ProductShop.Interfaces
{
    public interface IQuantityProduct<T>
    {
        int GetQuantity(int productId, int shopingCartId);
        T SetQuantity(int productId, int shopingCartId, int quantity);
    }
}
