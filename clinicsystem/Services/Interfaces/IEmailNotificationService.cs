using clinicsystem.Models;

namespace clinicsystem.Services.Interfaces
{
    public interface IEmailNotificationService
    {
        Task<IEnumerable<EmailNotification>> GetAllAsync();

        Task<EmailNotification?> GetByIdAsync(int id);

        Task<IEnumerable<EmailNotification>> GetByReservationAsync(int reservationId);

        Task<IEnumerable<EmailNotification>> GetByUserAsync(int userId);

        Task<IEnumerable<EmailNotification>> GetFailedEmailsAsync();

        Task AddNotificationAsync(EmailNotification notification);

        Task UpdateStatusAsync(int notificationId, string status);

        Task<bool> DeleteNotificationAsync(int id);
    }
}