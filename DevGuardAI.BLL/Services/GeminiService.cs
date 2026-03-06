using DevGuardAI.BLL.PromptBuilders;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json;

public class GeminiService:IGeminiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public GeminiService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<ReviewResult?> ReviewCode(string content)
    {
        var apiKey = _config["Gemini:ApiKey"];

        var prompt = PromptBuilder.BuildSystemPrompt() +
                     PromptBuilder.BuildUserPrompt(content);

        var requestBody = new
        {
            contents = new[]
            {
            new
            {
                parts = new[]
                {
                    new { text = prompt }
                }
            }
        },

            generationConfig = new
            {
                maxOutputTokens = 2000,
                temperature = 0.2,
                topP = 0.9
            }
        };

        var response = await _httpClient.PostAsJsonAsync(
            $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}",
            requestBody);

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Gemini API error: {response.StatusCode}");

        var json = await response.Content.ReadAsStringAsync();
        await Task.Delay(1000);
        using var doc = JsonDocument.Parse(json);

        var text = doc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();

        var reviewResult = JsonSerializer.Deserialize<ReviewResult>(
            text!,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        return reviewResult;
    }
    public async Task<TestCaseResult?> GenerateTestCases(string content)
    {
        var apiKey = _config["Gemini:ApiKey"];
        var prompt = PromptBuilder.BuildTestCaseSystemPrompt() +
             PromptBuilder.BuildUserPrompt(content);

        var requestBody = new
        {
            contents = new[]
            {
            new
            {
                parts = new[]
                {
                    new { text = prompt }
                }
            }
        },
            generationConfig = new
            {
                temperature = 0.2,
                maxOutputTokens = 4000
            }
        };

        var response = await _httpClient.PostAsJsonAsync(
            $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}",
            requestBody);

        var json = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(json);

        var text = doc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();

        return JsonSerializer.Deserialize<TestCaseResult>(
            text!,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
    }
}