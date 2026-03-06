using DevGuardAI.BLL.PromptBuilders;
using DevGuardAI.DAL.Entities;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json;

public class GeminiService : IGeminiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly IChatService _chatService;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public GeminiService(
        HttpClient httpClient,
        IConfiguration config,
        IChatService chatService)
    {
        _httpClient = httpClient;
        _config = config;
        _chatService = chatService;
    }

    // -------------------------------------------------------------------------
    // Single-shot (không có conversation context) — giữ nguyên như cũ
    // -------------------------------------------------------------------------

    public async Task<ReviewResult?> ReviewCode(string content)
    {
        var prompt = PromptBuilder.BuildSystemPrompt()
                   + PromptBuilder.BuildUserPrompt(content);

        var text = await CallGeminiAsync(prompt, maxOutputTokens: 2000);
        return JsonSerializer.Deserialize<ReviewResult>(text, _jsonOptions);
    }

    public async Task<TestCaseResult?> GenerateTestCases(string content)
    {
        var prompt = PromptBuilder.BuildTestCaseSystemPrompt()
                   + PromptBuilder.BuildUserPrompt(content);

        var text = await CallGeminiAsync(prompt, maxOutputTokens: 4000);
        return JsonSerializer.Deserialize<TestCaseResult>(text, _jsonOptions);
    }

    // -------------------------------------------------------------------------
    // Conversation-aware: Code Review
    // -------------------------------------------------------------------------

    public async Task<ConversationReviewResult> ReviewWithContext(Guid sessionId, string userInput)
    {
        // 1. Lấy context từ DB
        var (history, contextSummary) = await GetContextFromDb(sessionId);

        // 2. Build prompt
        var prompt = PromptBuilder.BuildConversationPrompt(userInput, contextSummary, history);

        // 3. Gọi Gemini
        var text = await CallGeminiAsync(prompt, maxOutputTokens: 3000);

        // 4. Parse response
        var result = ParseReviewResponse(text);

        // 5. Lưu turn + cập nhật summary xuống DB
        var aiContent = result.Review?.Summary ?? result.Answer ?? string.Empty;
        await _chatService.SaveTurnAsync(sessionId, userInput, aiContent);
        await _chatService.UpdateContextSummaryAsync(sessionId, result.UpdatedContextSummary);

        return result;
    }

    // -------------------------------------------------------------------------
    // Conversation-aware: Test Case Generation
    // -------------------------------------------------------------------------

    public async Task<ConversationTestCaseResult> GenerateTestCasesWithContext(Guid sessionId, string userInput)
    {
        // 1. Lấy context từ DB
        var (history, contextSummary) = await GetContextFromDb(sessionId);

        // 2. Build prompt
        var prompt = PromptBuilder.BuildConversationTestCasePrompt(userInput, contextSummary, history);

        // 3. Gọi Gemini
        var text = await CallGeminiAsync(prompt, maxOutputTokens: 4000);

        // 4. Parse response
        var result = ParseTestCaseResponse(text);

        // 5. Lưu turn + cập nhật summary xuống DB
        var aiContent = result.Answer
                     ?? result.TestCases?.TestCases?.FirstOrDefault()?.Description
                     ?? string.Empty;
        await _chatService.SaveTurnAsync(sessionId, userInput, aiContent);
        await _chatService.UpdateContextSummaryAsync(sessionId, result.UpdatedContextSummary);

        return result;
    }

    // -------------------------------------------------------------------------
    // Private helpers
    // -------------------------------------------------------------------------

    /// <summary>
    /// Lấy 6 messages gần nhất và contextSummary của session từ DB,
    /// map sang ConversationTurn để truyền vào PromptBuilder.
    /// </summary>
    private async Task<(IEnumerable<ConversationTurn> history, string? contextSummary)> GetContextFromDb(Guid sessionId)
    {
        var session = await _chatService.GetSessionWithMessagesAsync(sessionId);

        if (session == null)
            throw new KeyNotFoundException($"Session {sessionId} not found.");

        var history = session.Messages
            .OrderBy(m => m.CreatedAt)
            .TakeLast(6)
            .Select(m => new ConversationTurn
            {
                Role = m.Role == "User" ? "user" : "assistant",
                Content = m.Content
            });

        return (history, session.ContextSummary);
    }

    /// <summary>
    /// Parse JSON response của Gemini cho conversation review.
    /// Phân biệt "review" và "followup" dựa vào field "type".
    /// </summary>
    private ConversationReviewResult ParseReviewResponse(string text)
    {
        using var doc = JsonDocument.Parse(text);
        var root = doc.RootElement;

        var type = root.TryGetProperty("type", out var typeProp)
            ? typeProp.GetString() : "review";

        var updatedSummary = root.TryGetProperty("updatedContextSummary", out var summaryProp)
            ? summaryProp.GetString() ?? string.Empty : string.Empty;

        if (type == "followup")
        {
            return new ConversationReviewResult
            {
                Answer = root.TryGetProperty("answer", out var ap) ? ap.GetString() : null,
                UpdatedContextSummary = updatedSummary
            };
        }

        // type == "review"
        return new ConversationReviewResult
        {
            Review = JsonSerializer.Deserialize<ReviewResult>(text, _jsonOptions),
            UpdatedContextSummary = updatedSummary
        };
    }

    /// <summary>
    /// Parse JSON response của Gemini cho conversation test case.
    /// Phân biệt "testcases" và "followup" dựa vào field "type".
    /// </summary>
    private ConversationTestCaseResult ParseTestCaseResponse(string text)
    {
        using var doc = JsonDocument.Parse(text);
        var root = doc.RootElement;

        var type = root.TryGetProperty("type", out var typeProp)
            ? typeProp.GetString() : "testcases";

        var updatedSummary = root.TryGetProperty("updatedContextSummary", out var summaryProp)
            ? summaryProp.GetString() ?? string.Empty : string.Empty;

        if (type == "followup")
        {
            return new ConversationTestCaseResult
            {
                Answer = root.TryGetProperty("answer", out var ap) ? ap.GetString() : null,
                UpdatedContextSummary = updatedSummary
            };
        }

        // type == "testcases"
        return new ConversationTestCaseResult
        {
            TestCases = JsonSerializer.Deserialize<TestCaseResult>(text, _jsonOptions),
            UpdatedContextSummary = updatedSummary
        };
    }

    /// <summary>
    /// Gọi Gemini API và trả về raw text từ response.
    /// </summary>
    private async Task<string> CallGeminiAsync(string prompt, int maxOutputTokens = 2000)
    {
        var apiKey = _config["Gemini:ApiKey"];

        var requestBody = new
        {
            contents = new[]
            {
                new { parts = new[] { new { text = prompt } } }
            },
            generationConfig = new
            {
                maxOutputTokens,
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
        await Task.Delay(500);

        using var doc = JsonDocument.Parse(json);
        return doc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString()!;
    }
}