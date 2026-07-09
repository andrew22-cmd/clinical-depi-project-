using clinicsystem.Models;
using clinicsystem.Repositories.Interfaces;

namespace clinicsystem.Repositories.Interfaces
{
    public interface IDoctorScheduleSlotRepository : IRepository<DoctorScheduleSlot>
    {
        Task<IEnumerable<DoctorScheduleSlot>> GetSlotsByScheduleAsync(int scheduleId);
        Task<DoctorScheduleSlot?> GetByIdWithScheduleAsync(int slotId);

        Task<IEnumerable<DoctorScheduleSlot>> GetAvailableSlotsAsync(int scheduleId, DateTime reservationDate);

        Task<DoctorScheduleSlot?> GetSlotWithReservationsAsync(int slotId);

        Task BookSlotAsync(int slotId);

        Task UnBookSlotAsync(int slotId);
        Task DeleteRangeAsync(IEnumerable<DoctorScheduleSlot> slots);
        Task<IEnumerable<DoctorScheduleSlot>> GetAvailableSlotsAsync(int scheduleId);
    }
}