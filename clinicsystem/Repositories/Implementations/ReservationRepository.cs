using clinicsystem.Models;
using clinicsystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace clinicsystem.Repositories.Implementations
{
    public class ReservationRepository
        : Repository<Reservation>,
          IReservationRepository
    {
        public ReservationRepository(ClinicDbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<Reservation>> GetAllWithDetailsAsync()
        {
            return await _context.Reservations
                .Include(r => r.Patient)
                    .ThenInclude(p => p.User)
                .Include(r => r.Doctor)
                    .ThenInclude(d => d.User)
                .Include(r => r.Doctor)
                    .ThenInclude(d => d.Speciality)
                .Include(r => r.Slot)
                    .ThenInclude(s => s.Schedule)
                .Include(r => r.Notes)
                .ToListAsync();
        }
        public async Task<Reservation?> GetReservationWithDetailsAsync(int reservationId)
        {
            return await _context.Reservations
                .Include(r => r.Patient)
                    .ThenInclude(p => p.User)

                .Include(r => r.Doctor)
                    .ThenInclude(d => d.User)

                .Include(r => r.Slot)
                    .ThenInclude(s => s.Schedule)

                .FirstOrDefaultAsync(r => r.ReservationId == reservationId);
        }
        public async Task<IEnumerable<Reservation>> GetByPatientAsync(int patientId)
        {
            return await _context.Reservations
                .Include(r => r.Doctor)
                    .ThenInclude(d => d.User)
                .Include(r => r.Slot)
                .Where(r => r.PatientId == patientId)
                .OrderByDescending(r => r.ReservationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetByDoctorAsync(int doctorId)
        {
            return await _context.Reservations
                .Include(r => r.Patient)
                .Include(r => r.Slot)
                .Where(r => r.DoctorId == doctorId)
                .OrderBy(r => r.ReservationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetByDateAsync(DateTime date)
        {
            return await _context.Reservations
                .Include(r => r.Patient)
                .Include(r => r.Doctor)
                    .ThenInclude(d => d.User)
                .Include(r => r.Slot)
                    .ThenInclude(s => s.Schedule)
                .Where(r => r.ReservationDate.Date == date.Date)
                .OrderBy(r => r.Slot.SlotTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetTodayReservationsAsync()
        {
            return await GetByDateAsync(DateTime.Today);
        }

        public async Task<IEnumerable<Reservation>> GetByStatusAsync(string status)
        {
            return await _context.Reservations
                .Include(r => r.Patient)
                .Include(r => r.Doctor)
                    .ThenInclude(d => d.User)
                .Include(r => r.Slot)
                .Where(r => r.Status == status)
                .ToListAsync();
        }
        public async Task<int> GetReservationsCountAsync(int patientId)
        {
            return await _context.Reservations
                .CountAsync(r => r.PatientId == patientId);
        }
        public async Task CreateReservationAsync(Reservation reservation)
        {
            await _context.Reservations.AddAsync(reservation);
            await SaveAsync();
        }
        public async Task<IEnumerable<Reservation>> GetPatientReservationsDetailsAsync(int patientId)
        {
            return await _context.Reservations
                .Include(r => r.Doctor)
                    .ThenInclude(d => d.User)
                .Include(r => r.Doctor)
                    .ThenInclude(d => d.Speciality)
                .Include(r => r.Slot)
                    .ThenInclude(s => s.Schedule)
                .Include(r => r.Notes)   // ✅ لازم عشان hasNotes يشتغل
                .Where(r => r.PatientId == patientId)
                .OrderByDescending(r => r.ReservationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetDoctorTodayReservationsAsync(int doctorId)
        {
            var today = DateTime.Today;

            return await _context.Reservations
                .Include(r => r.Patient)
                .Include(r => r.Slot)
                .Where(r =>
                    r.DoctorId == doctorId &&
                    r.ReservationDate.Date == today &&
                    r.Status != "Cancelled")
                .OrderBy(r => r.Slot.SlotTime)
                .ToListAsync();
        }
        public async Task<IEnumerable<Reservation>> GetDoctorReservationsForNotesAsync(int doctorId)
        {
            return await _context.Reservations
                .Include(r => r.Patient)
                .Where(r =>
                    r.DoctorId == doctorId &&
                    r.Status != "Cancelled")
                .OrderByDescending(r => r.ReservationDate)
                .ToListAsync();
        }
        public async Task<bool> IsSlotBookedAsync(int slotId, DateTime reservationDate)
        {
            return await _context.Reservations.AnyAsync(r =>
                r.SlotId == slotId &&
                r.ReservationDate.Date == reservationDate.Date &&
                r.Status != "Cancelled");
        }
    }
}