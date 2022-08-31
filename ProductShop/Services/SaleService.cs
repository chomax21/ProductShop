using Microsoft.AspNetCore.Http;
using ProductShop.Data;
using ProductShop.Interfaces;

namespace ProductShop.Services
{
    public class SaleService : ISaleService 
    {
        private readonly ApplicationDbContext _db;

        public SaleService(ApplicationDbContext dbContext)
        {
            _db = dbContext;
        }
        public decimal GetDiscount(decimal price, decimal discount)
        {
            return price * discount;   
        }

        public decimal HaveDiscountInProduct(int id)
        {
            var product = _db.Products.Find(id);
            if (product != null)
            {
                if (product.HaveDiscount)
                {
                    return GetDiscount(product.Price, product.Discount);
                }
                return product.Price;
            }
            return 0;   
        }
    }
}
