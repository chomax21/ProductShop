using System.Collections.Generic;

namespace ProductShop.Models
{
    public class ShopingCart
    {
        public ShopingCart() { }
        public ShopingCart(string NewUserId)
        {
            UserId = NewUserId;
        }
        public int Id { get; set; }
        public int ProductId { get; set; } // Используется для передачи Id продукта, с дальнейшим поиском этого продукта и добавления его в коризну.
        public string UserId { get; set; } // Привязка корзины к конкретному Юзеру по ID.
        public Order Order { get; set; } // В корзине содержится конкретный Заказ(Order) с вложенными в него продуктами(Products).
        public bool IsDone { get; set; } // Если заказ выполнен, товар куплен и отправлен помечаем как "Готово" и удаляем корзину. Сам заказ сохраняем в БД.
    }
}
