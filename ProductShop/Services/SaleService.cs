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

        public decimal GetDiscountInProduct(int id) // Проверяем наличие скидки у продукта. Если она есть делаем перерасчет, если нет, возвращаем стандартную цену.
        {
            var product = _db.Products.Find(id);
            if (product != null)
            {
                if (product.HaveDiscount)
                {
                    return product.Price - Math.Round(product.Price * product.Discount, 2); // Происходит расчет цены исходя из учета скидки, Метод Round() класса Math округляет значения до десятичной дроби.
                }
                return product.Price; // Если скидки на продукт нет, возвращем стандартную цену.                
            }
            return -1;   // Если вернули -1, значит что-то не так. Нужно обработать в вызывающем коде.
        }
    }
}
