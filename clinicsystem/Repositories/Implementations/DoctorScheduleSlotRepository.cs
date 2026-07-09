using clinicsystem.Models;
using clinicsystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace clinicsystem.Repositories.Implementations
{
    public class DoctorScheduleSlotRepository
        : Repository<DoctorScheduleSlot>,
          IDoctorScheduleSlotRepository
    {
        public DoctorScheduleSlotRepository(ClinicDbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<DoctorScheduleSlot>> GetSlotsByScheduleAsync(int scheduleId)
        {
            return await _context.DoctorScheduleSlots
                                 .Where(s => s.ScheduleId == scheduleId)
                                 .OrderBy(s => s.SlotTime)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<DoctorScheduleSlot>> GetAvailableSlotsAsync(int scheduleId, DateTime reservationDate)
        {
            return await _context.DoctorScheduleSlots
                .Where(s => s.ScheduleId == scheduleId)
                .Where(s => !_context.Reservations.Any(r =>
                    r.SlotId == s.SlotId &&
                    r.ReservationDate.Date == reservationDate.Date))
                .OrderBy(s => s.SlotTime)
                .ToListAsync();
        }
        public async Task<DoctorScheduleSlot?> GetSlotWithReservationsAsync(int slotId)
        {
            return await _context.DoctorScheduleSlots
                                 .Include(s => s.Reservations)
                                 .FirstOrDefaultAsync(s => s.SlotId == slotId);
        }

        public async Task BookSlotAsync(int slotId)
        {
            var slot = await _context.DoctorScheduleSlots.FindAsync(slotId);

            if (slot != null)
            {
                slot.IsBooked = true;
                await SaveAsync();
            }
        }

        public async Task UnBookSlotAsync(int slotId)
        {
            var slot = await _context.DoctorScheduleSlots.FindAsync(slotId);

            if (slot != null)
            {
                slot.IsBooked = false;
                await SaveAsync();
            }
        }
        public async Task DeleteRangeAsync(IEnumerable<DoctorScheduleSlot> slots)
        {
            _context.DoctorScheduleSlots.RemoveRange(slots);
            await SaveAsync();
        }
        public async Task<DoctorScheduleSlot?> GetByIdWithScheduleAsync(int slotId)
        {
            return await _context.DoctorScheduleSlots
                .Include(x => x.Schedule)
                .FirstOrDefaultAsync(x => x.SlotId == slotId);
        }
        public async Task<IEnumerable<DoctorScheduleSlot>> GetAvailableSlotsAsync(int scheduleId)
        {
            return await _context.DoctorScheduleSlots
                .Where(s => s.ScheduleId == scheduleId && !s.IsBooked)
                .OrderBy(s => s.SlotTime)
                .ToListAsync();
        }
    }
}