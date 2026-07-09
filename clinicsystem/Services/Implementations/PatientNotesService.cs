using clinicsystem.Models;
using clinicsystem.Repositories.Implementations;
using clinicsystem.Repositories.Interfaces;
using clinicsystem.Services.Interfaces;

namespace clinicsystem.Services.Implementations
{
    public class PatientNotesService : IPatientNotesService
    {
        private readonly IPatientNotesRepository _notesRepository;

        public PatientNotesService(IPatientNotesRepository notesRepository)
        {
            _notesRepository = notesRepository;
        }

        public async Task<IEnumerable<PatientNotes>> GetAllAsync()
        {
            return await _notesRepository.GetAllAsync();
        }

        public async Task<PatientNotes?> GetByIdAsync(int id)
        {
            return await _notesRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<PatientNotes>> GetByPatientAsync(int patientId)
        {
            return await _notesRepository.GetByPatientAsync(patientId);
        }

        public async Task<IEnumerable<PatientNotes>> GetByDoctorAsync(int doctorId)
        {
            return await _notesRepository.GetByDoctorAsync(doctorId);
        }

        public async Task<PatientNotes?> GetByReservationAsync(int reservationId)
        {
            return await _notesRepository.GetByReservationAsync(reservationId);
        }

        public async Task<IEnumerable<PatientNotes>> GetPatientHistoryAsync(int patientId)
        {
            return await _notesRepository.GetPatientHistoryAsync(patientId);
        }

        public async Task AddNoteAsync(PatientNotes note)
        {
            await _notesRepository.AddAsync(note);
            await _notesRepository.SaveAsync();
        }

        public async Task<bool> UpdateNoteAsync(PatientNotes note)
        {
            var existing = await _notesRepository.GetByIdAsync(note.NoteId);

            if (existing == null)
                return false;

            existing.Notes = note.Notes;
            existing.Diagnosis = note.Diagnosis;
            existing.VisitDate = note.VisitDate;

            _notesRepository.Update(existing);

            await _notesRepository.SaveAsync();

            return true;
        }

        public async Task<bool> DeleteNoteAsync(int id)
        {
            var note = await _notesRepository.GetByIdAsync(id);

            if (note == null)
                return false;

            _notesRepository.Delete(note);

            await _notesRepository.SaveAsync();

            return true;
        }
        public async Task AddAsync(PatientNotes note)
        {
            await _notesRepository.AddAsync(note);
        }
    }
}