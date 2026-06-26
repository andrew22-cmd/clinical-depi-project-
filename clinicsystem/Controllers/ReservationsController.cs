using clinicsystem.Models;
using clinicsystem.Services;
using clinicsystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace clinicsystem.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly AppDbContext _db;
        public ReservationsController(AppDbContext db) => _db = db;

        // GET /reservation
        [HttpGet("reservation")]
        public async Task<IActionResult> Index()
        {
            var vm = new ReservationIndexVM
            {
                Specialities = await _db.Specialities
                    .OrderBy(s => s.Name)
                    .Select(s => new SpecialityOptionVM { SpecialityId = s.SpecialityId, Name = s.Name })
                    .ToListAsync()
            };
            return View(vm);
        }

        // GET /reservation/doctors?specialityId=#
        [HttpGet("reservation/doctors")]
        public async Task<IActionResult> DoctorsBySpecialty(int specialityId)
        {
            var doctors = await _db.Doctors
                .Include(d => d.User)
                .Where(d => d.SpecialityId == specialityId)
                .ToListAsync();

            var result = doctors.Select(d => new
            {
                id = d.DoctorId,
                name = $"{d.User!.FirstName} {d.User.LastName}".Trim()
            });
            return Json(result);
        }

        // GET /reservation/slots?doctorId=#  -> available (unbooked) slots grouped by weekday
        [HttpGet("reservation/slots")]
        public async Task<IActionResult> AvailableSlots(int doctorId)
        {
            var schedules = await _db.DoctorSchedules
                .Include(s => s.Slots)
                .Where(s => s.DoctorId == doctorId && s.IsActive)
                .ToListAsync();

            var result = schedules.Select(s => new
            {
                scheduleId = s.ScheduleId,
                day = s.WeekDay,
                slots = (s.Slots ?? new List<DoctorScheduleSlot>())
                    .Where(x => !x.IsBooked)
                    .OrderBy(x => x.SlotTime)
                    .Select(x => new { slotId = x.SlotId, time = ClinicTime.FormatArabic(x.SlotTime) })
            }).Where(x => x.slots.Any());

            return Json(result);
        }

        // POST /reservation/book
        [HttpPost("reservation/book")]
        public async Task<IActionResult> Book([FromBody] BookRequest req)
        {
            var slot = await _db.DoctorScheduleSlots
                .Include(s => s.Schedule)
                .FirstOrDefaultAsync(s => s.SlotId == req.SlotId);

            if (slot == null) return Json(new { ok = false, message = "الموعد غير موجود." });
            if (slot.IsBooked) return Json(new { ok = false, message = "هذا الموعد محجوز بالفعل." });
            if (string.IsNullOrWhiteSpace(req.PatientName) || string.IsNullOrWhiteSpace(req.Phone))
                return Json(new { ok = false, message = "اسم المريض ورقم الهاتف مطلوبان." });

            var phone = req.Phone.Trim();
            var patient = await _db.Patients.FirstOrDefaultAsync(p => p.Phone == phone);
            if (patient == null)
            {
                patient = new Patient
                {
                    FullName = req.PatientName.Trim(),
                    Phone = phone,
                    CreatedAt = DateTime.UtcNow
                };
                _db.Patients.Add(patient);
                await _db.SaveChangesAsync();
            }

            var reservation = new Reservation
            {
                PatientId = patient.PatientId,
                DoctorId = slot.Schedule!.DoctorId,
                SlotId = slot.SlotId,
                ReservationDate = ClinicTime.NextDateFor(slot.Schedule.WeekDay),
                Status = "confirmed",
                Source = "online",
                CreatedAt = DateTime.UtcNow
            };
            _db.Reservations.Add(reservation);

            slot.IsBooked = true;
            await _db.SaveChangesAsync();

            return Json(new
            {
                ok = true,
                message = "تم تأكيد الحجز بنجاح.",
                day = slot.Schedule.WeekDay,
                time = ClinicTime.FormatArabic(slot.SlotTime)
            });
        }
    }
}
