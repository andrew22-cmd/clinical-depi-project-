using clinicsystem.Models;

namespace clinicsystem.Services.Interfaces
{
    public interface IDoctorService
    {
        Task<IEnumerable<Doctor>> GetAllAsync();

        Task<IEnumerable<Doctor>> GetAllWithDetailsAsync();

        Task<Doctor?> GetByIdAsync(int id);
        Task<Doctor?> GetDoctorByUserIdAsync(int userId);

        Task<Doctor?> GetByIdWithDetailsAsync(int id);

        Task<IEnumerable<Doctor>> GetBySpecialityAsync(int specialityId);

        Task AddDoctorAsync(Doctor doctor);

        Task<bool> UpdateDoctorAsync(Doctor doctor);

        Task<bool> DeleteDoctorAsync(int id);
    }
}