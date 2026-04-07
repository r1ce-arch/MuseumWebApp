using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MuseumWebApp.Data;
using MuseumWebApp.Models;

namespace MuseumWebApp.Controllers
{
    public class TicketsController : Controller
    {
        private readonly MuseumDbContext _context;

        public TicketsController(MuseumDbContext context)
        {
            _context = context;
        }

        private async Task PopulateTourScheduleSelectListAsync(int? selectedId = null)
        {
            var schedules = await _context.TourSchedules
                .AsNoTracking()
                .Include(ts => ts.Tour)
                .Include(ts => ts.Guide)
                .OrderBy(ts => ts.StartTime)
                .ToListAsync();

            var items = schedules
                .Select(ts => new
                {
                    ts.Id,
                    Display = (ts.Tour != null ? ts.Tour.Title : "Экскурсия") +
                              " — " + ts.StartTime.ToString("g") +
                              (ts.Guide != null ? (" — " + ts.Guide.FullName) : string.Empty)
                })
                .ToList();

            ViewData["TourScheduleId"] = new SelectList(items, "Id", "Display", selectedId);
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            var tickets = _context.Tickets
                .Include(t => t.TourSchedule)
                    .ThenInclude(ts => ts!.Tour);
            return View(await tickets.ToListAsync());
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.TourSchedule)
                    .ThenInclude(ts => ts!.Tour)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/Create
        public async Task<IActionResult> Create()
        {
            await PopulateTourScheduleSelectListAsync();
            return View();
        }

        // POST: Tickets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TourScheduleId,SaleDate,VisitorName,IsPaid")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            await PopulateTourScheduleSelectListAsync(ticket.TourScheduleId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            await PopulateTourScheduleSelectListAsync(ticket.TourScheduleId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TourScheduleId,SaleDate,VisitorName,IsPaid")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            await PopulateTourScheduleSelectListAsync(ticket.TourScheduleId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.TourSchedule)
                    .ThenInclude(ts => ts!.Tour)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.Id == id);
        }
    }
}