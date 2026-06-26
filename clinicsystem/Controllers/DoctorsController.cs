using clinicsystem.Models;
using clinicsystem.Services;
using clinicsystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace clinicsystem.Controllers
{
    public class DoctorsController : Controller
    {
        private const string DefaultPassword = "Doctor@123";
        private readonly AppDbContext _db;
        public DoctorsController(AppDbContext db) => _db = db;

        // ---------------- LIST (doctors-schedules) ----------------
        [HttpGet("doctors-schedules")]
        public async Task<IActionResult> Index()
        {
            var doctors = await _db.Doctors
                .Include(d => d.User)
                .Include(d => d.Speciality)
                .Include(d => d.Schedules)
                    .ThenInclude(s => s.Slots)
                .ToListAsync();

            var vm = doctors.Select(MapDoctor).ToList();
            return View(vm);
        }

        // ---------------- ADD (create doctor + first schedule) ----------------
        [HttpGet("add-schedule")]
        public IActionResult Add() => View(new AddScheduleVM());

        [HttpPost("add-schedule")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddScheduleVM vm)
        {
            var from = ClinicTime.ParseArabic(vm.From);
            var to = ClinicTime.ParseArabic(vm.To);

            if (string.IsNullOrWhiteSpace(vm.Name) || string.IsNullOrWhiteSpace(vm.Speciality)
                || string.IsNullOrWhiteSpace(vm.Day) || from == null || to == null)
            {
                ViewBag.Error = "من فضلك أكمل كل البيانات المطلوبة بصيغة وقت صحيحة (مثال: 10:00 ص).";
                return View(vm);
            }

            var spec = await FindOrCreateSpeciality(vm.Speciality);
            var (first, last) = SplitName(vm.Name);

            var user = new User
            {
                FirstName = first,
                LastName = last,
                Email = $"dr{DateTime.UtcNow.Ticks}@clinic.com",
                Phone = "",
                Password = DefaultPassword,
                Role = "doctor"
            };

            var slots = BuildSlots(vm.SlotsCsv, from.Value, to.Value);
            var schedule = new DoctorSchedule
            {
                WeekDay = vm.Day.Trim(),
                IsActive = true,
                Slots = slots
            };

            var doctor = new Doctor
            {
                User = user,
                SpecialityId = spec.SpecialityId,
                Schedules = new List<DoctorSchedule> { schedule }
            };

            _db.Doctors.Add(doctor);
            await _db.SaveChangesAsync();

            TempData["Ok"] = "تم إضافة الطبيب وجدوله بنجاح.";
            return RedirectToAction(nameof(Index));
        }

        // ---------------- EDIT ----------------
        [HttpGet("edit-schedule")]
        public async Task<IActionResult> Edit()
        {
            var vm = new EditScheduleVM { Doctors = await DoctorOptions() };
            return View(vm);
        }

        [HttpPost("edit-schedule/update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSchedule(EditScheduleVM vm)
        {
            var from = ClinicTime.ParseArabic(vm.From);
            var to = ClinicTime.ParseArabic(vm.To);

            if (vm.DoctorId == 0 || string.IsNullOrWhiteSpace(vm.Day) || from == null || to == null)
            {
                TempData["Error"] = "اختر الطبيب وأدخل اليوم والوقت بصيغة صحيحة.";
                return RedirectToAction(nameof(Edit));
            }

            var doctor = await _db.Doctors
                .Include(d => d.Schedules).ThenInclude(s => s.Slots)
                .FirstOrDefaultAsync(d => d.DoctorId == vm.DoctorId);
            if (doctor == null) { TempData["Error"] = "الطبيب غير موجود."; return RedirectToAction(nameof(Edit)); }

            var newSlots = BuildSlots(vm.SlotsCsv, from.Value, to.Value);
            var existing = doctor.Schedules?.FirstOrDefault(s => s.WeekDay == vm.Day.Trim());

            if (existing != null)
            {
                if (existing.Slots != null) _db.DoctorScheduleSlots.RemoveRange(existing.Slots);
                existing.IsActive = true;
                existing.Slots = newSlots;
            }
            else
            {
                _db.DoctorSchedules.Add(new DoctorSchedule
                {
                    DoctorId = doctor.DoctorId,
                    WeekDay = vm.Day.Trim(),
                    IsActive = true,
                    Slots = newSlots
                });
            }

            try { await _db.SaveChangesAsync(); TempData["Ok"] = "تم تحديث جدول الطبيب بنجاح."; }
            catch (DbUpdateException) { TempData["Error"] = "تعذر التحديث: قد توجد مواعيد محجوزة مرتبطة."; }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("edit-schedule/delete-day")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDay(int doctorId, string day)
        {
            var schedule = await _db.DoctorSchedules
                .Include(s => s.Slots)
                .FirstOrDefaultAsync(s => s.DoctorId == doctorId && s.WeekDay == day);
            if (schedule == null) { TempData["Error"] = "لا يوجد ميعاد لهذا اليوم."; return RedirectToAction(nameof(Edit)); }

            if (schedule.Slots != null) _db.DoctorScheduleSlots.RemoveRange(schedule.Slots);
            _db.DoctorSchedules.Remove(schedule);
            try { await _db.SaveChangesAsync(); TempData["Ok"] = "تم حذف الميعاد بنجاح."; }
            catch (DbUpdateException) { TempData["Error"] = "تعذر الحذف: قد توجد حجوزات مرتبطة بهذا اليوم."; }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("edit-schedule/delete-doctor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDoctor(int doctorId)
        {
            var doctor = await _db.Doctors
                .Include(d => d.User)
                .Include(d => d.Schedules).ThenInclude(s => s.Slots)
                .FirstOrDefaultAsync(d => d.DoctorId == doctorId);
            if (doctor == null) { TempData["Error"] = "الطبيب غير موجود."; return RedirectToAction(nameof(Edit)); }

            foreach (var sch in doctor.Schedules ?? new List<DoctorSchedule>())
                if (sch.Slots != null) _db.DoctorScheduleSlots.RemoveRange(sch.Slots);
            if (doctor.Schedules != null) _db.DoctorSchedules.RemoveRange(doctor.Schedules);
            _db.Doctors.Remove(doctor);
            if (doctor.User != null) _db.Users.Remove(doctor.User);

            try { await _db.SaveChangesAsync(); TempData["Ok"] = "تم حذف الطبيب بالكامل."; }
            catch (DbUpdateException) { TempData["Error"] = "تعذر حذف الطبيب: توجد حجوزات مرتبطة به."; }
            return RedirectToAction(nameof(Index));
        }

        // ---------------- CREATE STAFF (manager-add-staff) ----------------
        [HttpPost("doctors/create-staff")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStaff(CreateStaffVM vm)
        {
            if (string.IsNullOrWhiteSpace(vm.Name) || string.IsNullOrWhiteSpace(vm.Email))
            {
                TempData["Error"] = "الاسم والبريد الإلكتروني مطلوبان.";
                return RedirectToAction("ManagerAddStaff", "Home");
            }

            bool isDoctor = vm.Role.Trim() == "طبيب";
            var (first, last) = SplitName(vm.Name);

            var user = new User
            {
                FirstName = first,
                LastName = last,
                Email = vm.Email.Trim(),
                Phone = vm.Phone ?? "",
                Password = DefaultPassword,
                Role = isDoctor ? "doctor" : "secretary"
            };

            if (isDoctor)
            {
                var spec = await FindOrCreateSpeciality(vm.Speciality ?? "عام");
                _db.Doctors.Add(new Doctor { User = user, SpecialityId = spec.SpecialityId });
            }
            else
            {
                _db.Users.Add(user);
            }

            await _db.SaveChangesAsync();
            TempData["Ok"] = "تم إضافة الموظف بنجاح.";
            return RedirectToAction("ManagerDashboard", "Home");
        }

        // ---------------- helpers ----------------
        private DoctorListItemVM MapDoctor(Doctor d)
        {
            var item = new DoctorListItemVM
            {
                DoctorId = d.DoctorId,
                Name = DisplayName(d.User),
                Speciality = d.Speciality?.Name ?? "-"
            };
            foreach (var s in (d.Schedules ?? new List<DoctorSchedule>()).OrderBy(x => x.WeekDay))
            {
                var times = (s.Slots ?? new List<DoctorScheduleSlot>())
                    .Select(x => x.SlotTime).OrderBy(t => t).ToList();
                item.Days.Add(new ScheduleDayVM
                {
                    ScheduleId = s.ScheduleId,
                    WeekDay = s.WeekDay,
                    IsActive = s.IsActive,
                    From = times.Count > 0 ? ClinicTime.FormatArabic(times.First()) : "-",
                    To = times.Count > 0 ? ClinicTime.FormatArabic(times.Last()) : "-",
                    Slots = times.Select(ClinicTime.FormatArabic).ToList()
                });
            }
            return item;
        }

        private async Task<List<DoctorOptionVM>> DoctorOptions()
        {
            var doctors = await _db.Doctors
                .Include(d => d.User).Include(d => d.Speciality).ToListAsync();
            return doctors.Select(d => new DoctorOptionVM
            {
                DoctorId = d.DoctorId,
                Label = $"{DisplayName(d.User)} - {(d.Speciality?.Name ?? "-")}"
            }).ToList();
        }

        private async Task<Speciality> FindOrCreateSpeciality(string name)
        {
            var trimmed = (name ?? "").Trim();
            var existing = await _db.Specialities.FirstOrDefaultAsync(s => s.Name == trimmed);
            if (existing != null) return existing;
            var spec = new Speciality { Name = trimmed.Length == 0 ? "عام" : trimmed };
            _db.Specialities.Add(spec);
            await _db.SaveChangesAsync();
            return spec;
        }

        private static List<DoctorScheduleSlot> BuildSlots(string? csv, TimeSpan from, TimeSpan to)
        {
            var times = ClinicTime.ParseSlotsCsv(csv);
            if (times.Count == 0) times = ClinicTime.GenerateSlots(from, to); // Generate Available Slots
            return times.Select(t => new DoctorScheduleSlot
            {
                SlotTime = t,
                IsBooked = false,
                CreatedAt = DateTime.UtcNow
            }).ToList();
        }

        private static (string first, string last) SplitName(string name)
        {
            var parts = (name ?? "").Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return ("", "");
            if (parts.Length == 1) return (parts[0], "");
            return (parts[0], string.Join(" ", parts.Skip(1)));
        }

        private static string DisplayName(User? u)
            => u == null ? "-" : $"{u.FirstName} {u.LastName}".Trim();
    }
}
