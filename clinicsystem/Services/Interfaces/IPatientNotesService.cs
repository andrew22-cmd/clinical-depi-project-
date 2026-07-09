using clinicsystem.Models;

namespace clinicsystem.Services.Interfaces
{
    public interface IPatientNotesService
    {
        Task<IEnumerable<PatientNotes>> GetAllAsync();

        Task<PatientNotes?> GetByIdAsync(int id);

        Task<IEnumerable<PatientNotes>> GetByPatientAsync(int patientId);

        Task<IEnumerable<PatientNotes>> GetByDoctorAsync(int doctorId);

        Task<PatientNotes?> GetByReservationAsync(int reservationId);

        Task<IEnumerable<PatientNotes>> GetPatientHistoryAsync(int patientId);

        Task AddNoteAsync(PatientNotes note);

        Task<bool> UpdateNoteAsync(PatientNotes note);

        Task<bool> DeleteNoteAsync(int id);

        Task AddAsync(PatientNotes note);
    }

}