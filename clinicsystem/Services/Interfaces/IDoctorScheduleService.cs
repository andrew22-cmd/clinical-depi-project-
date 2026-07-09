using clinicsystem.Models;

namespace clinicsystem.Services.Interfaces
{
    public interface IDoctorScheduleService
    {
        Task<IEnumerable<DoctorSchedule>> GetAllAsync();

        Task<IEnumerable<DoctorSchedule>> GetAllWithDetailsAsync();

        Task<DoctorSchedule?> GetByIdAsync(int id);

        Task<DoctorSchedule?> GetByIdWithDetailsAsync(int id);

        Task<IEnumerable<DoctorSchedule>> GetDoctorSchedulesAsync(int doctorId);

        Task<IEnumerable<DoctorSchedule>> GetActiveSchedulesAsync();

        Task AddScheduleAsync(DoctorSchedule schedule);

        Task<bool> UpdateScheduleAsync(DoctorSchedule schedule);

        Task<bool> DeleteScheduleAsync(int id);
        Task<bool> HasOverlapAsync(int doctorId, string day, TimeSpan startTime, TimeSpan endTime);
        Task DeleteRangeAsync(IEnumerable<DoctorSchedule> schedules);
        Task<IEnumerable<DoctorSchedule>> GetAllWithDoctorsAsync();
        Task<IEnumerable<DoctorSchedule>> GetDoctorSchedulesByDayAsync(int doctorId, string weekDay);
    }
}