using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MuseumWebApp.Models
{
    public class Tour
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        public TimeSpan Duration { get; set; } // в БД будет тип INTERVAL

        public ICollection<TourSchedule>? TourSchedules { get; set; }

        public ICollection<TourExhibit>? TourExhibits { get; set; }
    }
}