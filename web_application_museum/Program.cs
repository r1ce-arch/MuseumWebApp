using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MuseumWebApp.Data;
using MuseumWebApp.Models;
using MuseumWebApp.Services;
using BCryptNet = BCrypt.Net.BCrypt;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<MuseumDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("MuseumDb")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath  = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

builder.Services.AddAuthorization();

// Сервисы приложения
builder.Services.AddScoped<QRCodeService>();
builder.Services.AddScoped<EmailService>();

var app = builder.Build();

// Seed: создать должность и администратора при первом запуске
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MuseumDbContext>();
    db.Database.Migrate();

    if (!db.Positions.Any(p => p.Title == "Администратор"))
        db.Positions.Add(new Position { Title = "Администратор" });
    db.SaveChanges();

    var adminPosition = db.Positions.First(p => p.Title == "Администратор");

    if (!db.Employees.Any(e => e.Login == "admin"))
    {
        db.Employees.Add(new Employee
        {
            FullName     = "Администратор",
            Login        = "admin",
            PasswordHash = BCryptNet.HashPassword("admin123"),
            PositionId   = adminPosition.Id,
            HireDate     = DateTime.Today
        });
        db.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
