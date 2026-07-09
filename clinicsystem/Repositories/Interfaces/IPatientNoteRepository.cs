using clinicsystem.Models;

namespace clinicsystem.Repositories.Interfaces
{
    public interface IPatientNotesRepository : IRepository<PatientNotes>
    {
        Task<IEnumerable<PatientNotes>> GetByPatientAsync(int patientId);

        Task<IEnumerable<PatientNotes>> GetByDoctorAsync(int doctorId);

        Task<PatientNotes?> GetByReservationAsync(int reservationId);

        Task<IEnumerable<PatientNotes>> GetPatientHistoryAsync(int patientId);
        Task AddAsync(PatientNotes note);
    }
}