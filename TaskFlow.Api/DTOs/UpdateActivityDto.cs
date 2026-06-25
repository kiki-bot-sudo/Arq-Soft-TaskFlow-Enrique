using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Api.DTOs
{
    public class UpdateActivityDto
    {
        [Required(ErrorMessage = "El título es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El título no puede superar 100 caracteres.")]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "La descripción no puede superar 500 caracteres.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        [MaxLength(50, ErrorMessage = "La categoría no puede superar 50 caracteres.")]
        public string Category { get; set; } = string.Empty;

        [RegularExpression("^(Low|Normal|High)$", ErrorMessage = "La prioridad debe ser Low, Normal o High.")]
        public string? Priority { get; set; }

        public bool IsCompleted { get; set; }
    }
}
