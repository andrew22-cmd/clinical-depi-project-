using clinicsystem.Models;
using clinicsystem.Services.Implementations;
using clinicsystem.Services.Interfaces;
using clinicsystem.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace clinicsystem.Controllers
{
    public class ManagerController : Controller
    {
     
        private readonly IDoctorService _doctorService;
        private readonly ISpecialityService _specialityService;
        private readonly IUserService _userService;
        private readonly IDoctorScheduleService _doctorScheduleService;
        private readonly IDoctorScheduleSlotService _doctorScheduleSlotService;
        private readonly IReservationService _ReservationService;
        public ManagerController(
                    IDoctorService doctorService,
                    ISpecialityService specialityService,
                    IUserService userService,
                    IDoctorScheduleService doctorScheduleService,
                    IDoctorScheduleSlotService doctorScheduleSlotService,IReservationService reservationService )
        {
            _doctorService = doctorService;
            _specialityService = specialityService;
            _userService = userService;
            _doctorScheduleService = doctorScheduleService;
            _doctorScheduleSlotService = doctorScheduleSlotService;
            _ReservationService = reservationService;
        }
        

        // ================= Views =================

        public IActionResult DashBoard()
        {
            return View();
        }
        public IActionResult ManageUsers() => View();
        public IActionResult DoctorsSchedules() => View();
        public IActionResult ViewReservations() => View();
        public IActionResult Reports() => View();
        public IActionResult AddSchedule() => View();
        public IActionResult EditSchedule() => View();

        [HttpGet]
        public async Task<IActionResult> AddStaff()
        {
            var viewModel = new AddStaffViewModel
            {
                Specialities = await _specialityService.GetAllAsync()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddStaff(AddStaffViewModel model)
        {
            ModelState.Remove("User");
            ModelState.Remove("Doctor");
            ModelState.Remove("Patient");
            ModelState.Remove("Specialities");

            if (model.Role == "Doctor" && !model.SpecialityId.HasValue)
            {
                ModelState.AddModelError("SpecialityId", "Field required for doctors");
            }

            if (ModelState.IsValid)
            {
                var newUser = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Phone = model.Phone,
                    Password = model.Password,
                    Role = model.Role
                };

                bool isSaved = false;

                if (model.Role == "Doctor")
                {
                    var newDoctor = new Doctor
                    {
                        User = newUser,
                        SpecialityId = model.SpecialityId.Value
                    };
                    await _doctorService.AddDoctorAsync(newDoctor);
                    TempData["Success"] = "Doctor added successfully.";
                    isSaved = true;
                }
                else
                {
                    isSaved = await _userService.RegisterAsync(newUser);
                    if (!isSaved)
                    {
                        TempData["Error"] = "Add staff failed. Email or Phone already exists.";
                        ModelState.AddModelError("", "Email or Phone already exists");
                        model.Specialities = await _specialityService.GetAllAsync() ?? new List<Speciality>();
                        return View(model);
                    }
                    TempData["Success"] = "User created successfully.";
                }
                return RedirectToAction(nameof(DashBoard));
            }

            TempData["Warning"] = "Please correct the validation errors.";
            model.Specialities = await _specialityService.GetAllAsync() ?? new List<Speciality>();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditStaff(int id, string role)
        {
            var viewModel = new EditStaffViewModel
            {
                Specialities = await _specialityService.GetAllAsync() ?? new List<Speciality>()
            };

            if (role == "Doctor" || role == "طبيب")
            {
                var doctor = await _doctorService.GetByIdAsync(id);
                if (doctor == null || doctor.User == null) return NotFound();
                viewModel.Id = doctor.DoctorId;
                viewModel.FirstName = doctor.User.FirstName;
                viewModel.LastName = doctor.User.LastName;
                viewModel.Email = doctor.User.Email;
                viewModel.Phone = doctor.User.Phone;
                viewModel.Role = "Doctor";
                viewModel.SpecialityId = doctor.SpecialityId;
            }
            else
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null) return NotFound();
                viewModel.Id = user.UserId;
                viewModel.FirstName = user.FirstName;
                viewModel.LastName = user.LastName;
                viewModel.Email = user.Email;
                viewModel.Phone = user.Phone;
                viewModel.Role = "Secretary";
            }
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStaff(EditStaffViewModel model)
        {
            ModelState.Remove("Specialities");
            ModelState.Remove("Role");

            if (string.IsNullOrEmpty(model.FirstName) || string.IsNullOrEmpty(model.LastName))
            {
                TempData["Warning"] = "Basic fields are required.";
                ModelState.AddModelError("", "Basic fields are required");
                model.Specialities = await _specialityService.GetAllAsync() ?? new List<Speciality>();
                return View(model);
            }

            var doctorsList = await _doctorService.GetAllWithDetailsAsync() ?? new List<Doctor>();
            var doctor = doctorsList.FirstOrDefault(d => d.DoctorId == model.Id);

            if (doctor != null)
            {
                if (doctor.User != null)
                {
                    doctor.User.FirstName = model.FirstName;
                    doctor.User.LastName = model.LastName;
                    doctor.User.Email = model.Email;
                    doctor.User.Phone = model.Phone;
                }
                if (model.SpecialityId.HasValue) doctor.SpecialityId = model.SpecialityId.Value;
                await _doctorService.UpdateDoctorAsync(doctor);
                TempData["Success"] = "Doctor updated successfully.";
            }
            else
            {
                var user = await _userService.GetUserByIdAsync(model.Id);
                if (user != null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;
                    user.Phone = model.Phone;
                    await _userService.UpdateUserAsync(user);
                    TempData["Success"] = "User updated successfully.";
                }
            }
            return RedirectToAction(nameof(DashBoard));
        }

        // ================= APIs =================

        [HttpGet]
        public async Task<IActionResult> GetDoctorsList()
        {
            var doctors = await _doctorService.GetAllWithDetailsAsync() ?? new List<Doctor>();
            var allUsers = await _userService.GetAllUsersAsync() ?? new List<User>();
            var secretaries = allUsers.Where(u => u.Role == "Secretary").ToList();

            var staffList = doctors.Select(d => new
            {
                id = d.DoctorId,
                name = d.User != null ? $"{d.User.FirstName} {d.User.LastName}" : "Unknown",
                role = "Doctor",
                speciality = d.Speciality != null ? d.Speciality.Name : "General"
            }).Concat(secretaries.Select(s => new
            {
                id = s.UserId,
                name = $"{s.FirstName} {s.LastName}",
                role = "Secretary",
                speciality = "—"
            })).ToList();

            return Json(new { success = true, data = staffList });
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboardStats()
        {
            var allUsers = await _userService.GetAllUsersAsync() ?? new List<User>();
            var allDoctors = await _doctorService.GetAllAsync() ?? new List<Doctor>();

            var stats = new
            {
                totalUsers = allUsers.Count(),
                totalDoctors = allDoctors.Count(),
                totalSecretaries = allUsers.Count(u => u.Role == "Secretary" || u.Role == "سكرتارية"),
                weeklyReservations = 12,
                occupancy = 75,
                doctorsToday = allDoctors.Count()
            };
            return Json(new { success = true, data = stats });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsersList()
        {
            try
            {
                var users = (await _userService.GetAllUsersAsync())
            ?.Where(u => u.Role != "Manager" &&
                         u.Role != "مدير")
            .ToList();

                if (users == null) return Json(new { success = false, message = "No users found" });
                var result = users.Select(u => new
                {
                    id = u.UserId,
                    name = $"{u.FirstName} {u.LastName}",
                    email = u.Email,
                    phone = u.Phone ?? "—",
                    role = u.Role switch
                    {
                        "Doctor" => "طبيب",
                        "Secretary" => "سكرتارية",
                        "Manager" => "مدير",
                        "Patient" => "مريض",
                        _ => u.Role
                    },
                    status = "نشط"
                }).ToList();

                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                // تسجيل الخطأ لمعرفته
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteStaff(int id, string role)
        {
            try
            {
                bool result;

                if (role == "Doctor" || role == "طبيب")
                {
                    result = await _doctorService.DeleteDoctorAsync(id);
                }
                else
                {
                    result = await _userService.DeleteUserAsync(id);
                }

                return result ? Json(new { success = true, message = "تم الحذف بنجاح." }) : Json(new { success = false, message = "فشل الحذف." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "حدث خطأ أثناء الحذف." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDoctorsListOnly()
        {
            try
            {
                var doctors = await _doctorService.GetAllWithDetailsAsync() ?? new List<Doctor>();
                var result = doctors.Select(d => new
                {
                    id = d.DoctorId,
                    name = d.User != null ? $"د. {d.User.FirstName} {d.User.LastName}" : $"طبيب {d.DoctorId}"
                }).ToList();
                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveSchedule([FromBody] ScheduleModel model)
        {

            TimeSpan newStart = TimeSpan.Parse(model.StartTime);
            TimeSpan newEnd = TimeSpan.Parse(model.EndTime);

            var schedules = await _doctorScheduleService.GetDoctorSchedulesAsync(model.DoctorId);

            foreach (var schedule in schedules.Where(s => s.WeekDay == model.Day))
            {
                if (schedule.Slots == null || !schedule.Slots.Any())
                    continue;

                TimeSpan oldStart = schedule.Slots.Min(x => x.SlotTime);
                TimeSpan oldEnd = schedule.Slots.Max(x => x.SlotTime).Add(TimeSpan.FromMinutes(30));

                if (newStart < oldEnd && newEnd > oldStart)
                {
                    return Json(new
                    {
                        success = false,
                        message = $"يوجد معاد بالفعل من {oldStart:hh\\:mm} إلى {oldEnd:hh\\:mm}"
                    });
                }
            }



            // إنشاء الجدول
            var newSchedule = new DoctorSchedule
            {
                DoctorId = model.DoctorId,
                WeekDay = model.Day,
                StartTime = newStart,
                EndTime = newEnd,
                IsActive = true
            };

            await _doctorScheduleService.AddScheduleAsync(newSchedule);

            // إنشاء الـ Slots كل نصف ساعة
            for (TimeSpan time = newStart; time < newEnd; time += TimeSpan.FromMinutes(30))
            {
                await _doctorScheduleSlotService.AddSlotAsync(new DoctorScheduleSlot
                {
                    ScheduleId = newSchedule.ScheduleId,
                    SlotTime = time,
                    IsBooked = false,
                    CreatedAt = DateTime.Now
                });
            }

            return Json(new
            {
                success = true,
                message = "تم حفظ الجدول بنجاح"
            });
        }
        [HttpGet]
        public async Task<IActionResult> GetSchedules()
        {
            try
            {
                var schedules = await _doctorScheduleService.GetAllWithDetailsAsync();
                var result = (schedules ?? new List<DoctorSchedule>()).Select(sch => new
                {
                    id = sch.ScheduleId,
                    doctorName = sch.Doctor?.User != null
                        ? $"د. {sch.Doctor.User.FirstName} {sch.Doctor.User.LastName}"
                        : "طبيب غير معروف",
                    day = sch.WeekDay,
                    startTime = sch.StartTime.ToString(@"hh\:mm"),
                    endTime = sch.EndTime.ToString(@"hh\:mm"),
                }).ToList();

                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            var result = await _doctorScheduleService.DeleteScheduleAsync(id);

            if (!result)
            {
                return Json(new
                {
                    success = false,
                    message = "لم يتم العثور على الميعاد."
                });
            }

            return Json(new
            {
                success = true,
                message = "تم حذف الميعاد بنجاح."
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetReservationStats()
        {
            try
            {
                var all = (await _ReservationService.GetAllAsync() ?? new List<Reservation>())
                          .Where(r => r.Status == "Completed" || r.Status == "Confirmed")
                          .ToList();

                var today = DateTime.Today;
                var startOfMonth = new DateTime(today.Year, today.Month, 1);

                decimal GetFee(Reservation r) =>
                    r.Doctor?.Speciality?.ConsultationFee ?? r.Doctor?.ConsultationFee ?? 0;

                var resToday  = all.Count(r => r.ReservationDate.Date == today);
                var resMonth  = all.Count(r => r.ReservationDate.Date >= startOfMonth);
                var revToday  = all.Where(r => r.ReservationDate.Date == today).Sum(GetFee);
                var revMonth  = all.Where(r => r.ReservationDate.Date >= startOfMonth).Sum(GetFee);

                // حجوزات كل تخصص
                var bySpeciality = all
                    .Where(r => r.Doctor?.Speciality != null)
                    .GroupBy(r => r.Doctor.Speciality.Name)
                    .Select(g => new
                    {
                        speciality = g.Key,
                        fee        = g.First().Doctor.Speciality.ConsultationFee,
                        count      = g.Count(),
                        revenue    = g.Sum(GetFee)
                    })
                    .OrderByDescending(x => x.count)
                    .ToList();

                return Json(new
                {
                    success    = true,
                    resToday   = resToday,
                    resMonth   = resMonth,
                    revToday   = revToday.ToString("N2"),
                    revMonth   = revMonth.ToString("N2"),
                    bySpeciality = bySpeciality
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public class ScheduleModel
        {
            public int DoctorId { get; set; }
            public string Day { get; set; }
            public string StartTime { get; set; }
            public string EndTime { get; set; }
            public TimeSpan SlotTime { get; set; }
        }
        [HttpGet]
        public async Task<IActionResult> GetReservationsByDate(DateTime date)
        {
            var reservations = await _ReservationService.GetByDateAsync(date);

            return Json(new
            {
                success = true,
                data = reservations.Select(r => new
                {
                    patient = r.Patient.FullName,
                    phone = r.Patient.Phone,
                    doctor = $"{r.Doctor.User.FirstName} {r.Doctor.User.LastName}",
                    day = r.Slot.Schedule.WeekDay,
                    time = r.Slot.SlotTime.ToString(@"hh\:mm"),
                    source = r.Source,
                    status = r.Status
                })
            });
        }
    }
}