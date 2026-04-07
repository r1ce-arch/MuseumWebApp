using Microsoft.EntityFrameworkCore;
using MuseumWebApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Добавляем контроллеры с представлениями
builder.Services.AddControllersWithViews();

// Регистрируем DbContext с PostgreSQL
builder.Services.AddDbContext<MuseumDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("MuseumDb")));

var app = builder.Build();

// Настройка конвейера HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();