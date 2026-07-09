using clinicsystem.Models;
using clinicsystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace clinicsystem.Repositories.Implementations
{
    public class EmailNotificationRepository
        : Repository<EmailNotification>,
          IEmailNotificationRepository
    {
        public EmailNotificationRepository(ClinicDbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<EmailNotification>> GetByReservationAsync(int reservationId)
        {
            return await _context.EmailNotifications
                                 .Where(e => e.ReservationId == reservationId)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<EmailNotification>> GetByUserAsync(int userId)
        {
            return await _context.EmailNotifications
                                 .Where(e => e.RecipientUserId == userId)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<EmailNotification>> GetFailedEmailsAsync()
        {
            return await _context.EmailNotifications
                                 .Where(e => e.Status == "Failed")
                                 .ToListAsync();
        }

        public async Task UpdateStatusAsync(int notificationId, string status)
        {
            var notification = await _context.EmailNotifications
                                             .FindAsync(notificationId);

            if (notification != null)
            {
                notification.Status = status;

                await SaveAsync();
            }
        }
    }
}