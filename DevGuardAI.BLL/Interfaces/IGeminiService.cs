public interface IGeminiService
{
    public Task<ReviewResult?> ReviewCode(string content);
    public Task<TestCaseResult?> GenerateTestCases(string content);
}