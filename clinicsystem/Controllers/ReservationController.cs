using Microsoft.AspNetCore.Mvc;

namespace clinicsystem.Controllers
{
    public class ReservationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Info()
        {
            return View();
        }

        public IActionResult EmailNotification()
        {
            return View();
        }
    }
}