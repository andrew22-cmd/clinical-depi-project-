using Microsoft.AspNetCore.Mvc;

namespace clinicsystem.Controllers
{
    public class SecretaryController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}