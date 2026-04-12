using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MuseumWebApp.Data;
using MuseumWebApp.Models;
using MuseumWebApp.Services;
using System.Security.Claims;

namespace MuseumWebApp.Controllers
{
    public class TicketsController : Controller
    {
        private readonly MuseumDbContext _context;
        private readonly QRCodeService   _qr;
        private readonly EmailService    _email;

        public TicketsController(MuseumDbContext context, QRCodeService qr, EmailService email)
        {
            _context = context;
            _qr      = qr;
            _email   = email;
        }

        private async Task PopulateTourScheduleSelectListAsync(int? selectedId = null)
        {
            var schedules = await _context.TourSchedules
                .AsNoTracking()
                .Include(ts => ts.Tour)
                .Include(ts => ts.Guide)
                .OrderBy(ts => ts.StartTime)
                .ToListAsync();

            var items = schedules.Select(ts => new
            {
                ts.Id,
                Display = (ts.Tour?.Title ?? "Экскурсия") + " — " +
                          ts.StartTime.ToString("dd.MM.yyyy HH:mm") +
                          (ts.Guide != null ? " — " + ts.Guide.FullName : "")
            }).ToList();

            ViewData["TourScheduleId"] = new SelectList(items, "Id", "Display", selectedId);
        }

        // ── Административные методы ──────────────────────────────

        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Index()
        {
            var tickets = _context.Tickets
                .Include(t => t.TourSchedule).ThenInclude(ts => ts!.Tour)
                .Include(t => t.Visitor);
            return View(await tickets.ToListAsync());
        }

        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var ticket = await _context.Tickets
                .Include(t => t.TourSchedule).ThenInclude(ts => ts!.Tour)
                .Include(t => t.Visitor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null) return NotFound();

            ViewBag.QrCode = _qr.GenerateQRCodeBase64(ticket.TicketCode);
            return View(ticket);
        }

        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Create()
        {
            await PopulateTourScheduleSelectListAsync();
            ViewData["VisitorId"] = new SelectList(
                await _context.Visitors.OrderBy(v => v.FullName).ToListAsync(), "Id", "FullName");
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Create([Bind("TourScheduleId,VisitorId,IsPaid")] Ticket ticket)
        {
            ticket.SaleDate   = DateTime.Today;
            ticket.TicketCode = Guid.NewGuid().ToString("N").ToUpper();
            if (ModelState.IsValid)
            {
                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            await PopulateTourScheduleSelectListAsync(ticket.TourScheduleId);
            ViewData["VisitorId"] = new SelectList(
                await _context.Visitors.OrderBy(v => v.FullName).ToListAsync(), "Id", "FullName", ticket.VisitorId);
            return View(ticket);
        }

        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) return NotFound();
            await PopulateTourScheduleSelectListAsync(ticket.TourScheduleId);
            ViewData["VisitorId"] = new SelectList(
                await _context.Visitors.OrderBy(v => v.FullName).ToListAsync(), "Id", "FullName", ticket.VisitorId);
            return View(ticket);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TourScheduleId,VisitorId,SaleDate,IsPaid,TicketCode")] Ticket ticket)
        {
            if (id != ticket.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try { _context.Update(ticket); await _context.SaveChangesAsync(); }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Tickets.Any(e => e.Id == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            await PopulateTourScheduleSelectListAsync(ticket.TourScheduleId);
            ViewData["VisitorId"] = new SelectList(
                await _context.Visitors.OrderBy(v => v.FullName).ToListAsync(), "Id", "FullName", ticket.VisitorId);
            return View(ticket);
        }

        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var ticket = await _context.Tickets
                .Include(t => t.TourSchedule).ThenInclude(ts => ts!.Tour)
                .Include(t => t.Visitor)
                .FirstOrDefaultAsync(m => m.Id == id);
            return ticket == null ? NotFound() : View(ticket);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket != null) _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ── Методы для посетителей ───────────────────────────────

        // Мои билеты (авторизованный посетитель)
        [Authorize(Roles = "Visitor")]
        public async Task<IActionResult> MyTickets()
        {
            var visitorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var tickets = await _context.Tickets
                .Include(t => t.TourSchedule).ThenInclude(ts => ts!.Tour)
                .Include(t => t.TourSchedule!).ThenInclude(ts => ts!.Guide)
                .Where(t => t.VisitorId == visitorId)
                .ToListAsync();

            ViewBag.QrCodes = tickets.ToDictionary(
                t => t.Id,
                t => _qr.GenerateQRCodeBase64(t.TicketCode));

            return View(tickets);
        }

        // Поиск билетов по email (без авторизации)
        [HttpGet]
        public IActionResult FindByEmail() => View();

        [HttpPost]
        public async Task<IActionResult> FindByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError("email", "Введите email.");
                return View();
            }

            var visitor = await _context.Visitors
                .FirstOrDefaultAsync(v => v.Email == email);

            if (visitor == null)
            {
                ViewBag.NotFound = true;
                return View();
            }

            var tickets = await _context.Tickets
                .Include(t => t.TourSchedule).ThenInclude(ts => ts!.Tour)
                .Include(t => t.TourSchedule!).ThenInclude(ts => ts!.Guide)
                .Where(t => t.VisitorId == visitor.Id)
                .ToListAsync();

            ViewBag.QrCodes = tickets.ToDictionary(
                t => t.Id,
                t => _qr.GenerateQRCodeBase64(t.TicketCode));
            ViewBag.Email = email;

            return View("MyTicketsByEmail", tickets);
        }

