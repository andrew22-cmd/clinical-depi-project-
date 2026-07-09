using clinicsystem.Models;

namespace clinicsystem.Repositories.Interfaces
{
    public interface IEmailNotificationRepository : IRepository<EmailNotification>
    {
        Task<IEnumerable<EmailNotification>> GetByReservationAsync(int reservationId);

        Task<IEnumerable<EmailNotification>> GetByUserAsync(int userId);

        Task<IEnumerable<EmailNotification>> GetFailedEmailsAsync();

        Task UpdateStatusAsync(int notificationId, string status);
    }
}