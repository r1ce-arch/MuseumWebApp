using System.ComponentModel.DataAnnotations;

namespace MuseumWebApp.Models
{
    public class Visitor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public DateTime RegistrationDate { get; set; } = DateTime.Today;

        public ICollection<Ticket>? Tickets { get; set; }
    }
}