        // Выбор расписания для покупки
        public async Task<IActionResult> Buy(int? scheduleId, int? tourId)
        {
            if (scheduleId == null)
            {
                // Если передан tourId — показываем расписания только для этого тура
                var query = _context.TourSchedules
                    .Include(ts => ts.Tour)
                    .Include(ts => ts.Guide)
                    .AsQueryable();

                if (tourId.HasValue)
                    query = query.Where(ts => ts.TourId == tourId.Value);

                var schedules = await query.OrderBy(ts => ts.StartTime).ToListAsync();

                // Передаём название тура если фильтруем
                if (tourId.HasValue)
                {
                    var tour = await _context.Tours.FindAsync(tourId.Value);
                    ViewBag.TourTitle = tour?.Title;
                    ViewBag.TourId = tourId;
                }

                return View("BuySelect", schedules);
            }

            var schedule = await _context.TourSchedules
                .Include(ts => ts.Tour)
                .Include(ts => ts.Guide)
                .FirstOrDefaultAsync(ts => ts.Id == scheduleId);

            if (schedule == null) return NotFound();
            return View("BuyConfirm", schedule);
        }

        // POST покупки — только для авторизованных посетителей
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Visitor")]
        public async Task<IActionResult> BuyConfirm(int tourScheduleId)
        {
            var visitorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var visitor   = await _context.Visitors.FindAsync(visitorId);

            var schedule = await _context.TourSchedules
                .Include(ts => ts.Tour)
                .Include(ts => ts.Guide)
                .FirstOrDefaultAsync(ts => ts.Id == tourScheduleId);

            var ticket = new Ticket
            {
                TourScheduleId = tourScheduleId,
                VisitorId      = visitorId,
                SaleDate       = DateTime.Today,
                IsPaid         = false,
                TicketCode     = Guid.NewGuid().ToString("N").ToUpper()
            };

            _context.Add(ticket);
            await _context.SaveChangesAsync();

            // Отправляем письмо если есть email
            if (visitor != null && !string.IsNullOrEmpty(visitor.Email))
            {
                try
                {
                    var qrBytes   = _qr.GenerateQRCode(ticket.TicketCode);
                    var tourTitle = schedule?.Tour?.Title ?? "Экскурсия";
                    var tourDate  = schedule?.StartTime.ToString("dd.MM.yyyy HH:mm") ?? "";
                    await _email.SendTicketEmailAsync(
                        visitor.Email, visitor.FullName,
                        ticket.TicketCode, tourTitle, tourDate, qrBytes);
                }
                catch
                {
                    // Не прерываем процесс если письмо не отправилось
                }
            }

            return RedirectToAction(nameof(TicketSuccess), new { id = ticket.Id });
        }

        // Страница успешной покупки с QR
        [Authorize(Roles = "Visitor")]
        public async Task<IActionResult> TicketSuccess(int id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.TourSchedule).ThenInclude(ts => ts!.Tour)
                .Include(t => t.TourSchedule!).ThenInclude(ts => ts!.Guide)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (ticket == null) return NotFound();

            ViewBag.QrCode = _qr.GenerateQRCodeBase64(ticket.TicketCode);
            return View(ticket);
        }
    }
}
