namespace Orchestify.Shared.Logging;

/// <summary>
/// Defines a structured logging service for capturing application events, errors, and metrics.
/// Provides typed methods for different log levels with contextual metadata support.
/// All logs include correlation tracking and enrichment capabilities.
/// </summary>
public interface ILogService
{
    /// <summary>
    /// Logs an informational message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="context">Optional contextual data for log enrichment.</param>
    /// <param name="correlationId">Optional correlation ID for request tracing.</param>
    void Information(string message, IDictionary<string, object?>? context = null, string? correlationId = null);

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    /// <param name="message">The warning message to log.</param>
    /// <param name="context">Optional contextual data for log enrichment.</param>
    /// <param name="correlationId">Optional correlation ID for request tracing.</param>
    void Warning(string message, IDictionary<string, object?>? context = null, string? correlationId = null);

    /// <summary>
    /// Logs an error with full exception details.
    /// </summary>
    /// <param name="exception">The exception that occurred.</param>
    /// <param name="errorCode">The error code for categorization (e.g., ORC_CORE_UNHANDLED_ERROR).</param>
    /// <param name="message">A descriptive error message safe for client consumption.</param>
    /// <param name="context">Optional contextual data for log enrichment.</param>
    /// <param name="correlationId">Optional correlation ID for request tracing.</param>
    void Error(
        Exception exception,
        string errorCode,
        string message,
        IDictionary<string, object?>? context = null,
        string? correlationId = null);

    /// <summary>
    /// Logs a debug-level message for development diagnostics.
    /// </summary>
    /// <param name="message">The debug message to log.</param>
    /// <param name="context">Optional contextual data for log enrichment.</param>
    /// <param name="correlationId">Optional correlation ID for request tracing.</param>
    void Debug(string message, IDictionary<string, object?>? context = null, string? correlationId = null);

    /// <summary>
    /// Creates a logging scope with additional context that applies to all log entries within.
    /// Returns an IDisposable that ends the scope when disposed.
    /// </summary>
    /// <param name="context">The contextual data to add to the scope.</param>
    /// <returns>An IDisposable that ends the scope when disposed.</returns>
    IDisposable BeginScope(IDictionary<string, object?> context);

    /// <summary>
    /// Creates a logging scope with a named identifier and additional context.
    /// Returns an IDisposable that ends the scope when disposed.
    /// </summary>
    /// <param name="scopeName">The name identifying this scope.</param>
    /// <param name="context">The contextual data to add to the scope.</param>
    /// <returns>An IDisposable that ends the scope when disposed.</returns>
    IDisposable BeginScope(string scopeName, IDictionary<string, object?>? context = null);
}
