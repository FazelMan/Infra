using System.Net.Mail;
using System.Threading.Tasks;
using Core.Shared.Configuration;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;

namespace Core.Shared.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;
        private readonly SmtpConfiguration _smtpConfiguration;
        private readonly SmtpClient _client;

        public EmailSender(ILogger<EmailSender> logger, SmtpConfiguration smtpConfiguration)
        {
            _logger = logger;
            _smtpConfiguration = smtpConfiguration;
            _client = new SmtpClient
            {
                Host = smtpConfiguration.Host,
                Port = smtpConfiguration.Port,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = smtpConfiguration.UseSSL
            };

            if (!string.IsNullOrEmpty(smtpConfiguration.Password))
                _client.Credentials = new System.Net.NetworkCredential(
                    smtpConfiguration.Login,
                    smtpConfiguration.Password);
            else
                _client.UseDefaultCredentials = true;
        }


        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            await Task.Run(() =>
            {
                _logger.LogInformation($"Sending email: {email}, subject: {subject}, message: {htmlMessage}");

                var mail = new MailMessage(_smtpConfiguration.Login, email);
                mail.IsBodyHtml = true;
                mail.Subject = subject;
                mail.Body = htmlMessage;

                _client.Send(mail);

                _logger.LogInformation($"Email: {email}, subject: {subject}, message: {htmlMessage} successfully sent");

                return Task.CompletedTask;
            });
        }
    }
}
