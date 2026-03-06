public class ReviewResult
{
    public string? Language { get; set; }

    public int Score { get; set; }

    public string? Summary { get; set; }

    public List<Issue>? Issues { get; set; }

    public List<string>? Suggestions { get; set; }

    public string? ImprovedCode { get; set; }
}