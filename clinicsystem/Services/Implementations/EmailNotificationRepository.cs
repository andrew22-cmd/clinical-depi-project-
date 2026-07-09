using clinicsystem.Models;
using clinicsystem.Repositories.Interfaces;
using clinicsystem.Services.Interfaces;

namespace clinicsystem.Services.Implementations
{
    public class EmailNotificationService : IEmailNotificationService
    {
        private readonly IEmailNotificationRepository _emailRepository;

        public EmailNotificationService(IEmailNotificationRepository emailRepository)
        {
            _emailRepository = emailRepository;
        }

        public async Task<IEnumerable<EmailNotification>> GetAllAsync()
        {
            return await _emailRepository.GetAllAsync();
        }

        public async Task<EmailNotification?> GetByIdAsync(int id)
        {
            return await _emailRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<EmailNotification>> GetByReservationAsync(int reservationId)
        {
            return await _emailRepository.GetByReservationAsync(reservationId);
        }

        public async Task<IEnumerable<EmailNotification>> GetByUserAsync(int userId)
        {
            return await _emailRepository.GetByUserAsync(userId);
        }

        public async Task<IEnumerable<EmailNotification>> GetFailedEmailsAsync()
        {
            return await _emailRepository.GetFailedEmailsAsync();
        }

        public async Task AddNotificationAsync(EmailNotification notification)
        {
            await _emailRepository.AddAsync(notification);
            await _emailRepository.SaveAsync();
        }

        public async Task UpdateStatusAsync(int notificationId, string status)
        {
            await _emailRepository.UpdateStatusAsync(notificationId, status);
        }

        public async Task<bool> DeleteNotificationAsync(int id)
        {
            var notification = await _emailRepository.GetByIdAsync(id);

            if (notification == null)
                return false;

            _emailRepository.Delete(notification);

            await _emailRepository.SaveAsync();

            return true;
        }
    }
}