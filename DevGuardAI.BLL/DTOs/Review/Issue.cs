using System.Text.Json.Serialization;

public class Issue
{
    [JsonPropertyName("severity")]
    public string? Severity { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("line")]
    public int? Line { get; set; }

    [JsonPropertyName("suggestion")]
    public string? Suggestion { get; set; }
}