using System.ComponentModel.DataAnnotations;

namespace MuseumWebApp.Models
{
    public class Position
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        // Навигационное свойство: у должности может быть много сотрудников
        public ICollection<Employee>? Employees { get; set; }
    }
}