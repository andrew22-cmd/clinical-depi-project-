using clinicsystem.Models;

namespace clinicsystem.Services.Interfaces
{
    public interface ISpecialityService
    {
        Task<IEnumerable<Speciality>> GetAllAsync();

        Task<Speciality?> GetByIdAsync(int id);

        Task<Speciality?> GetByNameAsync(string name);

        Task<IEnumerable<Speciality>> GetWithDoctorsAsync();

        Task AddSpecialityAsync(Speciality speciality);

        Task<bool> UpdateSpecialityAsync(Speciality speciality);

        Task<bool> DeleteSpecialityAsync(int id);
    }
}