using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuseumWebApp.Data;
using MuseumWebApp.Models;
using System.Security.Claims;
using BCryptNet = BCrypt.Net.BCrypt;

namespace MuseumWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly MuseumDbContext _context;

        public AccountController(MuseumDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string login, string password, string? returnUrl = null)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Введите логин и пароль.");
                return View();
            }

            // Сначала проверяем сотрудников (вход по Login)
            var employee = await _context.Employees
                .Include(e => e.Position)
                .FirstOrDefaultAsync(e => e.Login == login);

            if (employee != null && employee.PasswordHash != null &&
                BCryptNet.Verify(password, employee.PasswordHash))
            {
                var role = employee.Position?.Title == "Администратор" ? "Admin" : "Employee";
                await SignInAsync(employee.Id.ToString(), employee.FullName, role);
                return Redirect(returnUrl ?? "/");
            }

            // Затем проверяем посетителей (вход по Email)
            var visitor = await _context.Visitors
                .FirstOrDefaultAsync(v => v.Email == login);

            if (visitor != null && BCryptNet.Verify(password, visitor.PasswordHash))
            {
                await SignInAsync(visitor.Id.ToString(), visitor.FullName, "Visitor");
                return Redirect(returnUrl ?? "/");
            }

            ModelState.AddModelError("", "Неверный логин/email или пароль.");
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string fullName, string email, string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                ModelState.AddModelError("fullName", "Введите имя.");

            if (string.IsNullOrWhiteSpace(email))
                ModelState.AddModelError("email", "Введите email.");

            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                ModelState.AddModelError("password", "Пароль не короче 6 символов.");
            else if (password != confirmPassword)
                ModelState.AddModelError("confirmPassword", "Пароли не совпадают.");

            if (await _context.Visitors.AnyAsync(v => v.Email == email))
                ModelState.AddModelError("email", "Этот email уже зарегистрирован.");

            if (!ModelState.IsValid)
                return View();

            var visitor = new Visitor
            {
                FullName         = fullName!,
                Email            = email!,
                PasswordHash     = BCryptNet.HashPassword(password),
                RegistrationDate = DateTime.Today
            };

            _context.Visitors.Add(visitor);
            await _context.SaveChangesAsync();

            await SignInAsync(visitor.Id.ToString(), visitor.FullName, "Visitor");
            return RedirectToAction("Index", "Home");
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        private async Task SignInAsync(string id, string name, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, id),
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Role, role)
            };

            var identity  = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties { IsPersistent = true });
        }
    }
}
