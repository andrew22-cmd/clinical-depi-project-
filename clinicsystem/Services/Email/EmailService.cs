using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace clinicsystem.Services.Email
{
    public class EmailService : IEmailSender
    {
        private readonly EmailSettings _settings;

        public EmailService(
            IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendEmailAsync(
            string toEmail,
            string subject,
            string htmlMessage)
        {
            using var message = new MailMessage();

            message.From = new MailAddress(
                _settings.SenderEmail,
                _settings.SenderName);

            message.To.Add(toEmail);

            message.Subject = subject;

            message.Body = htmlMessage;

            message.IsBodyHtml = true;

            using var client = new SmtpClient(
                _settings.SmtpServer,
                _settings.SmtpPort);

            client.Credentials =
                new NetworkCredential(
                    _settings.Username,
                    _settings.Password);

            client.EnableSsl =
                _settings.EnableSsl;
            Console.WriteLine($"Username = {_settings.Username}");
            Console.WriteLine($"Password = {_settings.Password}");
            Console.WriteLine($"SMTP = {_settings.SmtpServer}:{_settings.SmtpPort}");

            await client.SendMailAsync(message);
        }
    }
}