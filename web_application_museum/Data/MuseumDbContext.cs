using Microsoft.EntityFrameworkCore;
using MuseumWebApp.Models;

namespace MuseumWebApp.Data
{
    public class MuseumDbContext : DbContext
    {
        public MuseumDbContext(DbContextOptions<MuseumDbContext> options)
            : base(options)
        {
        }

        public DbSet<Position> Positions { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Tour> Tours { get; set; }
        public DbSet<TourSchedule> TourSchedules { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Exhibit> Exhibits { get; set; }
        public DbSet<TourExhibit> TourExhibits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Уникальность логина
            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Login)
                .IsUnique();

            // Дополнительная настройка для TimeSpan -> INTERVAL
            modelBuilder.Entity<Tour>()
                .Property(t => t.Duration)
                .HasConversion(
                    v => v,
                    v => v
                );

            modelBuilder.Entity<TourExhibit>()
                .HasKey(te => new { te.TourId, te.ExhibitId });

            modelBuilder.Entity<TourExhibit>()
                .HasOne(te => te.Tour)
                .WithMany(t => t.TourExhibits)
                .HasForeignKey(te => te.TourId);

            modelBuilder.Entity<TourExhibit>()
                .HasOne(te => te.Exhibit)
                .WithMany(e => e.TourExhibits)
                .HasForeignKey(te => te.ExhibitId);
        }
    }
}