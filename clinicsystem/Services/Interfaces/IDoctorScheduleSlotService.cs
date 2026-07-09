using clinicsystem.Models;

namespace clinicsystem.Services.Interfaces
{
    public interface IDoctorScheduleSlotService
    {
        Task<IEnumerable<DoctorScheduleSlot>> GetAllAsync();

        Task<DoctorScheduleSlot?> GetByIdAsync(int id);

        Task<IEnumerable<DoctorScheduleSlot>> GetSlotsByScheduleAsync(int scheduleId);

        Task<IEnumerable<DoctorScheduleSlot>> GetAvailableSlotsAsync(int scheduleId, DateTime reservationDate);

        Task<DoctorScheduleSlot?> GetSlotWithReservationsAsync(int slotId);

        Task AddSlotAsync(DoctorScheduleSlot slot);

        Task<bool> UpdateSlotAsync(DoctorScheduleSlot slot);

        Task<bool> DeleteSlotAsync(int id);

        Task<bool> BookSlotAsync(int slotId);

        Task<bool> UnBookSlotAsync(int slotId);
        Task DeleteRangeAsync(IEnumerable<DoctorScheduleSlot> slots);
        Task<DoctorScheduleSlot?> GetByIdWithScheduleAsync(int slotId);
        Task<IEnumerable<DoctorScheduleSlot>> GetAvailableSlotsAsync(int scheduleId);
    }
}