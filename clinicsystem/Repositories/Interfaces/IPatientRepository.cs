using clinicsystem.Models;

namespace clinicsystem.Repositories.Interfaces
{
    public interface IPatientRepository : IRepository<Patient>
    {
        Task<Patient?> GetByUserIdAsync(int userId);

        Task<Patient?> GetByPhoneAsync(string phone);

        Task<Patient?> GetWithReservationsAsync(int patientId);

        Task<IEnumerable<Patient>> SearchAsync(string keyword);

    }
}