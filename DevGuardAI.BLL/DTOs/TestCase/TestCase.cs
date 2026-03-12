using System.Text.Json.Serialization;

public class TestCase
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("input")]
    public string? Input { get; set; }

    [JsonPropertyName("expected")]
    public string? Expected { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}