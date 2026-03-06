public class ConversationTurn
{
    /// <summary>"user" hoặc "assistant"</summary>
    public string Role { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;
}