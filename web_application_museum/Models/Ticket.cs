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

        public int? VisitorId { get; set; }

        [ForeignKey("VisitorId")]
        public Visitor? Visitor { get; set; }

        public DateTime SaleDate { get; set; } = DateTime.Today;

        public bool IsPaid { get; set; } = false;

        [MaxLength(32)]
        public string TicketCode { get; set; } = Guid.NewGuid().ToString("N").ToUpper();
    }
}
