using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuseumWebApp.Data;

namespace MuseumWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly MuseumDbContext _context;

        public HomeController(MuseumDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Tours = await _context.Tours
                .AsNoTracking()
                .OrderBy(t => t.Title)
                .ToListAsync();
            return View();
        }

        public IActionResult Privacy() => View();
    }
}
