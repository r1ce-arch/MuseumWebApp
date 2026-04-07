using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MuseumWebApp.Models
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        public int TourScheduleId { get; set; }

        [ForeignKey("TourScheduleId")]
        public TourSchedule? TourSchedule { get; set; }

        public DateTime SaleDate { get; set; } = DateTime.Now;

        [MaxLength(100)]
        public string? VisitorName { get; set; }

        public bool IsPaid { get; set; } = false;
    }
}