namespace Orchestify.Shared.Correlation;

/// <summary>
/// Provides access to the current request's correlation ID.
/// Enables services to access correlation context without direct HttpContext dependencies.
/// </summary>
public interface ICorrelationIdProvider
{
    /// <summary>
    /// Gets the current correlation ID for the ongoing request.
    /// Returns null if correlation tracking is not enabled or no request is in progress.
    /// </summary>
    /// <returns>The current correlation ID, or null if unavailable.</returns>
    string? GetCorrelationId();

    /// <summary>
    /// Gets the current request ID for the ongoing request.
    /// Returns null if no request is in progress.
    /// </summary>
    /// <returns>The current request ID, or null if unavailable.</returns>
    string? GetRequestId();
}
