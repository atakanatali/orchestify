namespace Orchestify.Shared.Constants;

/// <summary>
/// Defines standard HTTP header names used throughout the Orchestify platform.
/// Centralizes header names to eliminate magic strings and ensure consistency.
/// </summary>
public static class HeaderNamesConstants
{
    /// <summary>
    /// Header for correlation ID tracking across requests.
    /// Used for distributed tracing and request correlation.
    /// </summary>
    public const string CorrelationId = "X-Correlation-ID";

    /// <summary>
    /// Header for requesting IDempotency-Key for idempotent operations.
    /// Ensures that duplicate requests do not result in duplicate operations.
    /// </summary>
    public const string IdempotencyKey = "Idempotency-Key";

    /// <summary>
    /// Header for tenant identification in multi-tenant scenarios.
    /// </summary>
    public const string TenantId = "X-Tenant-ID";

    /// <summary>
    /// Header for user identification in authenticated requests.
    /// </summary>
    public const string UserId = "X-User-ID";

    /// <summary>
    /// Header for request rate limiting information.
    /// </summary>
    public const string RateLimitRemaining = "X-RateLimit-Remaining";

    /// <summary>
    /// Header for rate limit reset timestamp.
    /// </summary>
    public const string RateLimitReset = "X-RateLimit-Reset";

    /// <summary>
    /// Header for retry-after duration in seconds.
    /// </summary>
    public const string RetryAfter = "Retry-After";

    /// <summary>
    /// Header for API version specification.
    /// </summary>
    public const string ApiVersion = "X-API-Version";

    /// <summary>
    /// Header for request trace ID for debugging.
    /// </summary>
    public const string TraceId = "X-Trace-ID";

    /// <summary>
    /// Header for pagination cursor.
    /// </summary>
    public const string Cursor = "X-Cursor";

    /// <summary>
    /// Header for pagination continuation token.
    /// </summary>
    public const string ContinuationToken = "X-Continuation-Token";
}
