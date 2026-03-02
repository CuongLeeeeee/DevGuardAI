namespace DevGuardAI.DAL.Entities;

public class ChatMessage
{
    public Guid Id { get; set; }

    public Guid ChatSessionId { get; set; }

    public string Role { get; set; } = string.Empty;
    // "User" or "AI"

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ChatSession ChatSession { get; set; } = null!;
}