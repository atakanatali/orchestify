namespace Orchestify.Shared.Logging;

/// <summary>
/// Defines a structured logging service for capturing application events, errors, and metrics.
/// Provides typed methods for different log levels with contextual metadata support.
/// All logs include correlation tracking and enrichment capabilities.
/// </summary>
public interface ILogService
{
    /// <summary>
    /// Logs an informational message with optional structured data.
    /// </summary>
    /// <param name="code">A code identifying the type of log event (e.g., ORC_API_REQUEST_RECEIVED).</param>
    /// <param name="message">The message to log.</param>
    /// <param name="data">Optional structured data for log enrichment.</param>
    void Info(string code, string message, IDictionary<string, object?>? data = null);

    /// <summary>
    /// Logs a warning message with optional structured data.
    /// </summary>
    /// <param name="code">A code identifying the type of warning (e.g., ORC_CACHE_MISS).</param>
    /// <param name="message">The warning message to log.</param>
    /// <param name="data">Optional structured data for log enrichment.</param>
    void Warn(string code, string message, IDictionary<string, object?>? data = null);

    /// <summary>
    /// Logs an error with full exception details including type, message, and stack trace.
    /// </summary>
    /// <param name="exception">The exception that occurred.</param>
    /// <param name="code">The error code for categorization (e.g., ORC_CORE_UNHANDLED_ERROR).</param>
    /// <param name="message">A descriptive error message safe for client consumption.</param>
    /// <param name="data">Optional structured data for log enrichment.</param>
    void Error(
        Exception exception,
        string code,
        string message,
        IDictionary<string, object?>? data = null);

    /// <summary>
    /// Logs a debug-level message for development diagnostics.
    /// </summary>
    /// <param name="code">A code identifying the debug event (e.g., ORC_DB_QUERY_EXECUTED).</param>
    /// <param name="message">The debug message to log.</param>
    /// <param name="data">Optional structured data for log enrichment.</param>
    void Debug(string code, string message, IDictionary<string, object?>? data = null);
}
