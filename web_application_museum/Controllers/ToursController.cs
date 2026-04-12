using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuseumWebApp.Data;
using MuseumWebApp.Models;

namespace MuseumWebApp.Controllers
{
    public class ToursController : Controller
    {
        private readonly MuseumDbContext _context;

        public ToursController(MuseumDbContext context)
        {
            _context = context;
        }

        // Доступно всем
        public async Task<IActionResult> Index()
        {
            return View(await _context.Tours.ToListAsync());
        }

        // Доступно всем
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var tour = await _context.Tours.FirstOrDefaultAsync(m => m.Id == id);
            return tour == null ? NotFound() : View(tour);
        }

        [Authorize(Roles = "Admin,Employee")]
        public IActionResult Create() => View();

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Price")] Tour tour, int durationHours)
        {
            if (durationHours <= 0)
                ModelState.AddModelError("durationHours", "Укажите длительность больше 0 часов.");

            if (ModelState.IsValid)
            {
                tour.Duration = TimeSpan.FromHours(durationHours);
                _context.Add(tour);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.DurationHours = durationHours;
            return View(tour);
        }

        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var tour = await _context.Tours.FindAsync(id);
            return tour == null ? NotFound() : View(tour);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Price")] Tour tour, int durationHours)
        {
            if (id != tour.Id) return NotFound();
            if (durationHours <= 0)
                ModelState.AddModelError("durationHours", "Укажите длительность больше 0 часов.");

            if (ModelState.IsValid)
            {
                try
                {
                    tour.Duration = TimeSpan.FromHours(durationHours);
                    _context.Update(tour);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TourExists(tour.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.DurationHours = durationHours;
            return View(tour);
        }

        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var tour = await _context.Tours.FirstOrDefaultAsync(m => m.Id == id);
            return tour == null ? NotFound() : View(tour);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tour = await _context.Tours.FindAsync(id);
            if (tour != null) _context.Tours.Remove(tour);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TourExists(int id) => _context.Tours.Any(e => e.Id == id);
    }
}
