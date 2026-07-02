using Microsoft.AspNetCore.Mvc;

namespace clinicsystem.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
