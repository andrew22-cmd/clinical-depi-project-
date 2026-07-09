using clinicsystem.Models;
using clinicsystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace clinicsystem.Repositories.Implementations
{
    public class PatientRepository : Repository<Patient>, IPatientRepository
    {
        public PatientRepository(ClinicDbContext context)
            : base(context)
        {
        }

        public async Task<Patient?> GetByUserIdAsync(int userId)
        {
            return await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<Patient?> GetByPhoneAsync(string phone)
        {
            return await _context.Patients
                                 .FirstOrDefaultAsync(p => p.Phone == phone);
        }

        public async Task<Patient?> GetWithReservationsAsync(int patientId)
        {
            return await _context.Patients
                                 .Include(p => p.Reservations)
                                 .FirstOrDefaultAsync(p => p.PatientId == patientId);
        }

        public async Task<IEnumerable<Patient>> SearchAsync(string keyword)
        {
            return await _context.Patients
                                 .Where(p =>
                                     p.FullName.Contains(keyword) ||
                                     p.Phone.Contains(keyword))
                                 .ToListAsync();
        }
    
    
    }
}