using Microsoft.AspNetCore.Http;
using Orchestify.Shared.Constants;
using Orchestify.Shared.Correlation;

namespace Orchestify.Api.Middlewares;

/// <summary>
/// Middleware for correlation ID management across the request pipeline.
/// Ensures every request has a correlation ID for distributed tracing.
/// </summary>
/// <remarks>
/// This middleware should be the first in the pipeline to ensure correlation
/// tracking is available for all subsequent middleware and services.
/// </remarks>
public sealed class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes a new instance of the <see cref="CorrelationIdMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the request pipeline.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="next"/> is null.
    /// </exception>
    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
    }

    /// <summary>
    /// Processes the incoming request by ensuring a correlation ID is present
    /// and adding it to the response headers for client-side tracking.
    /// </summary>
    /// <param name="context">The HTTP context for the current request.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        // Extract or generate correlation ID
        string correlationId = ExtractOrGenerateCorrelationId(context);

        // Generate unique request ID
        string requestId = CorrelationIdGenerator.Generate();

        // Store in HttpContext for access by other components
        context.Items[CorrelationIdConstants.HttpContextKey] = correlationId;
        context.Items[CorrelationIdConstants.HttpRequestIdKey] = requestId;

        // Add to response headers for client-side tracking
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HttpHeaderConstants.CorrelationId] = correlationId;
            context.Response.Headers[HttpHeaderConstants.RequestId] = requestId;
            return Task.CompletedTask;
        });

        await _next(context);
    }

    /// <summary>
    /// Extracts the correlation ID from the request header or generates a new one.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>The correlation ID to use for this request.</returns>
    private static string ExtractOrGenerateCorrelationId(HttpContext context)
    {
        // Check if the client provided a correlation ID
        if (context.Request.Headers.TryGetValue(HttpHeaderConstants.CorrelationId, out var headerValue))
        {
            string? providedCorrelationId = headerValue.ToString();

            // Validate and normalize the provided correlation ID
            string? normalizedCorrelationId = CorrelationIdGenerator.Normalize(providedCorrelationId);
            if (!string.IsNullOrWhiteSpace(normalizedCorrelationId))
            {
                return normalizedCorrelationId;
            }
        }

        // Generate a new correlation ID if none was provided or invalid
        return CorrelationIdGenerator.Generate();
    }
}
