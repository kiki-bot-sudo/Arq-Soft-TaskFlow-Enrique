using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Domain.Models
{
    public class Task
    {
        public int Id { get; set; }
        public int ActivityId { get; set; }

        [Required] [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        public bool IsCompleted { get; set; } = false;
        public DateTime? DueTime { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Activity Activity { get; set; } = null!;
    }
}
