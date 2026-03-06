
public class ConversationTestCaseRequest
{
    public string Content { get; set; } = string.Empty;
    public List<ConversationTurn> History { get; set; } = new();
    public string? ContextSummary { get; set; }
}