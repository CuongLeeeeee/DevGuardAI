namespace DevGuardAI.BLL.Exceptions;

/// <summary>
/// Thrown when the Gemini AI API returns an error or an unexpected response.
/// Maps to HTTP 502 Bad Gateway.
/// </summary>
public class GeminiApiException : AppException
{
    public int? GeminiStatusCode { get; }

    public GeminiApiException(string message, int? geminiStatusCode = null)
        : base(message, 502, "GEMINI_API_ERROR")
    {
        GeminiStatusCode = geminiStatusCode;
    }

    public GeminiApiException(System.Net.HttpStatusCode statusCode)
        : base($"Gemini API returned an unexpected status: {(int)statusCode} {statusCode}.", 502, "GEMINI_API_ERROR")
    {
        GeminiStatusCode = (int)statusCode;
    }
}