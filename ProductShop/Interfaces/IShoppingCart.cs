namespace ProductShop.Interfaces
{
    public interface IShoppingCart<T>
    {
        public T GetShoppingCart(string id);
        public void DeleteSgoppingCart(string id);
        public bool AddShoppingCartInDb(T t);
        public bool UpdateShoppingCartInDb(T t);


    }
}
