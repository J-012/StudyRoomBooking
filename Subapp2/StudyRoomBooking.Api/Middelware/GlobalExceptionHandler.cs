using Microsoft.AspNetCore.Diagnostics;

namespace StudyRoomBooking.Api.Middleware;

// Turns exceptions thrown by the services into JSON error responses, so the
// controllers do not need a try/catch in every action. All errors use the
// same shape: { "message": "..." }.
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        int statusCode;
        string message;

        // Only the plain ArgumentException is a broken domain rule thrown by
        // our services. Subclasses such as ArgumentNullException come from
        // programming errors and must end up as 500 below.
        if (exception.GetType() == typeof(ArgumentException))
        {
            statusCode = StatusCodes.Status400BadRequest;
            message = exception.Message;

            _logger.LogWarning(
                "{Method} {Path} was rejected: {Reason}",
                httpContext.Request.Method,
                httpContext.Request.Path,
                exception.Message
            );
        }
        else if (exception is UnauthorizedAccessException)
        {
            statusCode = StatusCodes.Status403Forbidden;
            message = exception.Message;

            _logger.LogWarning(
                "{Method} {Path} was forbidden: {Reason}",
                httpContext.Request.Method,
                httpContext.Request.Path,
                exception.Message
            );
        }
        else
        {
            statusCode = StatusCodes.Status500InternalServerError;
            message = "An unexpected error occurred.";

            _logger.LogError(
                exception,
                "Unhandled exception for {Method} {Path}.",
                httpContext.Request.Method,
                httpContext.Request.Path
            );
        }

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(
            new { message },
            cancellationToken
        );

        return true;
    }
}
