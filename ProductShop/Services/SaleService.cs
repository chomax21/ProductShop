using Microsoft.AspNetCore.Http;
using ProductShop.Data;
using ProductShop.Interfaces;
using System;

namespace ProductShop.Services
{
    public class SaleService : ISaleService 
    {
        private readonly ApplicationDbContext _db;

        public SaleService(ApplicationDbContext dbContext)
        {
            _db = dbContext;
        }
        public decimal GetDiscount(decimal price, decimal discount) // Здесь просто расчитывается общая цена относительно скидки. Первым аргументом приходит цена, вторым - размер скидки.
        {
            return price * discount;   // Возвращается итоговая цена.
        }

        public decimal HaveDiscountInProduct(int id) // Проверяем наличие скидки у продукта. Если она есть делаем перерасчет, если нет, возвращаем станадртную цену.
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
            return 0;   // Если вернули 0, значит что-то не так. Нужно обработать в вызывающем коде.
        }
    }
}
