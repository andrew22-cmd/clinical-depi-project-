using clinicsystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace clinicsystem.Controllers
{
    public class SpecialitiesController : Controller
    {
        private readonly AppDbContext _db;
        public SpecialitiesController(AppDbContext db) => _db = db;

        // GET /Specialities
        public async Task<IActionResult> Index()
        {
            var items = await _db.Specialities
                .Include(s => s.Doctors)
                .OrderBy(s => s.Name)
                .ToListAsync();
            return View(items);
        }

        // GET /Specialities/Create
        public IActionResult Create() => View(new Speciality());

        // POST /Specialities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Speciality model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ModelState.AddModelError(nameof(model.Name), "اسم التخصص مطلوب.");
                return View(model);
            }
            bool exists = await _db.Specialities.AnyAsync(s => s.Name == model.Name.Trim());
            if (exists)
            {
                ModelState.AddModelError(nameof(model.Name), "هذا التخصص موجود بالفعل.");
                return View(model);
            }
            model.Name = model.Name.Trim();
            _db.Specialities.Add(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET /Specialities/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _db.Specialities.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        // POST /Specialities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Speciality model)
        {
            if (id != model.SpecialityId) return NotFound();
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ModelState.AddModelError(nameof(model.Name), "اسم التخصص مطلوب.");
                return View(model);
            }
            var item = await _db.Specialities.FindAsync(id);
            if (item == null) return NotFound();
            item.Name = model.Name.Trim();
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST /Specialities/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _db.Specialities
                .Include(s => s.Doctors)
                .FirstOrDefaultAsync(s => s.SpecialityId == id);
            if (item == null) return NotFound();

            if (item.Doctors != null && item.Doctors.Any())
            {
                TempData["Error"] = "لا يمكن حذف تخصص مرتبط بأطباء. انقل الأطباء أو احذفهم أولاً.";
                return RedirectToAction(nameof(Index));
            }
            _db.Specialities.Remove(item);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
