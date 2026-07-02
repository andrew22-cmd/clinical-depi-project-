using System.Threading.Tasks;

namespace clinicsystem.Services
{
    /// <summary>
    /// Abstraction over "send an email". Controllers depend on this interface only,
    /// never on SmtpClient or any specific provider — keeps email-sending logic
    /// swappable (Gmail, Outlook, SendGrid relay, etc.) without touching callers.
    /// </summary>
    public interface IEmailSender
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlMessage);
    }
}
