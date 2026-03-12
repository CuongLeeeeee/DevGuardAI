using System.Text.Json.Serialization;

public class ReviewResult
{
    [JsonPropertyName("language")]
    public string? Language { get; set; }

    [JsonPropertyName("score")]
    public int Score { get; set; }

    [JsonPropertyName("summary")]
    public string? Summary { get; set; }

    [JsonPropertyName("issues")]
    public List<Issue>? Issues { get; set; }

    [JsonPropertyName("suggestions")]
    public List<string>? Suggestions { get; set; }

    [JsonPropertyName("improvedCode")]
    public string? ImprovedCode { get; set; }
}