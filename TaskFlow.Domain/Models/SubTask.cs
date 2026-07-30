namespace TaskFlow.Domain.Models;

public class SubTask
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Task Task { get; set; } = null!;
}
