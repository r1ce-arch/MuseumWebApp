using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MuseumWebApp.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите ФИО.")]
        [MaxLength(255)]
        public string FullName { get; set; } = string.Empty;

        public int PositionId { get; set; }

        [ForeignKey("PositionId")]
        public Position? Position { get; set; }

        public DateTime HireDate { get; set; } = DateTime.Today;

        [MaxLength(50)]
        public string? Login { get; set; }

        public string? PasswordHash { get; set; }

        [NotMapped]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [NotMapped]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }
    }
}
