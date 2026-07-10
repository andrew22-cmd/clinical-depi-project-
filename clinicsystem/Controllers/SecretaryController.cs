using clinicsystem.Models;
using clinicsystem.Services.Implementations;
using clinicsystem.Services.Interfaces;
using clinicsystem.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace clinicsystem.Controllers
{
    public class SecretaryController : Controller
    {
        private readonly ISpecialityService _specialityService;
        private readonly IDoctorService _doctorService;
        private IDoctorScheduleService _scheduleService;
        private IDoctorScheduleSlotService _slotService;
private readonly IReservationService _reservationService;
        private readonly IPatientService _patientService;
        private readonly IPatientNotesService _patientNotesService;

        public SecretaryController(
            ISpecialityService specialityService,
            IDoctorService doctorService,
            IDoctorScheduleService scheduleService,
            IDoctorScheduleSlotService slotService,
            IReservationService reservationService,
            IPatientService patientService,
            IPatientNotesService patientNotesService)
        {
            _specialityService = specialityService;
            _doctorService = doctorService;
            _scheduleService = scheduleService;
            _slotService = slotService;
            _reservationService = reservationService;
            _patientService = patientService;
            _patientNotesService = patientNotesService;
        }
        public IActionResult DashBoard()
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
        public async Task<IActionResult> GetDoctorsBySpeciality(int specialityId)
        {
            var doctors = await _doctorService.GetBySpecialityAsync(specialityId);

            return Json(new
            {
                success = true,
                data = doctors.Select(d => new
                {
                    id = d.DoctorId,
                    name = d.User.FirstName + " " + d.User.LastName
                })
            });
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
        public class SecretaryReservationDto
        {
            public string PatientName { get; set; }

            public string PatientPhone { get; set; }

            public int DoctorId { get; set; }

            public int SlotId { get; set; }

            public DateTime ReservationDate { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> CreateReservation(
    [FromBody] SecretaryReservationDto model)
        {
            var patient =
                await _patientService.GetByPhoneAsync(model.PatientPhone);

            if (patient == null)
            {
                patient =
                    await _patientService.CreateOfflinePatientAsync(
                        model.PatientName,
                        model.PatientPhone);
            }

            var reservation = new Reservation
            {
                PatientId = patient.PatientId,

                DoctorId = model.DoctorId,

                SlotId = model.SlotId,

                ReservationDate = model.ReservationDate,

                Status = "Confirmed",

                Source = "Offline",

                CreatedAt = DateTime.Now
            };

            var success =
                await _reservationService.CreateReservationAsync(reservation);

            if (!success)
            {
                return Json(new
                {
                    success = false,
                    message = "الموعد محجوز بالفعل."
                });
            }

            return Json(new
            {
                success = true,
                message = "تم الحجز بنجاح."
            });
        }
        [HttpGet]
        public async Task<IActionResult> GetReservations()
        {

            var allData = await _reservationService.GetAllAsync();
            var data = allData
                .Where(x => x.ReservationDate.Date >= DateTime.Today)
                .OrderBy(x => x.ReservationDate.Date)
                .ThenBy(x => x.Slot?.SlotTime)
                .ToList();

            return Json(new
            {
                success = true,
                data = data.Select(x => new
                {
                    id = x.ReservationId,
                    patient = x.Patient.FullName,
                    phone = x.Patient.Phone,
                    doctor = x.Doctor.User.FirstName + " " + x.Doctor.User.LastName,
                    date = x.ReservationDate.ToString("dd-MM-yyyy"),
                    day = x.Slot.Schedule.WeekDay,
                    time = x.Slot.SlotTime.ToString(@"hh\:mm"),
                    source = x.Source,
                    status = x.Status
                })
            });

        }
        [HttpDelete]
        public async Task<IActionResult> CancelReservation(int id)
        {
            var reservation = await _reservationService.GetReservationWithDetailsAsync(id);

            if (reservation == null)
            {
                return Json(new { success = false, message = "الحجز غير موجود." });
            }

            // ✅ منع الإلغاء إذا تم إضافة كشف بواسطة الطبيب
            var existingNote = await _patientNotesService.GetByReservationAsync(id);
            if (existingNote != null)
            {
                return Json(new
                {
                    success = false,
                    message = "لا يمكن إلغاء الحجز لأن الطبيب قد قام بإضافة تفاصيل الكشف. المريض تمت زيارته بالفعل."
                });
            }

            var success = await _reservationService.CancelReservationAsync(id);

            if (!success)
            {
                return Json(new { success = false, message = "حدث خطأ أثناء إلغاء الحجز." });
            }

            return Json(new { success = true, message = "تم إلغاء الحجز." });
        }
        [HttpGet]
        public async Task<IActionResult> GetDashboardStatistics()
        {
            var reservations = await _reservationService.GetAllAsync();

            return Json(new
            {
                success = true,
                data = new
                {
                    totalReservations = reservations.Count(),
                    todayReservations = reservations.Count(x =>
                        x.ReservationDate.Date == DateTime.Today)
                }
            });
        }
        [HttpGet]
        public async Task<IActionResult> GetDoctorsSchedules()
        {
            var schedules = await _scheduleService.GetAllWithDoctorsAsync();

            return Json(new
            {
                success = true,
                data = schedules.Select(x => new
                {
                    doctor = x.Doctor.User.FirstName + " " + x.Doctor.User.LastName,
                    speciality = x.Doctor?.Speciality?.Name ?? "",
                    day = x.WeekDay,
                    start = x.StartTime.ToString(@"hh\:mm"),
                    end = x.EndTime.ToString(@"hh\:mm")
                })
            });
        }
    }
}