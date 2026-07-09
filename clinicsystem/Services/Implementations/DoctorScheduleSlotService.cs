using clinicsystem.Models;
using clinicsystem.Repositories.Interfaces;
using clinicsystem.Services.Interfaces;

namespace clinicsystem.Services.Implementations
{
    public class DoctorScheduleSlotService : IDoctorScheduleSlotService
    {
        private readonly IDoctorScheduleSlotRepository _slotRepository;

        public DoctorScheduleSlotService(IDoctorScheduleSlotRepository slotRepository)
        {
            _slotRepository = slotRepository;
        }

        public async Task<IEnumerable<DoctorScheduleSlot>> GetAllAsync()
        {
            return await _slotRepository.GetAllAsync();
        }

        public async Task<DoctorScheduleSlot?> GetByIdAsync(int id)
        {
            return await _slotRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<DoctorScheduleSlot>> GetSlotsByScheduleAsync(int scheduleId)
        {
            return await _slotRepository.GetSlotsByScheduleAsync(scheduleId);
        }

        public async Task<IEnumerable<DoctorScheduleSlot>> GetAvailableSlotsAsync(int scheduleId, DateTime reservationDate)
        {
            return await _slotRepository.GetAvailableSlotsAsync(scheduleId, reservationDate);
        }

        public async Task<DoctorScheduleSlot?> GetSlotWithReservationsAsync(int slotId)
        {
            return await _slotRepository.GetSlotWithReservationsAsync(slotId);
        }

        public async Task AddSlotAsync(DoctorScheduleSlot slot)
        {
            await _slotRepository.AddAsync(slot);
            await _slotRepository.SaveAsync();
        }

        public async Task<bool> UpdateSlotAsync(DoctorScheduleSlot slot)
        {
            var existing = await _slotRepository.GetByIdAsync(slot.SlotId);

            if (existing == null)
                return false;

            existing.ScheduleId = slot.ScheduleId;
            existing.SlotTime = slot.SlotTime;
            existing.IsBooked = slot.IsBooked;

            _slotRepository.Update(existing);

            await _slotRepository.SaveAsync();

            return true;
        }

        public async Task<bool> DeleteSlotAsync(int id)
        {
            var slot = await _slotRepository.GetByIdAsync(id);

            if (slot == null)
                return false;

            _slotRepository.Delete(slot);

            await _slotRepository.SaveAsync();

            return true;
        }

        public async Task<bool> BookSlotAsync(int slotId)
        {
            var slot = await _slotRepository.GetByIdAsync(slotId);

            if (slot == null)
                return false;

            if (slot.IsBooked)
                return false;

            await _slotRepository.BookSlotAsync(slotId);

            return true;
        }

        public async Task<bool> UnBookSlotAsync(int slotId)
        {
            var slot = await _slotRepository.GetByIdAsync(slotId);

            if (slot == null)
                return false;

            await _slotRepository.UnBookSlotAsync(slotId);

            return true;
        }
        public async Task DeleteRangeAsync(IEnumerable<DoctorScheduleSlot> slots)
        {
            await _slotRepository.DeleteRangeAsync(slots);
        }
        public async Task<DoctorScheduleSlot?> GetByIdWithScheduleAsync(int slotId)
        {
            return await _slotRepository.GetByIdWithScheduleAsync(slotId);
        }
        public async Task<IEnumerable<DoctorScheduleSlot>> GetAvailableSlotsAsync(int scheduleId)
        {
            return await _slotRepository.GetAvailableSlotsAsync(scheduleId);
        }
    }
}