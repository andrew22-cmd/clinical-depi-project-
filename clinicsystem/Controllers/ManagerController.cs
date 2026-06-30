//using Microsoft.AspNetCore.Mvc;

//namespace clinicsystem.Controllers
//{
//    public class ManagerController : Controller
//    {
//        public IActionResult Index()
//        {
//            return View();
//        }
//    }
//}
using Microsoft.AspNetCore.Mvc;

namespace clinicsystem.Controllers
{
    public class ManagerController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult ManageUsers()
        {
            return View();
        }

        public IActionResult ViewReservations()
        {
            return View();
        }

        public IActionResult AddStaff()
        {
            return View();
        }
    }
}