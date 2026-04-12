using Microsoft.EntityFrameworkCore;
using MuseumWebApp.Models;

namespace MuseumWebApp.Data
{
    public class MuseumDbContext : DbContext
    {
        public MuseumDbContext(DbContextOptions<MuseumDbContext> options) : base(options) { }

        public DbSet<Position> Positions { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Tour> Tours { get; set; }
        public DbSet<TourSchedule> TourSchedules { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Exhibit> Exhibits { get; set; }
        public DbSet<TourExhibit> TourExhibits { get; set; }
        public DbSet<Visitor> Visitors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Явные имена таблиц в нижнем регистре для PostgreSQL
            modelBuilder.Entity<Position>().ToTable("positions");
            modelBuilder.Entity<Employee>().ToTable("employees");
            modelBuilder.Entity<Tour>().ToTable("tours");
            modelBuilder.Entity<TourSchedule>().ToTable("tour_schedules");
            modelBuilder.Entity<Ticket>().ToTable("tickets");
            modelBuilder.Entity<Exhibit>().ToTable("exhibits");
            modelBuilder.Entity<TourExhibit>().ToTable("tour_exhibits");
            modelBuilder.Entity<Visitor>().ToTable("visitors");

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Login)
                .IsUnique();

            modelBuilder.Entity<Visitor>()
                .HasIndex(v => v.Email)
                .IsUnique();

            modelBuilder.Entity<Tour>()
                .Property(t => t.Duration)
                .HasConversion(v => v, v => v);

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

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Visitor)
                .WithMany(v => v.Tickets)
                .HasForeignKey(t => t.VisitorId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
