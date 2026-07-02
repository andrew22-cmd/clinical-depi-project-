
using Microsoft.AspNetCore.Mvc;

namespace clinicsystem.Controllers
{
    public class DoctorController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult PatientDetails()
        {
            return View();
        }
    }
}