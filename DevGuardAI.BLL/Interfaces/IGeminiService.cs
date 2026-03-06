public interface IGeminiService
{
    public Task<ReviewResult?> ReviewCode(string content);
    public Task<TestCaseResult?> GenerateTestCases(string content);
    Task<ConversationReviewResult> ReviewWithContext(Guid sessionId, string userInput);
    Task<ConversationTestCaseResult> GenerateTestCasesWithContext(Guid sessionId, string userInput);
}