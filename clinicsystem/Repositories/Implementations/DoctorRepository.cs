using clinicsystem.Models;
using clinicsystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace clinicsystem.Repositories.Implementations
{
    public class DoctorRepository : Repository<Doctor>, IDoctorRepository
    {
        public DoctorRepository(ClinicDbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<Doctor>> GetAllWithDetailsAsync()
        {
            return await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Speciality)
                .ToListAsync();
        }

        public async Task<Doctor?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Speciality)
                .FirstOrDefaultAsync(d => d.DoctorId == id);
        }

        public async Task<IEnumerable<Doctor>> GetBySpecialityAsync(int specialityId)
        {
            return await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Speciality)
                .Where(d => d.SpecialityId == specialityId)
                .ToListAsync();
        }
        public async Task<Doctor?> GetByUserIdAsync(int userId)
        {
            return await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Speciality)
                .FirstOrDefaultAsync(d => d.UserId == userId);
        }
    }
}