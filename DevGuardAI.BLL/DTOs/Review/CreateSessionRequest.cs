public class CreateSessionRequest
{
    public string Title { get; set; } = string.Empty;

    /// <summary>"Review" hoặc "TestCase"</summary>
    public string SessionType { get; set; } = string.Empty;
}