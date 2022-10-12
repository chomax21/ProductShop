using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using MimeKit;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ProductShop.Services
{
    public class EmailService : IEmailSender
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }


        public async Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                                                
                var emailMessage = new MimeMessage();

                emailMessage.From.Add(new MailboxAddress("Максимка","songmax21@mail.ru"));
                emailMessage.To.Add(new MailboxAddress(email, email));
                emailMessage.Subject = subject;
                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = message
                };

                using (MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient())
                {                    
                    await client.ConnectAsync("smtp.mail.ru", 465, true); //либо использум порт 465
                    await client.AuthenticateAsync("songmax21@mail.ru", "Q8T4txjekw2rx2eqMDMd"); //логин-пароль от аккаунта
                    await client.SendAsync(emailMessage);

                    await client.DisconnectAsync(true);
                    _logger.LogInformation("Сообщение отправлено успешно!");
                }                
            }
            catch (System.Exception ex)
            {
                _logger.LogError("Ошибка в отправлении Email", ex);
            }           
            
        }
    }
}
