using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MuseumWebApp.Models
{
    public class TourSchedule
    {
        [Key]
        public int Id { get; set; }

        public int TourId { get; set; }

        [ForeignKey("TourId")]
        public Tour? Tour { get; set; }

        public int GuideId { get; set; }

        [ForeignKey("GuideId")]
        public Employee? Guide { get; set; }

        public DateTime StartTime { get; set; }

        public ICollection<Ticket>? Tickets { get; set; }
    }
}