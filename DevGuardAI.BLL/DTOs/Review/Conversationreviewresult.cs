public class ConversationReviewResult
{
    public ReviewResult? Review { get; set; }
    public string? Answer { get; set; }
    public string UpdatedContextSummary { get; set; } = string.Empty;
}