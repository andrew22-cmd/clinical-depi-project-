using clinicsystem.Models;
using clinicsystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace clinicsystem.Repositories.Implementations
{
    public class DoctorScheduleRepository
        : Repository<DoctorSchedule>,
          IDoctorScheduleRepository
    {
        public DoctorScheduleRepository(ClinicDbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<DoctorSchedule>> GetAllWithDetailsAsync()
        {
            return await _context.DoctorSchedules
                .Include(s => s.Doctor)
                    .ThenInclude(d => d.User)
                .Include(s => s.Doctor)
                    .ThenInclude(d => d.Speciality)
                .Include(s => s.Slots)
                .ToListAsync();
        }

        public async Task<DoctorSchedule?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.DoctorSchedules
                .Include(s => s.Doctor)
                    .ThenInclude(d => d.User)
                .Include(s => s.Doctor)
                    .ThenInclude(d => d.Speciality)
                .Include(s => s.Slots)
                .FirstOrDefaultAsync(s => s.ScheduleId == id);
        }

        public async Task<IEnumerable<DoctorSchedule>> GetDoctorSchedulesAsync(int doctorId)
        {
            return await _context.DoctorSchedules
                .Include(s => s.Slots)
                .Where(s => s.DoctorId == doctorId)
                .ToListAsync();
        }

        public async Task<IEnumerable<DoctorSchedule>> GetActiveSchedulesAsync()
        {
            return await _context.DoctorSchedules
                .Include(s => s.Doctor)
                    .ThenInclude(d => d.User)
                .Include(s => s.Slots)
                .Where(s => s.IsActive)
                .ToListAsync();
        }
        public async Task<bool> HasOverlapAsync(int doctorId, string day, TimeSpan startTime, TimeSpan endTime)
        {
            return await _context.DoctorSchedules
                .Include(s => s.Slots)
                .AnyAsync(s =>
                    s.DoctorId == doctorId &&
                    s.WeekDay == day &&
                    s.Slots.Any() &&
                    startTime < s.Slots.Max(x => x.SlotTime).Add(TimeSpan.FromMinutes(30)) &&
                    endTime > s.Slots.Min(x => x.SlotTime)
                );
        }
        public async Task DeleteRangeAsync(IEnumerable<DoctorSchedule> schedules)
        {
            _context.DoctorSchedules.RemoveRange(schedules);
            await SaveAsync();
        }
        public async Task<IEnumerable<DoctorSchedule>> GetAllWithDoctorsAsync()
        {
            return await _context.DoctorSchedules
                .Include(s => s.Doctor)
                    .ThenInclude(d => d.User)

                .Include(s => s.Doctor)
                    .ThenInclude(d => d.Speciality)

                .Include(s => s.Slots)

                .OrderBy(s => s.WeekDay)
                .ThenBy(s => s.StartTime)
                .ToListAsync();
        }
        public async Task<IEnumerable<DoctorSchedule>> GetDoctorSchedulesByDayAsync(int doctorId, string weekDay)
        {
            return await _context.DoctorSchedules
                .Where(s =>
                    s.DoctorId == doctorId &&
                    s.WeekDay == weekDay)
                .ToListAsync();
        }
    }
}