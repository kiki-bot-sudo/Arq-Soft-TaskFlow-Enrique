using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TaskFlow.Api.DTOs
{
    public class UpdateTaskDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("activityId")]
        public int ActivityId { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El título no puede superar 100 caracteres.")]
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "La descripción no puede superar 500 caracteres.")]
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("isCompleted")]
        public bool IsCompleted { get; set; }

        [JsonPropertyName("dueTime")]
        public DateTime? DueTime { get; set; }
    }
}
