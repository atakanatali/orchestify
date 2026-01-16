using Serilog;
using Serilog.Events;
using Orchestify.Shared.Constants;
using Orchestify.Shared.Logging;
using Orchestify.Shared.Correlation;

namespace Orchestify.Infrastructure.Logging;

/// <summary>
/// Serilog-based implementation of <see cref="ILogService"/>.
/// Provides structured logging with enrichment for correlation tracking, service metadata,
/// and full exception serialization including stack traces.
/// </summary>
public sealed class SerilogLogService : ILogService
{
    private readonly ILogger _logger;
    private readonly string _serviceName;
    private readonly ICorrelationIdProvider? _correlationIdProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="SerilogLogService"/> class.
    /// </summary>
    /// <param name="serviceName">The name of the service generating logs (e.g., "orchestify-api").</param>
    /// <param name="correlationIdProvider">Optional correlation ID provider for request tracing.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="serviceName"/> is null or empty.
    /// </exception>
    public SerilogLogService(string serviceName, ICorrelationIdProvider? correlationIdProvider = null)
    {
        if (string.IsNullOrWhiteSpace(serviceName))
        {
            throw new ArgumentException("Service name cannot be null or empty.", nameof(serviceName));
        }

        _serviceName = serviceName;
        _correlationIdProvider = correlationIdProvider;
        _logger = Log.ForContext(SerilogLogServiceConstants.SourceContextPropertyName, "Orchestify.Infrastructure.Logging.SerilogLogService")
                     .ForContext(SerilogLogServiceConstants.ServiceNamePropertyName, serviceName);
    }

    /// <inheritdoc/>
    public void Info(string code, string message, IDictionary<string, object?>? data = null)
    {
        var enrichedLogger = EnrichLogger();
        enrichedLogger = EnrichWithData(enrichedLogger, data);

        enrichedLogger.Information(
            "[{Code}] {Message}",
            EscapeBraces(code),
            EscapeBraces(message));
    }

    /// <inheritdoc/>
    public void Warn(string code, string message, IDictionary<string, object?>? data = null)
    {
        var enrichedLogger = EnrichLogger();
        enrichedLogger = EnrichWithData(enrichedLogger, data);

        enrichedLogger.Warning(
            "[{Code}] {Message}",
            EscapeBraces(code),
            EscapeBraces(message));
    }

    /// <inheritdoc/>
    public void Error(Exception exception, string code, string message, IDictionary<string, object?>? data = null)
    {
        if (exception is null)
        {
            throw new ArgumentNullException(nameof(exception));
        }

        var enrichedLogger = EnrichLogger();
        enrichedLogger = EnrichWithData(enrichedLogger, data);
        enrichedLogger = EnrichWithException(enrichedLogger, exception);

        enrichedLogger.Error(
            exception,
            "[{Code}] {Message}",
            EscapeBraces(code),
            EscapeBraces(message));
    }

    /// <inheritdoc/>
    public void Debug(string code, string message, IDictionary<string, object?>? data = null)
    {
        var enrichedLogger = EnrichLogger();
        enrichedLogger = EnrichWithData(enrichedLogger, data);

        enrichedLogger.Debug(
            "[{Code}] {Message}",
            EscapeBraces(code),
            EscapeBraces(message));
    }

    /// <summary>
    /// Enriches the logger with correlation ID if available.
    /// </summary>
    private ILogger EnrichLogger()
    {
        var logger = _logger;

        if (_correlationIdProvider is not null)
        {
            string? correlationId = _correlationIdProvider.GetCorrelationId();
            if (!string.IsNullOrWhiteSpace(correlationId))
            {
                logger = logger.ForContext(LogConstants.CorrelationIdPropertyName, correlationId);
            }
        }

        return logger;
    }

    /// <summary>
    /// Enriches the logger with structured data.
    /// </summary>
    private ILogger EnrichWithData(ILogger logger, IDictionary<string, object?>? data)
    {
        if (data is null || data.Count == 0)
        {
            return logger;
        }

        foreach (var kvp in data)
        {
            logger = logger.ForContext(kvp.Key, kvp.Value?.ToString() ?? "null");
        }

        return logger;
    }

    /// <summary>
    /// Enriches the logger with exception details.
    /// </summary>
    private ILogger EnrichWithException(ILogger logger, Exception exception)
    {
        return logger
            .ForContext(LogConstants.ExceptionTypePropertyName, exception.GetType().FullName)
            .ForContext(LogConstants.ExceptionMessagePropertyName, exception.Message)
            .ForContext(LogConstants.ExceptionDetailsPropertyName, SerializeException(exception));
    }

    /// <summary>
    /// Serializes exception details for structured logging.
    /// </summary>
    private string SerializeException(Exception exception)
    {
        var details = new System.Text.StringBuilder();
        SerializeExceptionRecursive(exception, details, 0);
        return details.ToString();
    }

    /// <summary>
    /// Recursively serializes exception and inner exceptions.
    /// </summary>
    private void SerializeExceptionRecursive(Exception exception, System.Text.StringBuilder details, int level)
    {
        var indent = new string(' ', level * 2);

        details.AppendLine($"{indent}Type: {exception.GetType().FullName}");
        details.AppendLine($"{indent}Message: {exception.Message}");

        if (exception.StackTrace is not null)
        {
            details.AppendLine($"{indent}StackTrace:");
            details.AppendLine($"{indent}{exception.StackTrace}");
        }

        if (exception.TargetSite is not null)
        {
            details.AppendLine($"{indent}TargetSite: {exception.TargetSite}");
        }

        details.AppendLine($"{indent}HResult: {exception.HResult}");

        if (exception.Data.Count > 0)
        {
            details.AppendLine($"{indent}Data:");
            foreach (var key in exception.Data.Keys)
            {
                if (key is not null)
                {
                    details.AppendLine($"{indent}  {key}: {exception.Data[key]}");
                }
            }
        }

        if (exception.InnerException is not null)
        {
            details.AppendLine($"{indent}--- InnerException ---");
            SerializeExceptionRecursive(exception.InnerException, details, level + 1);
        }
    }

    /// <summary>
    /// Escapes brace characters in strings to prevent Serilog message template parsing issues.
    /// Serilog uses {{ and }} as escape sequences for literal braces.
    /// </summary>
    private static string EscapeBraces(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        return value
            .Replace("{", "{{")
            .Replace("}", "}}");
    }
}

/// <summary>
/// Constants used internally by the Serilog logging implementation.
/// </summary>
internal static class SerilogLogServiceConstants
{
    /// <summary>
    /// Property name for the source context (logger name) in Serilog.
    /// </summary>
    public const string SourceContextPropertyName = "SourceContext";

    /// <summary>
    /// Property name for the service name in Serilog.
    /// </summary>
    public const string ServiceNamePropertyName = "ServiceName";
}
