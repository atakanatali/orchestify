namespace Orchestify.Shared.Correlation;

/// <summary>
/// Constants for correlation ID handling across the Orchestify platform.
/// Used for distributed tracing and request correlation.
/// </summary>
public static class CorrelationIdConstants
{
    /// <summary>
    /// The key used to store and retrieve the correlation ID from the HttpContext.
    /// This key is used by middleware and services to access the current request's correlation ID.
    /// </summary>
    public const string HttpContextKey = "CorrelationId";

    /// <summary>
    /// The key used to store and retrieve the request ID from the HttpContext.
    /// This uniquely identifies each request, even within the same correlation scope.
    /// </summary>
    public const string HttpRequestIdKey = "RequestId";
}
