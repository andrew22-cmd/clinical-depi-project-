using clinicsystem.Models;
using clinicsystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace clinicsystem.Repositories.Implementations
{
    public class PatientNotesRepository
        : Repository<PatientNotes>,
          IPatientNotesRepository
    {
        public PatientNotesRepository(ClinicDbContext context)
            : base(context)
        {
        }

        public IEnumerable<PatientNotes> GetByPatient(int patientId)
        {
            return _context.PatientNotes
                           .Where(n => n.PatientId == patientId)
                           .OrderByDescending(n => n.VisitDate)
                           .ToList();
        }

        public IEnumerable<PatientNotes> GetByDoctor(int doctorId)
        {
            return _context.PatientNotes
                           .Where(n => n.DoctorId == doctorId)
                           .OrderByDescending(n => n.VisitDate)
                           .ToList();
        }

        public PatientNotes? GetByReservation(int reservationId)
        {
            return _context.PatientNotes
                           .FirstOrDefault(n => n.ReservationId == reservationId);
        }

        public IEnumerable<PatientNotes> GetPatientHistory(int patientId)
        {
            return _context.PatientNotes
                           .Where(n => n.PatientId == patientId)
                           .Include(n => n.Reservation)
                           .OrderByDescending(n => n.VisitDate)
                           .ToList();
        }

        public async Task<IEnumerable<PatientNotes>> GetByPatientAsync(int patientId)
        {
            return await _context.PatientNotes
                .Where(x => x.PatientId == patientId)
                .OrderByDescending(x => x.VisitDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<PatientNotes>> GetByDoctorAsync(int doctorId)
        {
            return await _context.PatientNotes
                .Where(x => x.DoctorId == doctorId)
                .OrderByDescending(x => x.VisitDate)
                .ToListAsync();
        }

        public async Task<PatientNotes?> GetByReservationAsync(int reservationId)
        {
            return await _context.PatientNotes
                .FirstOrDefaultAsync(x => x.ReservationId == reservationId);
        }

        public async Task<IEnumerable<PatientNotes>> GetPatientHistoryAsync(int patientId)
        {
            return await _context.PatientNotes
                .Where(x => x.PatientId == patientId)
                .Include(x => x.Reservation)
    .ThenInclude(r => r.Doctor)
    .ThenInclude(d => d.Speciality)
                .OrderByDescending(x => x.VisitDate)
                .ToListAsync();
        }
        public async Task<IEnumerable<Reservation>> GetDoctorTodayReservationsAsync(int doctorId)
        {
            return await _context.Reservations
                .Include(r => r.Patient)
                .Include(r => r.Slot)
                .Where(r =>
                    r.DoctorId == doctorId &&
                    r.ReservationDate.Date == DateTime.Today &&
                    r.Status == "Confirmed")
                .OrderBy(r => r.Slot.SlotTime)
                .ToListAsync();
        }
        public async Task AddAsync(PatientNotes note)
        {
            await _context.PatientNotes.AddAsync(note);
            await _context.SaveChangesAsync();
        }
    }
}