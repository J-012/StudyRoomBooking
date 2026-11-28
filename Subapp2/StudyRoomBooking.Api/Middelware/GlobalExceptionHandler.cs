using Microsoft.AspNetCore.Diagnostics;

namespace StudyRoomBooking.Api.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Only the plain ArgumentException is thrown by our services on purpose.
        (int statusCode, string message) = exception switch
        {
            _ when exception.GetType() == typeof(ArgumentException)
                => (StatusCodes.Status400BadRequest, exception.Message),
            UnauthorizedAccessException
                => (StatusCodes.Status403Forbidden, exception.Message),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception for {Method} {Path}.",
                httpContext.Request.Method, httpContext.Request.Path);
        }
        else
        {
            _logger.LogWarning("{Method} {Path} was rejected with {StatusCode}: {Reason}",
                httpContext.Request.Method, httpContext.Request.Path, statusCode, message);
        }

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(new { message }, cancellationToken);

        return true;
    }
}
