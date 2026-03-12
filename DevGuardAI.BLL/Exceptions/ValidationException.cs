namespace DevGuardAI.BLL.Exceptions;

/// <summary>
/// Thrown when input validation fails. Maps to HTTP 400.
/// </summary>
public class ValidationException : AppException
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public ValidationException(string field, string message)
        : base("One or more validation errors occurred.", 400, "VALIDATION_ERROR")
    {
        Errors = new Dictionary<string, string[]>
        {
            { field, new[] { message } }
        };
    }

    public ValidationException(IDictionary<string, string[]> errors)
        : base("One or more validation errors occurred.", 400, "VALIDATION_ERROR")
    {
        Errors = new Dictionary<string, string[]>(errors);
    }

    public ValidationException(string message)
        : base(message, 400, "VALIDATION_ERROR")
    {
        Errors = new Dictionary<string, string[]>();
    }
}