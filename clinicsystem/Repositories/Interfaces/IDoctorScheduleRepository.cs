using clinicsystem.Models;

namespace clinicsystem.Repositories.Interfaces
{
    public interface IDoctorScheduleRepository : IRepository<DoctorSchedule>
    {
        Task<IEnumerable<DoctorSchedule>> GetAllWithDetailsAsync();

        Task<DoctorSchedule?> GetByIdWithDetailsAsync(int id);

        Task<IEnumerable<DoctorSchedule>> GetDoctorSchedulesAsync(int doctorId);

        Task<IEnumerable<DoctorSchedule>> GetActiveSchedulesAsync();
        Task<bool> HasOverlapAsync(int doctorId, string day, TimeSpan startTime, TimeSpan endTime);
        Task DeleteRangeAsync(IEnumerable<DoctorSchedule> schedules);
        Task<IEnumerable<DoctorSchedule>> GetAllWithDoctorsAsync();
        Task<IEnumerable<DoctorSchedule>> GetDoctorSchedulesByDayAsync(int doctorId, string weekDay);
    }
}