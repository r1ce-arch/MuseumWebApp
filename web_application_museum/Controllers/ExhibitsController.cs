using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuseumWebApp.Data;
using MuseumWebApp.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MuseumWebApp.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class ExhibitsController : Controller
    {
        private readonly MuseumDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ExhibitsController(MuseumDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Exhibits
        public async Task<IActionResult> Index()
        {
            var exhibits = await _context.Exhibits
                .AsNoTracking()
                .OrderBy(e => e.Name)
                .ToListAsync();
            return View(exhibits);
        }

        // GET: Exhibits/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var exhibit = await _context.Exhibits
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (exhibit == null) return NotFound();
            return View(exhibit);
        }

        // GET: Exhibits/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Exhibits/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Year")] Exhibit exhibit, IFormFile? photo)
        {
            // Валидация файла (если загружен)
            if (photo != null && photo.Length > 0)
            {
                if (!IsValidImageFile(photo))
                {
                    ModelState.AddModelError("photo", "Разрешены только изображения (jpg, jpeg, png, gif) размером до 5 МБ.");
                    return View(exhibit);
                }
            }

            if (!ModelState.IsValid) return View(exhibit);

            try
            {
                if (photo != null && photo.Length > 0)
                {
                    exhibit.PhotoPath = await SavePhotoAsync(photo);
                }

                _context.Add(exhibit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ошибка при сохранении: {ex.Message}");
                return View(exhibit);
            }
        }

        // GET: Exhibits/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var exhibit = await _context.Exhibits.FindAsync(id);
            if (exhibit == null) return NotFound();
            return View(exhibit);
        }

        // POST: Exhibits/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Year,PhotoPath")] Exhibit exhibit, IFormFile? photo)
        {
            if (id != exhibit.Id) return NotFound();

            // Валидация нового файла
            if (photo != null && photo.Length > 0 && !IsValidImageFile(photo))
            {
                ModelState.AddModelError("photo", "Разрешены только изображения (jpg, jpeg, png, gif) размером до 5 МБ.");
                return View(exhibit);
            }

            if (!ModelState.IsValid) return View(exhibit);

            try
            {
                // Если загружено новое фото — удаляем старое и сохраняем новое
                if (photo != null && photo.Length > 0)
                {
                    // Удаляем старый файл, если он существует
                    if (!string.IsNullOrEmpty(exhibit.PhotoPath))
                    {
                        DeletePhotoFile(exhibit.PhotoPath);
                    }
                    exhibit.PhotoPath = await SavePhotoAsync(photo);
                }
                else
                {
                    // Если новое фото не загружено, оставляем существующий путь
                    // (при использовании Bind он может быть перезаписан пустотой, поэтому явно сохраняем старый)
                    var existing = await _context.Exhibits.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
                    if (existing != null && string.IsNullOrEmpty(exhibit.PhotoPath))
                    {
                        exhibit.PhotoPath = existing.PhotoPath;
                    }
                }

                _context.Update(exhibit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExhibitExists(exhibit.Id)) return NotFound();
                throw;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ошибка при обновлении: {ex.Message}");
                return View(exhibit);
            }
        }

        // GET: Exhibits/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var exhibit = await _context.Exhibits
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (exhibit == null) return NotFound();
            return View(exhibit);
        }

        // POST: Exhibits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exhibit = await _context.Exhibits.FindAsync(id);
            if (exhibit != null)
            {
                // Удаляем файл фото с диска
                if (!string.IsNullOrEmpty(exhibit.PhotoPath))
                {
                    DeletePhotoFile(exhibit.PhotoPath);
                }
                _context.Exhibits.Remove(exhibit);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ExhibitExists(int id)
        {
            return _context.Exhibits.Any(e => e.Id == id);
        }

        /// <summary>
        /// Проверяет, является ли файл допустимым изображением (расширение и размер)
        /// </summary>
        private bool IsValidImageFile(IFormFile file)
        {
            if (file == null || file.Length == 0) return false;

            // Максимальный размер 5 МБ
            if (file.Length > 5 * 1024 * 1024) return false;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            return allowedExtensions.Contains(ext);
        }

        /// <summary>
        /// Сохраняет фото на диск (поддержка локальной разработки и облачного хранилища Amvera)
        /// </summary>
        private async Task<string> SavePhotoAsync(IFormFile photo)
        {
            // Для Amvera Cloud используем постоянное хранилище /data/uploads/exhibits
            // Для локальной разработки (Windows) используем wwwroot/uploads/exhibits
            string baseDir;
            if (Directory.Exists("/data") && Environment.OSVersion.Platform == PlatformID.Unix)
            {
                // Облачная среда (Amvera)
                baseDir = "/data/uploads/exhibits";
            }
            else
            {
                // Локальная среда
                baseDir = Path.Combine(_env.WebRootPath, "uploads", "exhibits");
            }

            Directory.CreateDirectory(baseDir);

            var ext = Path.GetExtension(photo.FileName).ToLowerInvariant();
            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(baseDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await photo.CopyToAsync(stream);
            }


            if (baseDir.StartsWith("/data"))
            {

                return $"/data/uploads/exhibits/{fileName}";
            }
            else
            {
                return $"/uploads/exhibits/{fileName}";
            }
        }

        /// <summary>
        /// Удаляет файл фото по относительному пути
        /// </summary>
        private void DeletePhotoFile(string photoPath)
        {
            if (string.IsNullOrEmpty(photoPath)) return;

            string fullPath;
            if (photoPath.StartsWith("/data/"))
            {
                // Облачное хранилище
                fullPath = photoPath;
            }
            else
            {
                // Локальное хранилище
                fullPath = Path.Combine(_env.WebRootPath, photoPath.TrimStart('/'));
            }

            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }
    }
}