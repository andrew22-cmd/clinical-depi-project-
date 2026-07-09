using clinicsystem.Models;

namespace clinicsystem.Services.Interfaces
{
    public interface IReservationService
    {
        Task<IEnumerable<Reservation>> GetAllAsync();

        Task<Reservation?> GetByIdAsync(int id);

        Task<IEnumerable<Reservation>> GetTodayReservationsAsync();

        Task<IEnumerable<Reservation>> GetByDoctorAsync(int doctorId);

        Task<IEnumerable<Reservation>> GetByPatientAsync(int patientId);

        Task<IEnumerable<Reservation>> GetByStatusAsync(string status);

        Task<bool> CreateReservationAsync(Reservation reservation);

        Task<bool> UpdateReservationAsync(Reservation reservation);

        Task<bool> CancelReservationAsync(int reservationId);
        Task<int> GetReservationsCountAsync(int patientId);
        Task<IEnumerable<Reservation>> GetPatientReservationsDetailsAsync(int patientId);
        Task<IEnumerable<Reservation>> GetDoctorTodayReservationsAsync(int doctorId);
        Task<IEnumerable<Reservation>> GetDoctorReservationsForNotesAsync(int doctorId);
        Task<Reservation?> GetReservationWithDetailsAsync(int reservationId);
        Task<IEnumerable<Reservation>> GetByDateAsync(DateTime date);
        Task<bool> IsSlotBookedAsync(int slotId, DateTime reservationDate);
    }
}