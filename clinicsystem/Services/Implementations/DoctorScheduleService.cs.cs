using clinicsystem.Models;
using clinicsystem.Repositories.Implementations;
using clinicsystem.Repositories.Interfaces;
using clinicsystem.Services.Interfaces;

namespace clinicsystem.Services.Implementations
{
    public class DoctorScheduleService : IDoctorScheduleService
    {
        private readonly IDoctorScheduleRepository _scheduleRepository;
        private readonly IDoctorScheduleSlotService _slotService;
        public DoctorScheduleService(
     IDoctorScheduleRepository scheduleRepository,
     IDoctorScheduleSlotService slotService)
        {
            _scheduleRepository = scheduleRepository;
            _slotService = slotService;
        }

        public async Task<IEnumerable<DoctorSchedule>> GetAllAsync()
        {
            return await _scheduleRepository.GetAllAsync();
        }

        public async Task<IEnumerable<DoctorSchedule>> GetAllWithDetailsAsync()
        {
            return await _scheduleRepository.GetAllWithDetailsAsync();
        }

        public async Task<DoctorSchedule?> GetByIdAsync(int id)
        {
            return await _scheduleRepository.GetByIdAsync(id);
        }

        public async Task<DoctorSchedule?> GetByIdWithDetailsAsync(int id)
        {
            return await _scheduleRepository.GetByIdWithDetailsAsync(id);
        }

        public async Task<IEnumerable<DoctorSchedule>> GetDoctorSchedulesAsync(int doctorId)
        {
            return await _scheduleRepository.GetDoctorSchedulesAsync(doctorId);
        }

        public async Task<IEnumerable<DoctorSchedule>> GetActiveSchedulesAsync()
        {
            return await _scheduleRepository.GetActiveSchedulesAsync();
        }

        public async Task AddScheduleAsync(DoctorSchedule schedule)
        {
            await _scheduleRepository.AddAsync(schedule);
            await _scheduleRepository.SaveAsync();
        }

        public async Task<bool> UpdateScheduleAsync(DoctorSchedule schedule)
        {
            var existing = await _scheduleRepository.GetByIdAsync(schedule.ScheduleId);

            if (existing == null)
                return false;

            existing.DoctorId = schedule.DoctorId;
            existing.WeekDay = schedule.WeekDay;
            existing.IsActive = schedule.IsActive;

            _scheduleRepository.Update(existing);

            await _scheduleRepository.SaveAsync();

            return true;
        }

        public async Task<bool> DeleteScheduleAsync(int id)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(id);

            if (schedule == null)
                return false;

            // حذف جميع الـ Slots الخاصة بالجدول
            var slots = await _slotService.GetSlotsByScheduleAsync(id);

            foreach (var slot in slots)
            {
                await _slotService.DeleteSlotAsync(slot.SlotId);
            }

            // حذف الجدول
            _scheduleRepository.Delete(schedule);

            await _scheduleRepository.SaveAsync();

            return true;
        }
        public async Task<bool> HasOverlapAsync(int doctorId, string day, TimeSpan startTime, TimeSpan endTime)
        {
            return await _scheduleRepository.HasOverlapAsync(doctorId, day, startTime, endTime);
        }
        public async Task DeleteRangeAsync(IEnumerable<DoctorSchedule> schedules)
        {
            await _scheduleRepository.DeleteRangeAsync(schedules);
        }
        public async Task<IEnumerable<DoctorSchedule>> GetAllWithDoctorsAsync()
        {
            return await _scheduleRepository.GetAllWithDoctorsAsync();
        }
        public async Task<IEnumerable<DoctorSchedule>> GetDoctorSchedulesByDayAsync(int doctorId, string weekDay)
        {
            return await _scheduleRepository.GetDoctorSchedulesByDayAsync(doctorId, weekDay);
        }
    }
}