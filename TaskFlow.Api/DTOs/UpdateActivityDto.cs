namespace TaskFlow.Api.DTOs
{
    public class UpdateActivityDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? Priority { get; set; }
        public bool IsCompleted { get; set; }
    }
}
