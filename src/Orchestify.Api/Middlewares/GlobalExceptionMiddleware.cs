using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Orchestify.Contracts.Shared;
using Orchestify.Shared.Constants;
using Orchestify.Shared.Correlation;
using Orchestify.Shared.Errors;
using Orchestify.Shared.Logging;

namespace Orchestify.Api.Middlewares;

/// <summary>
/// Global exception handling middleware that captures all unhandled exceptions,
/// logs them with full context, and returns standardized error responses.
/// </summary>
/// <remarks>
/// This middleware must be registered early in the application pipeline
/// to ensure it can catch exceptions from all subsequent middleware.
/// It handles correlation ID tracking, structured logging, and safe error responses.
/// </remarks>
public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogService _logService;
    private readonly ICorrelationIdProvider _correlationIdProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="GlobalExceptionMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the request pipeline.</param>
    /// <param name="logService">The logging service for error capture.</param>
    /// <param name="correlationIdProvider">The correlation ID provider for request tracking.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when any required parameter is null.
    /// </exception>
    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogService logService,
        ICorrelationIdProvider correlationIdProvider)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        _correlationIdProvider = correlationIdProvider ?? throw new ArgumentNullException(nameof(correlationIdProvider));
    }

    /// <summary>
    /// Processes the incoming request and handles any unhandled exceptions.
    /// </summary>
    /// <param name="context">The HTTP context for the current request.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    /// <summary>
    /// Handles an uncaught exception by logging it and returning a standardized error response.
    /// </summary>
    /// <param name="context">The HTTP context for the current request.</param>
    /// <param name="exception">The unhandled exception.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Gather context data for logging
        string correlationId = _correlationIdProvider.GetCorrelationId() ?? CorrelationIdGenerator.Generate();
        string requestId = _correlationIdProvider.GetRequestId() ?? string.Empty;
        string method = context.Request.Method;
        string path = context.Request.Path;
        string queryString = context.Request.QueryString.HasValue ? context.Request.QueryString.Value! : string.Empty;
        string userAgent = context.Request.Headers[HttpHeaderConstants.UserAgent].ToString();
        string? clientIp = GetClientIpAddress(context);

        // Build contextual metadata for the log
        Dictionary<string, object?> logContext = new(StringComparer.Ordinal)
        {
            ["HttpMethod"] = method,
            ["RequestPath"] = path,
            ["QueryString"] = queryString,
            ["UserAgent"] = userAgent,
            ["ClientIp"] = clientIp,
            ["RequestId"] = requestId,
            ["ExceptionType"] = exception.GetType().FullName,
            ["ExceptionMessage"] = exception.Message
        };

        // Include inner exception details if present
        if (exception.InnerException is not null)
        {
            logContext["InnerExceptionType"] = exception.InnerException.GetType().FullName;
            logContext["InnerExceptionMessage"] = exception.InnerException.Message;
        }

        // Log the error with full context
        _logService.Error(
            exception,
            ServiceError.Core.UnhandledError,
            "An unhandled exception occurred during request processing.",
            logContext,
            correlationId);

        // Create the standardized error response
        ApiErrorResponseDto errorResponse = new(
            code: ServiceError.Core.UnhandledError,
            message: "An unexpected error occurred. Please try again later.",
            correlationId: correlationId);

        // Set response headers and status code
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";
        context.Response.Headers[HttpHeaderConstants.CorrelationId] = correlationId;
        context.Response.Headers[HttpHeaderConstants.RequestId] = requestId;

        // Serialize and write the response
        string json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        });

        return context.Response.WriteAsync(json);
    }

    /// <summary>
    /// Extracts the client IP address from the HTTP context.
    /// Handles scenarios where the application is behind a proxy or load balancer.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>The client IP address, or null if unavailable.</returns>
    private static string? GetClientIpAddress(HttpContext context)
    {
        // Check for X-Forwarded-For header (proxy/load balancer scenario)
        if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
        {
            string forwardedFor = context.Request.Headers["X-Forwarded-For"].ToString();
            // X-Forwarded-For may contain multiple IPs; the first one is the original client
            int firstCommaIndex = forwardedFor.IndexOf(',', StringComparison.Ordinal);
            return firstCommaIndex > 0
                ? forwardedFor[..firstCommaIndex].Trim()
                : forwardedFor;
        }

        // Check for X-Real-IP header (nginx and some proxies)
        if (context.Request.Headers.ContainsKey("X-Real-IP"))
        {
            return context.Request.Headers["X-Real-IP"].ToString();
        }

        // Fall back to the remote IP address
        return context.Connection.RemoteIpAddress?.ToString();
    }
}
