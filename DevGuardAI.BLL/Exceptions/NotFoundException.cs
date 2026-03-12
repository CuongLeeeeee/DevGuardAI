namespace DevGuardAI.BLL.Exceptions;

/// <summary>
/// Thrown when a requested resource is not found. Maps to HTTP 404.
/// </summary>
public class NotFoundException : AppException
{
    public NotFoundException(string resourceName, object id)
        : base($"{resourceName} with id '{id}' was not found.", 404, "NOT_FOUND")
    {
    }

    public NotFoundException(string message)
        : base(message, 404, "NOT_FOUND")
    {
    }
}