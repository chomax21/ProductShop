namespace ProductShop.Interfaces
{
    public interface IShoppingCart<T>
    {
        public T GetShoppingCart(string id);
        public void DeleteSgoppingCart(string id);
        
    }
}
