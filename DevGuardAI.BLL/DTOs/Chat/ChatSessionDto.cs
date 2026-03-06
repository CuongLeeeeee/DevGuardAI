public class ChatSessionDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string SessionType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string ContextSummary { get; set; } = string.Empty;
}