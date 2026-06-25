using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Domain.Models
{
    public class Activity
    {
        public int Id { get; set; }

        [Required] [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        [Required] [MaxLength(50)]
        public string Category { get; set; } = string.Empty;

        [MaxLength(10)]
        public string Priority { get; set; } = "Normal"; // Low | Normal | High

        public bool IsCompleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Task> Tasks { get; set; } = new List<Task>();
    }
}
