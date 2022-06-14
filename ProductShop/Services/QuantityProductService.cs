using ProductShop.Data;
using ProductShop.Interfaces;
using ProductShop.Models;
using System.Linq;

namespace ProductShop.Services
{
    public class QuantityProductService : IQuantityProduct<ProductQuantityInCart>
    {
        private readonly ApplicationDbContext _db;

        public QuantityProductService(ApplicationDbContext context)
        {
            _db = context;
        }
        public int GetQuantity(int productId, int ShopingCartId)
        {
            var result = _db.QuantityInCarts.SingleOrDefault(quant => quant.ProductId == productId && quant.ShoppingCartId == ShopingCartId);
            if (result == null)
            {
                ProductQuantityInCart productQuantity = new ProductQuantityInCart();
                productQuantity.ProductId = productId;
                productQuantity.ShoppingCartId = ShopingCartId;
                return productQuantity.Quantity;
            }
            return result.Quantity;
        }

        public ProductQuantityInCart SetQuantity(int productId, int ShopingCartId, int Quantity)
        {
            var result = _db.QuantityInCarts.SingleOrDefault(quant => quant.ProductId == productId && quant.ShoppingCartId == ShopingCartId);
            if (result!=null)
            {
                result.Quantity = Quantity;
                _db.QuantityInCarts.Update(result);
                _db.Entry(result).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                return result;
            }
            else
            {
                return null;
            }
        }
    }
}
