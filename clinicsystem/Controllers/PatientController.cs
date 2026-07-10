using clinicsystem.Models;
using clinicsystem.Services.Implementations;
using clinicsystem.Services.Interfaces;
using clinicsystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
namespace clinicsystem.Controllers
{
    public class PatientController : Controller
    {
        private readonly ISpecialityService _specialityService;
        private readonly IDoctorService _doctorService;
        private readonly IDoctorScheduleService _scheduleService;
        private readonly IDoctorScheduleSlotService _slotService;
        private readonly IReservationService _reservationService;
        private readonly IPatientService _patientService;
        private readonly IUserService _userService;
        private readonly IPatientNotesService _patientNotesService;
        public PatientController(
            ISpecialityService specialityService,
            IDoctorService doctorService,
            IDoctorScheduleService scheduleService,
            IDoctorScheduleSlotService slotService,
            IReservationService reservationService,
            IPatientService patientService, IUserService userService, IPatientNotesService patientNotesService)
        {
            _specialityService = specialityService;
            _doctorService = doctorService;
            _scheduleService = scheduleService;
            _slotService = slotService;
            _reservationService = reservationService;
            _patientService = patientService;
            _userService = userService;
            _patientNotesService = patientNotesService;
        }

        public IActionResult DashBoard()
        {
            return View();
        }

        public IActionResult Details()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Reservation()
        {
          

            return View();
        }

        public IActionResult ReservationInfo()
        {
            return View();
        }

        public IActionResult EmailNotification()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetSpecialities()
        {
            var data = await _specialityService.GetAllAsync();

            return Json(new
            {
                success = true,
                data = data.Select(x => new
                {
                    id = x.SpecialityId,
                    name = x.Name,
                    fee = x.ConsultationFee
                })
            });

        }
        [HttpGet]
        public async Task<IActionResult> GetPatientInfo()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return Json(new { success = false, message = "Unauthorized access." });

            var patient = await _patientService.GetByUserIdAsync(userId.Value);

            if (patient == null)
                return Json(new { success = false, message = "المريض غير موجود." });

            return Json(new
            {
                success = true,
                name = patient.FullName,
                phone = patient.Phone,
                email = patient.User.Email
            });
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdatePatientProfileVM model)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return Unauthorized();

            var patient = await _patientService.GetByUserIdAsync(userId.Value);

            if (patient == null)
                return Json(new { success = false, message = "المريض غير موجود." });

            var user = await _userService.GetUserByIdAsync(userId.Value);

            if (user == null)
                return Json(new { success = false, message = "المستخدم غير موجود." });

            user.FirstName = model.Name;
            user.Phone = model.Phone;

            if (!string.IsNullOrWhiteSpace(model.Password))
                user.Password = model.Password;

            await _userService.UpdateUserAsync(user);

            patient.FullName = model.Name;
            patient.Phone = model.Phone;

            await _patientService.UpdatePatientAsync(patient);

