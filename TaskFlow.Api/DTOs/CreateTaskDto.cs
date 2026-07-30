using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Api.DTOs
{
    public class CreateTaskDto
    {
        [Required(ErrorMessage = "El título es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El título no puede superar 100 caracteres.")]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "La descripción no puede superar 500 caracteres.")]
        public string Description { get; set; } = string.Empty;

        [RegularExpression("^(Low|Medium|High)$", ErrorMessage = "La prioridad debe ser Low, Medium o High.")]
        public string Priority { get; set; } = "Medium";

        public DateTime? DueTime { get; set; }
    }
}
