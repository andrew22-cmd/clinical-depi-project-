using clinicsystem.Models;

namespace clinicsystem.Repositories.Interfaces
{
    public interface IDoctorRepository : IRepository<Doctor>
    {
        Task<IEnumerable<Doctor>> GetAllWithDetailsAsync();

        Task<Doctor?> GetByIdWithDetailsAsync(int id);

        Task<IEnumerable<Doctor>> GetBySpecialityAsync(int specialityId);
        Task<Doctor?> GetByUserIdAsync(int userId);
    }
}