using clinicsystem.Models;
using clinicsystem.Repositories.Interfaces;
using clinicsystem.Services.Interfaces;

namespace clinicsystem.Services.Implementations
{
    public class SpecialityService : ISpecialityService
    {
        private readonly ISpecialityRepository _specialityRepository;

        public SpecialityService(ISpecialityRepository specialityRepository)
        {
            _specialityRepository = specialityRepository;
        }

        public async Task<IEnumerable<Speciality>> GetAllAsync()
        {
            return await _specialityRepository.GetAllAsync();
        }

        public async Task<Speciality?> GetByIdAsync(int id)
        {
            return await _specialityRepository.GetByIdAsync(id);
        }

        public async Task<Speciality?> GetByNameAsync(string name)
        {
            return await _specialityRepository.GetByNameAsync(name);
        }

        public async Task<IEnumerable<Speciality>> GetWithDoctorsAsync()
        {
            return await _specialityRepository.GetWithDoctorsAsync();
        }

        public async Task AddSpecialityAsync(Speciality speciality)
        {
            await _specialityRepository.AddAsync(speciality);
            await _specialityRepository.SaveAsync();
        }

        public async Task<bool> UpdateSpecialityAsync(Speciality speciality)
        {
            var existing = await _specialityRepository.GetByIdAsync(speciality.SpecialityId);

            if (existing == null)
                return false;

            existing.Name = speciality.Name;

            _specialityRepository.Update(existing);

            await _specialityRepository.SaveAsync();

            return true;
        }

        public async Task<bool> DeleteSpecialityAsync(int id)
        {
            var speciality = await _specialityRepository.GetByIdAsync(id);

            if (speciality == null)
                return false;

            _specialityRepository.Delete(speciality);

            await _specialityRepository.SaveAsync();

            return true;
        }
    }
}