using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MuseumWebApp.Data;
using MuseumWebApp.Models;

namespace MuseumWebApp.Controllers
{
    public class TourSchedulesController : Controller
    {
        private readonly MuseumDbContext _context;

        public TourSchedulesController(MuseumDbContext context)
        {
            _context = context;
        }

        private async Task PopulateSelectListsAsync(int? selectedTourId = null, int? selectedGuideId = null)
        {
            var tours = await _context.Tours
                .AsNoTracking()
                .OrderBy(t => t.Title)
                .ToListAsync();

            // Показываем всех сотрудников, не фильтруем по должности
            var guides = await _context.Employees
                .AsNoTracking()
                .Include(e => e.Position)
                .OrderBy(e => e.FullName)
                .Select(e => new {
                    e.Id,
                    DisplayName = e.Position != null
                        ? e.FullName + " (" + e.Position.Title + ")"
                        : e.FullName
                })
                .ToListAsync();

            ViewData["TourId"] = new SelectList(tours, "Id", "Title", selectedTourId);
            ViewData["GuideId"] = new SelectList(guides, "Id", "DisplayName", selectedGuideId);
        }

        // GET: TourSchedules
        public async Task<IActionResult> Index()
        {
            var tourSchedules = _context.TourSchedules
                .Include(t => t.Tour)
                .Include(t => t.Guide);
            return View(await tourSchedules.ToListAsync());
        }

        // GET: TourSchedules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tourSchedule = await _context.TourSchedules
                .Include(t => t.Tour)
                .Include(t => t.Guide)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tourSchedule == null)
            {
                return NotFound();
            }

            return View(tourSchedule);
        }

        // GET: TourSchedules/Create
        public async Task<IActionResult> Create()
        {
            await PopulateSelectListsAsync();
            return View();
        }

        // POST: TourSchedules/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TourId,GuideId")] TourSchedule tourSchedule, string startTimeStr)
        {
            if (DateTime.TryParse(startTimeStr, out var parsedTime))
                tourSchedule.StartTime = DateTime.SpecifyKind(parsedTime, DateTimeKind.Utc);
            else
                ModelState.AddModelError("startTimeStr", "Укажите корректное время начала.");

            if (ModelState.IsValid)
            {
                _context.Add(tourSchedule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            await PopulateSelectListsAsync(tourSchedule.TourId, tourSchedule.GuideId);
            return View(tourSchedule);
        }

        // GET: TourSchedules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tourSchedule = await _context.TourSchedules.FindAsync(id);
            if (tourSchedule == null)
            {
                return NotFound();
            }
            await PopulateSelectListsAsync(tourSchedule.TourId, tourSchedule.GuideId);
            return View(tourSchedule);
        }

        // POST: TourSchedules/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TourId,GuideId")] TourSchedule tourSchedule, string startTimeStr)
        {
            if (id != tourSchedule.Id) return NotFound();

            if (DateTime.TryParse(startTimeStr, out var parsedTime))
                tourSchedule.StartTime = DateTime.SpecifyKind(parsedTime, DateTimeKind.Utc);
            else
                ModelState.AddModelError("startTimeStr", "Укажите корректное время начала.");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tourSchedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TourScheduleExists(tourSchedule.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            await PopulateSelectListsAsync(tourSchedule.TourId, tourSchedule.GuideId);
            return View(tourSchedule);
        }

        // GET: TourSchedules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tourSchedule = await _context.TourSchedules
                .Include(t => t.Tour)
                .Include(t => t.Guide)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tourSchedule == null)
            {
                return NotFound();
            }

            return View(tourSchedule);
        }

        // POST: TourSchedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tourSchedule = await _context.TourSchedules.FindAsync(id);
            if (tourSchedule != null)
            {
                _context.TourSchedules.Remove(tourSchedule);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TourScheduleExists(int id)
        {
            return _context.TourSchedules.Any(e => e.Id == id);
        }
    }
}