using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuseumWebApp.Data;
using MuseumWebApp.Models;

namespace MuseumWebApp.Controllers
{
    public class ExhibitsController : Controller
    {
        private readonly MuseumDbContext _context;

        public ExhibitsController(MuseumDbContext context)
        {
            _context = context;
        }

        // GET: Exhibits
        public async Task<IActionResult> Index()
        {
            return View(await _context.Exhibits.AsNoTracking().OrderBy(e => e.Name).ToListAsync());
        }

        // GET: Exhibits/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var exhibit = await _context.Exhibits
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            return exhibit == null ? NotFound() : View(exhibit);
        }

        // GET: Exhibits/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Exhibits/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Era,Year")] Exhibit exhibit)
        {
            if (!ModelState.IsValid) return View(exhibit);

            _context.Add(exhibit);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Exhibits/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var exhibit = await _context.Exhibits.FindAsync(id);
            return exhibit == null ? NotFound() : View(exhibit);
        }

        // POST: Exhibits/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Era,Year")] Exhibit exhibit)
        {
            if (id != exhibit.Id) return NotFound();

            if (!ModelState.IsValid) return View(exhibit);

            try
            {
                _context.Update(exhibit);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExhibitExists(exhibit.Id)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Exhibits/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var exhibit = await _context.Exhibits
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            return exhibit == null ? NotFound() : View(exhibit);
        }

        // POST: Exhibits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exhibit = await _context.Exhibits.FindAsync(id);
            if (exhibit != null)
            {
                _context.Exhibits.Remove(exhibit);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ExhibitExists(int id)
        {
            return _context.Exhibits.Any(e => e.Id == id);
        }
    }
}

