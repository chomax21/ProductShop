using System;
using System.Globalization;
using System.IO;
using System.Net;
using Yandex.Checkout.V3;

namespace ProductShop.Services
{
    public class PayService
    {
        static Client client;
        public string Pay(decimal amount)
        {
            client = new Client(
            shopId: "3AFFB3DC836858E10097DCBAD794AFABFE1CF6810A83718BCD21BF5B3C938213",
            secretKey: "19B85C743A3B3C2AD0754F2885D8EB92C871A24D71420B7E244FA4882AA12C3EEAAD7A6DB588EEB1B309B1D2FC68994AE3B8E1A6A416AAC1E7380478A21E061F");

            NewPayment newPayment = new NewPayment
            {
                Amount = new Amount { Value = amount, Currency = "RUB" },
                Confirmation = new Confirmation
                {
                    Type = ConfirmationType.Redirect,
                    ReturnUrl = "http://choshop.online",                    
                }
            };
            Payment payment = client.CreatePayment(newPayment);

            string url = payment.Confirmation.ConfirmationUrl;
            return url;
        }

        public void GetMessage()
        {
            //Message message = Client.ParseMessage(requestCo,)
        }
       
    }
}
