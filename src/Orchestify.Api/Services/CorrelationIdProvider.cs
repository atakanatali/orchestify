using Microsoft.AspNetCore.Http;
using Orchestify.Shared.Correlation;

namespace Orchestify.Api.Services;

/// <summary>
/// ASP.NET Core implementation of <see cref="Shared.Correlation.ICorrelationIdProvider"/>.
/// Provides access to correlation IDs from the current HttpContext.
/// </summary>
public sealed class CorrelationIdProvider : Shared.Correlation.ICorrelationIdProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="CorrelationIdProvider"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="httpContextAccessor"/> is null.
    /// </exception>
    public CorrelationIdProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor
            ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    /// <summary>
    /// Gets the current correlation ID from the HttpContext.
    /// </summary>
    /// <returns>
    /// The correlation ID for the current request, or null if not available.
    /// </returns>
    public string? GetCorrelationId()
    {
        HttpContext? httpContext = _httpContextAccessor.HttpContext;

        if (httpContext?.Items.TryGetValue(CorrelationIdConstants.HttpContextKey, out var correlationId) == true
            && correlationId is string correlationIdString)
        {
            return correlationIdString;
        }

        return null;
    }

    /// <summary>
    /// Gets the current request ID from the HttpContext.
    /// </summary>
    /// <returns>
    /// The request ID for the current request, or null if not available.
    /// </returns>
    public string? GetRequestId()
    {
        HttpContext? httpContext = _httpContextAccessor.HttpContext;

        if (httpContext?.Items.TryGetValue(CorrelationIdConstants.HttpRequestIdKey, out var requestId) == true
            && requestId is string requestIdString)
        {
            return requestIdString;
        }

        return null;
    }
}
