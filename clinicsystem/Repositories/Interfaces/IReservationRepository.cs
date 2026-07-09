using clinicsystem.Models;

namespace clinicsystem.Repositories.Interfaces
{
    public interface IReservationRepository : IRepository<Reservation>
    {
        Task<IEnumerable<Reservation>> GetAllWithDetailsAsync();

        Task<Reservation?> GetReservationWithDetailsAsync(int reservationId);

        Task<IEnumerable<Reservation>> GetByPatientAsync(int patientId);

        Task<IEnumerable<Reservation>> GetByDoctorAsync(int doctorId);

        Task<IEnumerable<Reservation>> GetByDateAsync(DateTime date);

        Task<IEnumerable<Reservation>> GetTodayReservationsAsync();

        Task<IEnumerable<Reservation>> GetByStatusAsync(string status);

        Task<int> GetReservationsCountAsync(int patientId);
        Task CreateReservationAsync(Reservation reservation);
        Task<IEnumerable<Reservation>> GetPatientReservationsDetailsAsync(int patientId);
        Task<IEnumerable<Reservation>> GetDoctorTodayReservationsAsync(int doctorId);
        Task<IEnumerable<Reservation>> GetDoctorReservationsForNotesAsync(int doctorId);
        Task<bool> IsSlotBookedAsync(int slotId, DateTime reservationDate);

    }
}