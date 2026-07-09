using clinicsystem.Models;

namespace clinicsystem.Services.Interfaces
{
    public interface IPatientService
    {
        Task<IEnumerable<Patient>> GetAllAsync();

        Task<Patient?> GetByIdAsync(int id);

        Task<Patient?> GetByUserIdAsync(int userId);

        Task<Patient?> GetByPhoneAsync(string phone);

        Task<Patient?> GetWithReservationsAsync(int patientId);

        Task<IEnumerable<Patient>> SearchAsync(string keyword);

        Task AddPatientAsync(Patient patient);

        Task<bool> UpdatePatientAsync(Patient patient);

        Task<bool> DeletePatientAsync(int id);
        

Task<Patient> CreateOfflinePatientAsync(string name, string phone);

    }
}