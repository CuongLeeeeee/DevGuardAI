namespace DevGuardAI.DAL.Entities;

public class ChatSession
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public Guid UserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string? ContextSummary { get; set; } = string.Empty;

    public string SessionType { get; set; } = "Review";
    // "Review" | "TestCase"

    // Navigation
    public User User { get; set; } = null!;
    public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}