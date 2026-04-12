using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MuseumWebApp.Data;
using MuseumWebApp.Models;
using MuseumWebApp.Services;
using BCryptNet = BCrypt.Net.BCrypt;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Railway передаёт PORT через переменную окружения
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

builder.Services.AddControllersWithViews();

// Строка подключения: сначала переменная окружения DATABASE_URL (Railway),
// иначе из appsettings.json
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (!string.IsNullOrEmpty(databaseUrl))
{
    var uri      = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');
    var dbHost   = uri.Host;
    var dbPort   = uri.Port > 0 ? uri.Port : 5432;
    var dbName   = uri.AbsolutePath.TrimStart('/');
    var dbUser   = userInfo[0];
    var dbPass   = userInfo.Length > 1 ? userInfo[1] : "";
    var connStr  = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPass};SSL Mode=Require;Trust Server Certificate=true";
    builder.Services.AddDbContext<MuseumDbContext>(options => options.UseNpgsql(connStr));
}
else
{
    builder.Services.AddDbContext<MuseumDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("MuseumDb")));
}

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath       = "/Account/Login";
        options.LogoutPath      = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan  = TimeSpan.FromHours(8);
    });

builder.Services.AddAuthorization();
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

// На Railway HTTPS терминируется на уровне прокси — редирект не нужен
if (app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
