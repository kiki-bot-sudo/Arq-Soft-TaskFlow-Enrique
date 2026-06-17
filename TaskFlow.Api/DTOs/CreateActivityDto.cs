namespace TaskFlow.Api.DTOs
{
    public class CreateActivityDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Category { get; set; } = string.Empty;
        public string? Priority { get; set; }
    }
}
