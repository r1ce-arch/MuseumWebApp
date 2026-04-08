using Microsoft.EntityFrameworkCore;
using MuseumWebApp.Data;

// Глобально разрешает Npgsql принимать DateTime с Kind=Unspecified как UTC.
// Без этого PostgreSQL тип timestamptz выбрасывает исключение для DateTime.Today и new DateTime().
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<MuseumDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("MuseumDb")));

var app = builder.Build();

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
