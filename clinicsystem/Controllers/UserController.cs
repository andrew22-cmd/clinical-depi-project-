using clinicsystem.Data;
using clinicsystem.Models;
using clinicsystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace clinicsystem.Controllers
{
    // Only Managers can manage user accounts
    [Authorize(Roles = "Manager")]
    public class UserController : Controller
    {
        private readonly UserManager<User>  _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(
            UserManager<User>       userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // ── GET /User/Index ──────────────────────────────────────────────────
        // Supports: ?search=term  &role=Patient  &page=2
        public async Task<IActionResult> Index(
            string? search, string? role, int page = 1)
        {
            const int pageSize = 10;

            var allUsers = _userManager.Users.AsQueryable();

            // Filter by search term (first name, last name, or email)
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower();
                allUsers = allUsers.Where(u =>
                    u.FirstName.ToLower().Contains(s) ||
                    u.LastName.ToLower().Contains(s)  ||
                    (u.Email != null && u.Email.ToLower().Contains(s)));
            }

            // Filter by role (Identity role, not Users.Role string)
            List<User> userList;
            if (!string.IsNullOrWhiteSpace(role))
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(role);
                var ids = usersInRole.Select(u => u.Id).ToHashSet();
                userList = allUsers.Where(u => ids.Contains(u.Id)).ToList();
            }
            else
            {
                userList = allUsers.ToList();
            }

            // Pagination
            int totalCount = userList.Count;
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));

            var paged = userList
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Map to ViewModels (include Identity role per user)
            var vmList = new List<UserViewModel>();
            foreach (var u in paged)
            {
                var roles = await _userManager.GetRolesAsync(u);
                vmList.Add(new UserViewModel
                {
                    Id        = u.Id,
                    FirstName = u.FirstName,
                    LastName  = u.LastName,
                    Email     = u.Email    ?? string.Empty,
                    Phone     = u.PhoneNumber ?? string.Empty,
                    Role      = roles.FirstOrDefault() ?? u.Role
                });
            }

            // Build roles dropdown for filter
            ViewBag.Roles = new SelectList(new[]
            {
                UserRoles.Manager,
                UserRoles.Doctor,
                UserRoles.Secretary,
                UserRoles.Patient
            });

            var vm = new UserIndexViewModel
            {
                Users       = vmList,
                SearchTerm  = search,
                FilterRole  = role,
                CurrentPage = page,
                TotalPages  = totalPages,
                PageSize    = pageSize
            };

            return View(vm);
        }

        // ── GET /User/Details/id ─────────────────────────────────────────────
        public async Task<IActionResult> Details(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            var vm = new UserViewModel
            {
                Id        = user.Id,
                FirstName = user.FirstName,
                LastName  = user.LastName,
                Email     = user.Email         ?? string.Empty,
                Phone     = user.PhoneNumber   ?? string.Empty,
                Role      = roles.FirstOrDefault() ?? user.Role
            };

            return View(vm);
        }

        // ── GET /User/Create ─────────────────────────────────────────────────
        public IActionResult Create()
        {
            ViewBag.Roles = RolesSelectList();
            return View(new UserViewModel());
        }

        // ── POST /User/Create ────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserViewModel model)
        {
            // Password is required on Create
            if (string.IsNullOrWhiteSpace(model.Password))
                ModelState.AddModelError("Password", "كلمة المرور مطلوبة عند إنشاء مستخدم جديد.");

            if (!ModelState.IsValid)
            {
                ViewBag.Roles = RolesSelectList();
                return View(model);
            }

            var user = new User
            {
                FirstName      = model.FirstName,
                LastName       = model.LastName,
                UserName       = model.Email,
                Email          = model.Email,
                PhoneNumber    = model.Phone,
                EmailConfirmed = true,
                Role           = model.Role
            };

            var result = await _userManager.CreateAsync(user, model.Password!);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Role);
                TempData["Success"] = $"تم إنشاء المستخدم {user.FullName} بنجاح.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            ViewBag.Roles = RolesSelectList();
            return View(model);
        }

        // ── GET /User/Edit/id ────────────────────────────────────────────────
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            var vm = new UserViewModel
            {
                Id        = user.Id,
                FirstName = user.FirstName,
                LastName  = user.LastName,
                Email     = user.Email       ?? string.Empty,
                Phone     = user.PhoneNumber ?? string.Empty,
                Role      = roles.FirstOrDefault() ?? user.Role
            };

            ViewBag.Roles = RolesSelectList();
            return View(vm);
        }

        // ── POST /User/Edit ──────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserViewModel model)
        {
            // On Edit, password is optional — clear its errors if blank
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.Remove("Password");
                ModelState.Remove("ConfirmPassword");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Roles = RolesSelectList();
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id!);
            if (user == null) return NotFound();

            // Update basic fields
            user.FirstName   = model.FirstName;
            user.LastName    = model.LastName;
            user.Email       = model.Email;
            user.UserName    = model.Email;   // keep UserName == Email
            user.PhoneNumber = model.Phone;
            user.Role        = model.Role;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var e in updateResult.Errors)
                    ModelState.AddModelError(string.Empty, e.Description);
                ViewBag.Roles = RolesSelectList();
                return View(model);
            }

            // Update password only if provided
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var pwResult = await _userManager.ResetPasswordAsync(user, token, model.Password);
                if (!pwResult.Succeeded)
                {
                    foreach (var e in pwResult.Errors)
                        ModelState.AddModelError(string.Empty, e.Description);
                    ViewBag.Roles = RolesSelectList();
                    return View(model);
                }
            }

            // Update Identity role
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, model.Role);

            TempData["Success"] = $"تم تحديث بيانات {user.FullName} بنجاح.";
            return RedirectToAction(nameof(Index));
        }

        // ── GET /User/Delete/id ──────────────────────────────────────────────
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            var vm = new UserViewModel
            {
                Id        = user.Id,
                FirstName = user.FirstName,
                LastName  = user.LastName,
                Email     = user.Email       ?? string.Empty,
                Phone     = user.PhoneNumber ?? string.Empty,
                Role      = roles.FirstOrDefault() ?? user.Role
            };

            return View(vm);
        }

        // ── POST /User/DeleteConfirmed ───────────────────────────────────────
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var name = user.FullName;
            await _userManager.DeleteAsync(user);

            TempData["Success"] = $"تم حذف المستخدم {name} بنجاح.";
            return RedirectToAction(nameof(Index));
        }

        // ── Private helper ───────────────────────────────────────────────────
        private static SelectList RolesSelectList() =>
            new SelectList(new[]
            {
                UserRoles.Manager,
                UserRoles.Doctor,
                UserRoles.Secretary,
                UserRoles.Patient
            });
    }
}
