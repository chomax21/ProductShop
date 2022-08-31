using Microsoft.AspNetCore.Http;
using ProductShop.Interfaces;

namespace ProductShop.Services
{
    public class SaleService : ISaleService 
    {
        public decimal GetDiscount(decimal price, decimal discount)
        {
            return price * discount;   
        }
    }
}
