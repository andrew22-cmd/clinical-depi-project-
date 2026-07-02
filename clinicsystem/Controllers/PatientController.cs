using clinicsystem.Models;
using clinicsystem.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace clinicsystem.Controllers
{
    public class PatientController : Controller
    {
        private readonly UserManager<User> _userManager;

        public PatientController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        // ── GET /Patient/Dashboard ────────────────────────────────────────────
        public async Task<IActionResult> Dashboard()
        {
            var model = await BuildProfileAsync();
            return View(model);
        }

        // ── GET /Patient/Details ──────────────────────────────────────────────
        public async Task<IActionResult> Details()
        {
            var model = await BuildProfileAsync();
            return View(model);
        }

        // ── Shared helper ─────────────────────────────────────────────────────
        // Resolve the current Identity user and map it to the ViewModel.
        private async Task<UserViewModel> BuildProfileAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            return UserViewModel.FromUser(user);
        }
    }
}