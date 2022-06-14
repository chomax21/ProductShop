namespace ProductShop.Models
{
    public class ProductQuantityInCart
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ShoppingCartId { get; set; }
        public int Quantity { get; set; }
    }
}
