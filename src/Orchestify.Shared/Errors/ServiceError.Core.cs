namespace Orchestify.Shared.Errors;

/// <summary>
/// Core error codes for the Orchestify platform.
/// Covers general platform errors not specific to any domain.
/// </summary>
public static partial class ServiceError
{
    /// <summary>
    /// Core domain identifier for error code formatting.
    /// </summary>
    private const string CoreDomain = "CORE";

    /// <summary>
    /// Error codes and messages for core platform functionality.
    /// </summary>
    public static class Core
    {
        /// <summary>
        /// Unknown or unexpected error.
        /// </summary>
        public static string Unknown => FormatErrorCode(CoreDomain, "UNKNOWN");

        /// <summary>
        /// Invalid input provided.
        /// </summary>
        public static string InvalidInput => FormatErrorCode(CoreDomain, "INVALID_INPUT");

        /// <summary>
        /// Required value is null or empty.
        /// </summary>
        public static string RequiredValueMissing => FormatErrorCode(CoreDomain, "REQUIRED_VALUE_MISSING");

        /// <summary>
        /// Operation timed out.
        /// </summary>
        public static string Timeout => FormatErrorCode(CoreDomain, "TIMEOUT");

        /// <summary>
        /// Rate limit exceeded.
        /// </summary>
        public static string RateLimitExceeded => FormatErrorCode(CoreDomain, "RATE_LIMIT_EXCEEDED");

        /// <summary>
        /// Service temporarily unavailable.
        /// </summary>
        public static string ServiceUnavailable => FormatErrorCode(CoreDomain, "SERVICE_UNAVAILABLE");

        /// <summary>
        /// Unauthorized access attempt.
        /// </summary>
        public static string Unauthorized => FormatErrorCode(CoreDomain, "UNAUTHORIZED");

        /// <summary>
        /// Forbidden operation.
        /// </summary>
        public static string Forbidden => FormatErrorCode(CoreDomain, "FORBIDDEN");

        /// <summary>
        /// Resource not found.
        /// </summary>
        public static string NotFound => FormatErrorCode(CoreDomain, "NOT_FOUND");

        /// <summary>
        /// Conflict with existing resource.
        /// </summary>
        public static string Conflict => FormatErrorCode(CoreDomain, "CONFLICT");

        /// <summary>
        /// Database operation failed.
        /// </summary>
        public static string DatabaseError => FormatErrorCode(CoreDomain, "DATABASE_ERROR");

        /// <summary>
        /// External provider communication failed.
        /// </summary>
        public static string ProviderError => FormatErrorCode(CoreDomain, "PROVIDER_ERROR");

        /// <summary>
        /// Validation failed.
        /// </summary>
        public static string ValidationFailed => FormatErrorCode(CoreDomain, "VALIDATION_FAILED");

        /// <summary>
        /// Correlation ID missing for tracked operation.
        /// </summary>
        public static string CorrelationIdMissing => FormatErrorCode(CoreDomain, "CORRELATION_ID_MISSING");

        /// <summary>
        /// Idempotency key missing for idempotent operation.
        /// </summary>
        public static string IdempotencyKeyMissing => FormatErrorCode(CoreDomain, "IDEMPOTENCY_KEY_MISSING");

        /// <summary>
        /// Operation already processed with this idempotency key.
        /// </summary>
        public static string DuplicateOperation => FormatErrorCode(CoreDomain, "DUPLICATE_OPERATION");
    }
}
