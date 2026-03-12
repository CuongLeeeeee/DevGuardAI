namespace DevGuardAI.BLL.Exceptions;

/// <summary>
/// Base class for all application-specific exceptions.
/// </summary>
public abstract class AppException : Exception
{
    public int StatusCode { get; }
    public string ErrorCode { get; }

    protected AppException(string message, int statusCode, string errorCode)
        : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }
}