using clinicsystem.Models;

namespace clinicsystem.Repositories.Interfaces
{
    public interface ISpecialityRepository : IRepository<Speciality>
    {
        Task<Speciality?> GetByNameAsync(string name);

        Task<IEnumerable<Speciality>> GetWithDoctorsAsync();
    }
}