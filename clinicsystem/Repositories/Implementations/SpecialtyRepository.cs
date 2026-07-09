using clinicsystem.Models;
using clinicsystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace clinicsystem.Repositories.Implementations
{
    public class SpecialityRepository
        : Repository<Speciality>,
          ISpecialityRepository
    {
        public SpecialityRepository(ClinicDbContext context)
            : base(context)
        {
        }

        public async Task<Speciality?> GetByNameAsync(string name)
        {
            return await _context.Specialities
                                 .FirstOrDefaultAsync(s => s.Name == name);
        }

        public async Task<IEnumerable<Speciality>> GetWithDoctorsAsync()
        {
            return await _context.Specialities
                                 .Include(s => s.Doctors)
                                     .ThenInclude(d => d.User)
                                 .ToListAsync();
        }
    }
}