using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Api.DTOs
{
    /// <summary>
    /// DTO para actualizar una tarea existente.
    /// Nota: Id y ActivityId se toman de la ruta (ver TaskController.UpdateTask),
    /// por eso no se incluyen aquí; antes existían como campos sin uso real (ver ADR-04).
    /// </summary>
    public class UpdateTaskDto
    {
        [Required(ErrorMessage = "El título es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El título no puede superar 100 caracteres.")]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "La descripción no puede superar 500 caracteres.")]
        public string Description { get; set; } = string.Empty;

        public bool IsCompleted { get; set; }

        public DateTime? DueTime { get; set; }
    }
}
