using clinicsystem.Models;
using clinicsystem.Repositories.Interfaces;
using clinicsystem.Services.Interfaces;

namespace clinicsystem.Services.Implementations
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IDoctorScheduleService _scheduleService;
        private readonly IDoctorScheduleSlotService _slotService;
        private readonly IUserService _userService;

        public DoctorService(
            IDoctorRepository doctorRepository,
            IDoctorScheduleService scheduleService,
            IDoctorScheduleSlotService slotService,
            IUserService userService)
        {
            _doctorRepository = doctorRepository;
            _scheduleService = scheduleService;
            _slotService = slotService;
            _userService = userService;
        }

        public async Task<IEnumerable<Doctor>> GetAllAsync()
        {
            return await _doctorRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Doctor>> GetAllWithDetailsAsync()
        {
            return await _doctorRepository.GetAllWithDetailsAsync();
        }

        public async Task<Doctor?> GetByIdAsync(int id)
        {
            return await _doctorRepository.GetByIdAsync(id);
        }

        public async Task<Doctor?> GetByIdWithDetailsAsync(int id)
        {
            return await _doctorRepository.GetByIdWithDetailsAsync(id);
        }

        public async Task<IEnumerable<Doctor>> GetBySpecialityAsync(int specialityId)
        {
            return await _doctorRepository.GetBySpecialityAsync(specialityId);
        }

        public async Task AddDoctorAsync(Doctor doctor)
        {
            await _doctorRepository.AddAsync(doctor);
            await _doctorRepository.SaveAsync();
        }

        public async Task<bool> UpdateDoctorAsync(Doctor doctor)
        {
            var existing = await _doctorRepository.GetByIdAsync(doctor.DoctorId);

            if (existing == null)
                return false;

            existing.UserId = doctor.UserId;
            existing.SpecialityId = doctor.SpecialityId;

            _doctorRepository.Update(existing);

            await _doctorRepository.SaveAsync();

            return true;
        }

        public async Task<bool> DeleteDoctorAsync(int id)
        {
            var doctor = await _doctorRepository.GetByIdWithDetailsAsync(id);

            if (doctor == null)
                return false;

            // كل الجداول الخاصة بالدكتور
            var schedules = (await _scheduleService.GetDoctorSchedulesAsync(id)).ToList();

            // حذف كل الـ Slots
            foreach (var schedule in schedules)
            {
                var slots = (await _slotService.GetSlotsByScheduleAsync(schedule.ScheduleId)).ToList();

                if (slots.Any())
                    await _slotService.DeleteRangeAsync(slots);
            }

            // حذف الـ Schedules
            if (schedules.Any())
                await _scheduleService.DeleteRangeAsync(schedules);

            // حفظ الـ UserId قبل حذف الدكتور
            int userId = doctor.UserId;

            // حذف الدكتور
            _doctorRepository.Delete(doctor);
            await _doctorRepository.SaveAsync();

            // حذف اليوزر
            await _userService.DeleteUserAsync(userId);

            return true;
        }
        public async Task<Doctor?> GetDoctorByUserIdAsync(int userId)
        {
            return await _doctorRepository.GetByUserIdAsync(userId);
        }
    }
}