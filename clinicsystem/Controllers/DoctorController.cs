using clinicsystem.Services.Implementations;
using clinicsystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using clinicsystem.Models;
using clinicsystem.ViewModels;
namespace clinicsystem.Controllers
{
    public class DoctorController : Controller
    {
        private readonly IDoctorService _doctorService;
        private readonly IDoctorScheduleService _doctorScheduleService;
        private readonly IUserService _userService;
        private readonly IReservationService _reservationService;
        private readonly IPatientNotesService _patientNotesService;

        public DoctorController(
    IDoctorService doctorService,
    IDoctorScheduleService doctorScheduleService,
    IUserService userService,
    IReservationService reservationService, 
    IPatientNotesService patientNotesService)
        {
            _doctorService = doctorService;
            _doctorScheduleService = doctorScheduleService;
            _userService = userService;
            _reservationService = reservationService;
            _patientNotesService = patientNotesService;
        }
        public IActionResult DashBoard()
        {
            return View();
        }

        public IActionResult PatientDetails()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetDoctorInfo()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return Json(new { success = false, message = "Unauthorized access." });

            var doctor = await _doctorService.GetDoctorByUserIdAsync(userId.Value);

            if (doctor == null)
                return Json(new { success = false, message = "Doctor not found." });

            return Json(new
            {
                success = true,
                message = "Data retrieved successfully",
                name = $"د. {doctor.User.FirstName} {doctor.User.LastName}",
                speciality = doctor.Speciality.Name
            });
        }
        [HttpGet]
        public async Task<IActionResult> GetWeeklySchedule()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return Json(new { success = false, message = "Unauthorized access." });

            var doctor = await _doctorService.GetDoctorByUserIdAsync(userId.Value);

            if (doctor == null)
                return Json(new { success = false, message = "Doctor not found." });

            var schedules = await _doctorScheduleService.GetDoctorSchedulesAsync(doctor.DoctorId);

            var result = schedules.Select(s => new
            {
                day = s.WeekDay,
                start = DateTime.Today.Add(s.StartTime).ToString("hh:mm tt"),
                end = DateTime.Today.Add(s.EndTime).ToString("hh:mm tt")
            });

            return Json(new { success = true, data = result });
        }
        [HttpGet]
        public async Task<IActionResult> GetTodayPatients()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return Json(new { success = false, message = "Unauthorized access." });

            var doctor = await _doctorService.GetDoctorByUserIdAsync(userId.Value);

            if (doctor == null)
                return Json(new { success = false, message = "Doctor not found." });

            var reservations =
                await _reservationService.GetDoctorTodayReservationsAsync(doctor.DoctorId);

            return Json(new
            {
                success = true,
                data = reservations.Select(x => new
                {
                    patient = x.Patient.FullName,
                    phone = x.Patient.Phone,
                    time = $"{x.Slot.SlotTime.Hours:D2}:{x.Slot.SlotTime.Minutes:D2}"
                })
            });

        }

        [HttpGet]
        public async Task<IActionResult> GetPatientsForNotes()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return Json(new { success = false, message = "Unauthorized access." });

            var doctor = await _doctorService.GetDoctorByUserIdAsync(userId.Value);

            if (doctor == null)
                return Json(new { success = false, message = "Doctor not found." });

            var reservations =
                await _reservationService.GetDoctorReservationsForNotesAsync(doctor.DoctorId);

            return Json(new
            {
                success = true,
                data = reservations
                .GroupBy(x => x.PatientId)
                .Select(g => g.First())
                .Select(r => new
                {
                    reservationId = r.ReservationId,
                    patient = r.Patient.FullName
                })
            });
        }
        [HttpPost]
        public async Task<IActionResult> SavePatientNote(
    [FromBody] SavePatientNoteVM model)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return Json(new { success = false, message = "Unauthorized access." });

            var doctor =
                await _doctorService.GetDoctorByUserIdAsync(userId.Value);

            if (doctor == null)
                return Json(new { success = false, message = "الطبيب غير موجود." });

            var reservation =
                await _reservationService.GetReservationWithDetailsAsync(model.ReservationId);

            if (reservation == null)
                return Json(new { success = false, message = "الموعد غير موجود." });

            var note = new PatientNotes
            {
                ReservationId = reservation.ReservationId,
                PatientId = reservation.PatientId,
                DoctorId = doctor.DoctorId,

                Diagnosis = model.Diagnosis,
                Notes = model.Notes,

                VisitDate = model.VisitDate,
                CreatedAt = DateTime.Now
            };

            await _patientNotesService.AddNoteAsync(note);

            return Json(new
            {
                success = true,
                message = "تم حفظ الكشف"
            });
        }
    }

}