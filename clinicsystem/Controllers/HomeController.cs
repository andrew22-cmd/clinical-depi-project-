using System.Diagnostics;
using clinicsystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace clinicsystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Landing page (default route "/")
        public IActionResult Index()
        {
            return View();
        }

        // ---- Authentication (prototype) ----
        [Route("login")]
        public IActionResult Login() => View();

        [Route("register")]
        public IActionResult Register() => View();

        [Route("email-notification")]
        public IActionResult EmailNotification() => View();

        // ---- Patient / user (prototype dashboards) ----
        [Route("user-dashboard")]
        public IActionResult UserDashboard() => View();

        [Route("user-details")]
        public IActionResult UserDetails() => View();

        [Route("reservation-info")]
        public IActionResult ReservationInfo() => View();

        // ---- Doctor (prototype) ----
        [Route("doctor-dashboard")]
        public IActionResult DoctorDashboard() => View();

        [Route("doctor-patient-details")]
        public IActionResult DoctorPatientDetails() => View();

        // ---- Manager (prototype) ----
        [Route("manager-dashboard")]
        public IActionResult ManagerDashboard() => View();

        [Route("manage-users")]
        public IActionResult ManageUsers() => View();

        [Route("manager-add-staff")]
        public IActionResult ManagerAddStaff() => View();

        [Route("view-reservations")]
        public IActionResult ViewReservations() => View();

        // ---- Secretary (prototype) ----
        [Route("secretary-dashboard")]
        public IActionResult SecretaryDashboard() => View();

        // NOTE: doctors-schedules, add-schedule, edit-schedule and reservation
        // are now served by DoctorsController / ReservationsController (DB-backed).

        // ---- Default template pages ----
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
