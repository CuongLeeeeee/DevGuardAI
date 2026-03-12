namespace DevGuardAI.BLL.Exceptions;

/// <summary>
/// Thrown when the user is not authenticated. Maps to HTTP 401.
/// </summary>
public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message = "You are not authorized to access this resource.")
        : base(message, 401, "UNAUTHORIZED")
    {
    }
}

/// <summary>
/// Thrown when the user is authenticated but lacks permission. Maps to HTTP 403.
/// </summary>
public class ForbiddenException : AppException
{
    public ForbiddenException(string message = "You do not have permission to perform this action.")
        : base(message, 403, "FORBIDDEN")
    {
    }
}