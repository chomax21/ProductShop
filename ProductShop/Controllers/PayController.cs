using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Yandex.Checkout.V3;

namespace ProductShop.Controllers
{
    //public class PayController : Controller
    //{
    //    static readonly Client _client;

    //    public IActionResult Pay()
    //    {
    //        decimal amount = decimal.Parse("100,00", CultureInfo.InvariantCulture.NumberFormat);
    //        var newPayment = new NewPayment
    //        {
    //            Amount = new Amount { Value = amount, Currency = "RUB" },
    //            Confirmation = new Confirmation { Type = ConfirmationType.Redirect }
    //        };
    //        Payment payment = _client.GetPayment();
    //    }
    //}
}
