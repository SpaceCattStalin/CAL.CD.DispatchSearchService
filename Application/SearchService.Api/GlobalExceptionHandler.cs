using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IProblemDetailsService _problemDetails;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IProblemDetailsService problemDetails)
    {
        _logger = logger;
        _problemDetails = problemDetails;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception. TraceId: {TraceId}", httpContext.TraceIdentifier);

        ProblemDetails problem = new();

        if (exception is ValidationException validationException)
        {
            var errors = validationException.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(element => element.ErrorMessage).ToArray()
                );

            problem = new ValidationProblemDetails
            {
                Status = 400,
                Title = "Validation Error",
                Type = exception.GetType().Name,
                Errors = errors,
                Instance = httpContext.Request.Path
            };
        }
        else
        {
            var (status, title) = exception switch
            {
                KeyNotFoundException => (StatusCodes.Status404NotFound, "Resource Not Found"),
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
                ArgumentException => (StatusCodes.Status400BadRequest, "Invalid Request"),
                _ => (StatusCodes.Status500InternalServerError, "Server Error")
            };

            problem = new ProblemDetails
            {
                Status = status,
                Title = title,
                Type = exception.GetType().Name,
                Detail = httpContext.RequestServices
                                     .GetRequiredService<IHostEnvironment>()
                                     .IsDevelopment()
                         ? exception.Message
                         : null,
                Instance = httpContext.Request.Path
            };
        }

        problem.Extensions["traceId"] = httpContext.TraceIdentifier;
        problem.Extensions["timestamp"] = DateTime.UtcNow;

        await _problemDetails.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problem,
        });

        return true;
    }
}