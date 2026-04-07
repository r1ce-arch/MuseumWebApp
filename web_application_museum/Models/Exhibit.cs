using System.ComponentModel.DataAnnotations;

namespace MuseumWebApp.Models
{
    public class Exhibit
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [MaxLength(100)]
        public string? Era { get; set; }

        public int? Year { get; set; }

        // Путь к фото (относительный, например /uploads/exhibit_1.jpg)
        [MaxLength(255)]
        public string? PhotoPath { get; set; }

        public ICollection<TourExhibit>? TourExhibits { get; set; }
    }
}