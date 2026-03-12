using DevGuardAI.BLL.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace DevGuardAI.API.Middleware;

/// <summary>
/// Catches all unhandled exceptions and returns a consistent JSON error response.
/// Register in Program.cs before UseRouting: app.UseMiddleware&lt;ExceptionHandlingMiddleware&gt;();
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, errorCode, message, errors) = MapException(exception);

        // Log at appropriate level
        if (statusCode >= 500)
            _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
        else
            _logger.LogWarning("Handled exception [{ErrorCode}]: {Message}", errorCode, exception.Message);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = errors != null
            ? new ErrorResponse(statusCode, errorCode, message, errors)
            : new ErrorResponse(statusCode, errorCode, message);

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, _jsonOptions));
    }

    private static (int statusCode, string errorCode, string message, IDictionary<string, string[]>? errors)
        MapException(Exception exception)
    {
        return exception switch
        {
            // ── Application exceptions ──────────────────────────────────────
            ValidationException ve =>
                (ve.StatusCode, ve.ErrorCode, ve.Message, ve.Errors.Count > 0 ? new Dictionary<string, string[]>(ve.Errors) : null),

            NotFoundException nfe =>
                (nfe.StatusCode, nfe.ErrorCode, nfe.Message, null),

            UnauthorizedException ue =>
                (ue.StatusCode, ue.ErrorCode, ue.Message, null),

            ForbiddenException fe =>
                (fe.StatusCode, fe.ErrorCode, fe.Message, null),

            GeminiApiException ge =>
                (ge.StatusCode, ge.ErrorCode, ge.Message, null),

            // ── .NET built-ins ──────────────────────────────────────────────
            UnauthorizedAccessException =>
                (401, "UNAUTHORIZED", "You are not authorized to access this resource.", null),

            KeyNotFoundException knfe =>
                (404, "NOT_FOUND", knfe.Message, null),

            ArgumentException ae =>
                (400, "BAD_REQUEST", ae.Message, null),

            // ── Fallback ────────────────────────────────────────────────────
            _ => (500, "INTERNAL_SERVER_ERROR", "An unexpected error occurred. Please try again later.", null)
        };
    }
}

/// <summary>
/// Standard error response body sent to the client.
/// </summary>
public record ErrorResponse(
    int Status,
    string ErrorCode,
    string Message,
    IDictionary<string, string[]>? Errors = null);