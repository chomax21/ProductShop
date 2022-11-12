# ProductShop магазин - ChoShop. Решил, что будут продаваться кексы :)
### ___Краткое описание :___  
Очередной учебный проект, ASP.NET Core(MVC). IdentityServer со старта. 
Реализован универсальный интернет магазин в рамках своего понимания и пройденного материала. Проект по возможности развивается.

Сайт опубликован по адресу https://choshop.online .

Приложение разрабатывается собственными силами, помощники : YouTube, Metanit, книга Эндрю Люка - ASP.Net Core в действии.

Построенно в монолитном решении, в виде сервисов выступают классы.    
В отдельном решении внедряется и изучается Юнит-тестривание xUnit.

Из паттернов проектирования старался реализовать IReposityPattern. В остальном, пока старался писать просто рабочий код.


### ___Функицонал пользователя :___    
1. Просмотр каталога продуктов.    
2. Регистрация на сайте. Если пароль забыт, его можно восстановить через почту указанную при регистрации.  
3. Поиск продуктов по названию, производителю, категории.
4. Для каждого пользователя создается корзина покупок, где происходит подсчет общей стоимости заказа. Для входа в коризну нужно авторизоваться.    
5. Существует сервис подсчета цены, он зарегистрирован как Transient Service. При каждом обращении производит подсчет итоговоый цены. Если вдруг администратор решил изменить цену, то цена продуктов в корзине покупателя изменится соотвественно.
6. Произвести покупку своего заказа. Покупка является пока фективной, т.е. не принимаются карты и не производятся операции с деньгами. После покупки на почту покупателя отправляется письмо.

### ___Функицонал администратора :___    
1. При первом запуске приложения база данных инициализируется первичными данными пользователя в виде Администратора.     
2. Просмотр каталога.    
3. Редактирование каталога. Т.е. мы можем создавать новые продукты и категории товаров. Так же удаление. Мы можем восстановить удаленные ранее продукты.
4. К функциям поиска добавлена новая категория поиска продуктов по ID.    
5. Просмотр всех зарегестрировавшихся пользователей, и историю их покупок.    
6. Поиск и просмотр заказов по ФИО заказчика, а так же по дате совершения покупки.


## Используемые технологии :
 - ___ASP.Net Core 5.0,___    
 - ___MSSql,___    
 - ___EntityFramework 5.0.___ 
 - ___IdentityServer___
 - ___Boostrap___    
 - ___MailKit___ 
## Цель :
  1. Закрепить и получить знания необходимые для создания приложений используя ASP.NET Core.        
  2. Научиться работать в связке ASP.Net Core & EntityFramework.     
  3. Разобраться хотя-бы с одним из вариантов публикации приложления, и организации к нему доступа через интернет.      
  4. Максимально возможно совершенствовать продукт, реализовывать новые идеи.