            return Json(new
            {
                success = true,
                message = "تم تحديث البيانات بنجاح"
            });
        }
        [HttpGet]
        public async Task<IActionResult> GetDoctorsBySpeciality(int specialityId)
        {
            var doctors = await _doctorService.GetBySpecialityAsync(specialityId);

            var result = doctors.Select(d => new
            {
                id = d.DoctorId,
                name = $"{d.User.FirstName} {d.User.LastName}"
            });

            return Json(new { success = true, data = result });
        }

        private string GetArabicDayName(DateTime date)
        {
            return date.DayOfWeek switch
            {
                DayOfWeek.Saturday => "السبت",
                DayOfWeek.Sunday => "الأحد",
                DayOfWeek.Monday => "الاثنين",
                DayOfWeek.Tuesday => "الثلاثاء",
                DayOfWeek.Wednesday => "الأربعاء",
                DayOfWeek.Thursday => "الخميس",
                DayOfWeek.Friday => "الجمعة",
                _ => ""
            };
        }
        [HttpGet]
        public async Task<IActionResult> GetDoctorSlots(int doctorId, DateTime reservationDate)
        {
            string day = GetArabicDayName(reservationDate);

            var schedules =
                await _scheduleService.GetDoctorSchedulesByDayAsync(doctorId, day);

            var result = new List<object>();

            foreach (var schedule in schedules)
            {
                var slots = await _slotService.GetAvailableSlotsAsync(
    schedule.ScheduleId,
    reservationDate
);

                result.Add(new
                {
                    day = schedule.WeekDay,
                    slots = slots.Select(s => new
                    {
                        slotId = s.SlotId,
                        time = s.SlotTime.ToString(@"hh\:mm")
                    })
                });
            }

            return Json(new { success = true, data = result });
        }
        [HttpPost]
        public async Task<IActionResult> BookReservation([FromBody] BookReservationVM model)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return Json(new { success = false, message = "غير مصرح." });

            var patient = await _patientService.GetByUserIdAsync(userId.Value);

            if (patient == null)
                return Json(new
                {
                    success = false,
                    message = "المريض غير موجود."
                });

            var slot = await _slotService.GetByIdWithScheduleAsync(model.SlotId);

            if (slot == null)
                return Json(new
                {
                    success = false,
                    message = "الموعد غير موجود."
                });

            var reservations = await _reservationService.GetByDateAsync(model.ReservationDate);

            bool booked = reservations.Any(r => r.SlotId == model.SlotId);

            if (booked)
            {
                return Json(new
                {
                    success = false,
                    message = "هذا الموعد محجوز بالفعل."
                });
            }

            var reservation = new Reservation
            {
                PatientId = patient.PatientId,
                DoctorId = slot.Schedule.DoctorId,
                SlotId = slot.SlotId,
                ReservationDate = model.ReservationDate,
                Status = "Confirmed",
                Source = "Online",
                CreatedBy = userId.Value,
                CreatedAt = DateTime.Now
            };

            var success = await _reservationService.CreateReservationAsync(reservation);

            if (!success)
            {
                return Json(new
                {
                    success = false,
                    message = "فشل الحجز."
                });
            }

            return Json(new
            {
                success = true,
                message = "تم الحجز بنجاح."
            });
        }
        [HttpGet]
        public async Task<IActionResult> GetReservationHistory()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return Json(new { success = false, message = "Unauthorized access." });

            var patient = await _patientService.GetByUserIdAsync(userId.Value);

            if (patient == null)
                return Json(new { success = false, message = "Patient not found." });

            var reservations =
                await _reservationService.GetPatientReservationsDetailsAsync(patient.PatientId);

            var result = reservations.Select(r => new
            {
                doctor = $"{r.Doctor.User.FirstName} {r.Doctor.User.LastName}",
                speciality = r.Doctor.Speciality.Name,
                consultationFee = r.Doctor.Speciality.ConsultationFee,
                day = r.Slot.Schedule.WeekDay,
                date = r.ReservationDate.ToString("yyyy-MM-dd"),
                time = r.Slot.SlotTime.ToString(@"hh\:mm"),
                source = r.Source,
                status = r.Status,
                reservationId = r.ReservationId,
                hasNotes = r.Notes != null && r.Notes.Any()
            });

            return Json(new { success = true, data = result });
        }
        [HttpPost]
        public async Task<IActionResult> CancelReservation(int id)
        {
            // ✅ منع إلغاء الحجز لو الدكتور كتب ملاحظات (يعني المريض اتكشف فعلاً)
            var reservation = await _reservationService.GetReservationWithDetailsAsync(id);

            if (reservation == null)
                return Json(new { success = false, message = "الحجز غير موجود." });

            var existingNote = await _patientNotesService.GetByReservationAsync(id);

            if (existingNote != null)
                return Json(new
                {
                    success = false,
                    message = "لا يمكن إلغاء الحجز لأن الطبيب قد قام بإضافة تفاصيل الكشف. المريض تمت زيارته بالفعل."
                });

            var success = await _reservationService.CancelReservationAsync(id);

            if (!success)
            {
                return Json(new
                {
                    success = false,
                    message = "فشل إلغاء الحجز."
                });
            }

            return Json(new
            {
                success = true,
                message = "تم إلغاء الحجز بنجاح."
            });
        }
        [HttpGet]
        public async Task<IActionResult> GetDoctorsSchedules()
        {
            var schedules = await _scheduleService.GetAllWithDoctorsAsync();

            var sortedSchedules = schedules
                .OrderBy(s => s.Doctor.Speciality.Name)
                .ThenBy(s => s.Doctor.User.FirstName)
                .ThenBy(s => s.WeekDay)
                .ThenBy(s => s.StartTime);

            return Json(new
            {
                success = true,
                data = sortedSchedules.Select(s => new
                {
                    doctor = $"{s.Doctor.User.FirstName} {s.Doctor.User.LastName}",
                    speciality = s.Doctor.Speciality.Name,
                    day = s.WeekDay,
                    start = s.StartTime.ToString(@"hh\:mm"),
                    end = s.EndTime.ToString(@"hh\:mm"),
                })
            });
        }
        [HttpGet]
public async Task<IActionResult> GetPatientCheckups()
{
    int? userId = HttpContext.Session.GetInt32("UserId");

    if (userId == null)
        return Json(new { success = false, message = "Unauthorized access." });

    
var patient =
    await _patientService.GetByUserIdAsync(userId.Value);

            if (patient == null)
        return Json(new { success = false, message = "Patient not found." });

            var notes =
              await _patientNotesService.GetPatientHistoryAsync(patient.PatientId);

            return Json(new
            {
                success = true,
                data = notes.Select(x => new
                {
                    speciality = x.Reservation.Doctor.Speciality.Name,
                    diagnosis = x.Diagnosis,
                    notes = x.Notes,
                    date = x.VisitDate.ToString("yyyy-MM-dd")
                })
            });
}
    }
}