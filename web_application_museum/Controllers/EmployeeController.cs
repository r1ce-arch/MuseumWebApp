using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MuseumWebApp.Data;
using MuseumWebApp.Models;
using BCryptNet = BCrypt.Net.BCrypt;

namespace MuseumWebApp.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class EmployeesController : Controller
    {
        private readonly MuseumDbContext _context;

        public EmployeesController(MuseumDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Employees.Include(e => e.Position).ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var emp = await _context.Employees.Include(e => e.Position).FirstOrDefaultAsync(m => m.Id == id);
            return emp == null ? NotFound() : View(emp);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            ViewData["PositionId"] = new SelectList(_context.Positions, "Id", "Title");
            return View(new Employee { HireDate = DateTime.Today });
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            string fullName,
            int positionId,
            string hireDateStr,
            string? login,
            string? password,
            string? confirmPassword)
        {
            bool hasErrors = false;

            if (string.IsNullOrWhiteSpace(fullName))
            {
                ModelState.AddModelError("fullName", "Введите ФИО.");
                hasErrors = true;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("password", "Введите пароль.");
                hasErrors = true;
            }
            else if (password.Length < 6)
            {
                ModelState.AddModelError("password", "Пароль должен быть не короче 6 символов.");
                hasErrors = true;
            }
            else if (password != confirmPassword)
            {
                ModelState.AddModelError("confirmPassword", "Пароли не совпадают.");
                hasErrors = true;
            }

            if (hasErrors)
            {
                ViewData["PositionId"] = new SelectList(_context.Positions, "Id", "Title", positionId);
                var vm = new Employee
                {
                    FullName = fullName ?? "",
                    PositionId = positionId,
                    HireDate = DateTime.Today,
                    Login = login
                };
                return View(vm);
            }

            // Парсим дату вручную чтобы избежать Kind=Unspecified
            DateTime hireDate = DateTime.SpecifyKind(
                DateTime.TryParse(hireDateStr, out var parsed) ? parsed : DateTime.Today,
                DateTimeKind.Utc);

            var employee = new Employee
            {
                FullName = fullName!,
                PositionId = positionId,
                HireDate = hireDate,
                Login = login,
                PasswordHash = BCryptNet.HashPassword(password)
            };

            _context.Add(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var emp = await _context.Employees.FindAsync(id);
            if (emp == null) return NotFound();
            ViewData["PositionId"] = new SelectList(_context.Positions, "Id", "Title", emp.PositionId);
            return View(emp);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            string fullName,
            int positionId,
            string hireDateStr,
            string? login,
            string? password,
            string? confirmPassword)
        {
            bool hasErrors = false;

            if (string.IsNullOrWhiteSpace(fullName))
            {
                ModelState.AddModelError("fullName", "Введите ФИО.");
                hasErrors = true;
            }

            if (!string.IsNullOrWhiteSpace(password))
            {
                if (password.Length < 6)
                {
                    ModelState.AddModelError("password", "Пароль должен быть не короче 6 символов.");
                    hasErrors = true;
                }
                else if (password != confirmPassword)
                {
                    ModelState.AddModelError("confirmPassword", "Пароли не совпадают.");
                    hasErrors = true;
                }
            }

            var existing = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);
            if (existing == null) return NotFound();

            if (hasErrors)
            {
                ViewData["PositionId"] = new SelectList(_context.Positions, "Id", "Title", positionId);
                existing.FullName = fullName ?? existing.FullName;
                existing.PositionId = positionId;
                existing.Login = login;
                return View(existing);
            }

            DateTime hireDate = DateTime.SpecifyKind(
                DateTime.TryParse(hireDateStr, out var parsed) ? parsed : existing.HireDate,
                DateTimeKind.Utc);

            existing.FullName = fullName!;
            existing.PositionId = positionId;
            existing.HireDate = hireDate;
            existing.Login = login;

            if (!string.IsNullOrWhiteSpace(password))
                existing.PasswordHash = BCryptNet.HashPassword(password);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var emp = await _context.Employees.Include(e => e.Position).FirstOrDefaultAsync(m => m.Id == id);
            return emp == null ? NotFound() : View(emp);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var emp = await _context.Employees.FindAsync(id);
            if (emp != null) _context.Employees.Remove(emp);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
