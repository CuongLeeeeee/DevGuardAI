public class ConversationRequest
{
    public Guid SessionId { get; set; }
    public string Content { get; set; } = string.Empty;
}