using Microsoft.AspNetCore.Mvc;

namespace clinicsystem.Controllers
{
    public class PatientController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult Details()
        {
            return View();
        }
    }
}