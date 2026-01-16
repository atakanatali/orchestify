using Microsoft.Extensions.Logging;
using Orchestify.Shared.Logging;

namespace Orchestify.Api.Services;

/// <summary>
/// ASP.NET Core implementation of <see cref="ILogService"/> using <see cref="ILogger"/>.
/// Provides structured logging with correlation tracking and contextual enrichment.
/// </summary>
public sealed class LogService : ILogService
{
    private readonly ILogger<LogService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogService"/> class.
    /// </summary>
    /// <param name="logger">The underlying logger.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="logger"/> is null.
    /// </exception>
    public LogService(ILogger<LogService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public void Information(string message, IDictionary<string, object?>? context = null, string? correlationId = null)
    {
        using IDisposable? scope = CreateScope(context, correlationId);
        _logger.LogInformation(message);
    }

    /// <inheritdoc/>
    public void Warning(string message, IDictionary<string, object?>? context = null, string? correlationId = null)
    {
        using IDisposable? scope = CreateScope(context, correlationId);
        _logger.LogWarning(message);
    }

    /// <inheritdoc/>
    public void Error(
        Exception exception,
        string errorCode,
        string message,
        IDictionary<string, object?>? context = null,
        string? correlationId = null)
    {
        using IDisposable? scope = CreateScope(context, correlationId);

        // Log with full exception details including stack trace
        _logger.LogError(exception,
            "Error: {ErrorCode} - {Message}. Exception: {ExceptionType} - {ExceptionMessage}",
            errorCode,
            message,
            exception.GetType().FullName,
            exception.Message);
    }

    /// <inheritdoc/>
    public void Debug(string message, IDictionary<string, object?>? context = null, string? correlationId = null)
    {
        using IDisposable? scope = CreateScope(context, correlationId);
        _logger.LogDebug(message);
    }

    /// <inheritdoc/>
    public IDisposable BeginScope(IDictionary<string, object?> context)
    {
        return _logger.BeginScope(context) ?? EmptyScope.Instance;
    }

    /// <inheritdoc/>
    public IDisposable BeginScope(string scopeName, IDictionary<string, object?>? context = null)
    {
        Dictionary<string, object?> scopeContext = new(StringComparer.Ordinal)
        {
            ["ScopeName"] = scopeName
        };

        if (context is not null)
        {
            foreach (var kvp in context)
            {
                scopeContext[kvp.Key] = kvp.Value;
            }
        }

        return _logger.BeginScope(scopeContext) ?? EmptyScope.Instance;
    }

    /// <summary>
    /// Creates a logging scope with the provided context and correlation ID.
    /// </summary>
    /// <param name="context">Optional contextual data.</param>
    /// <param name="correlationId">Optional correlation ID.</param>
    /// <returns>A disposable scope, or null if no scope data was provided.</returns>
    private IDisposable? CreateScope(IDictionary<string, object?>? context, string? correlationId)
    {
        Dictionary<string, object?> scopeData = new(StringComparer.Ordinal);

        if (!string.IsNullOrWhiteSpace(correlationId))
        {
            scopeData["CorrelationId"] = correlationId;
        }

        if (context is not null)
        {
            foreach (var kvp in context)
            {
                scopeData[kvp.Key] = kvp.Value;
            }
        }

        return scopeData.Count > 0 ? _logger.BeginScope(scopeData) : null;
    }

    /// <summary>
    /// Empty scope implementation for when BeginScope returns null.
    /// </summary>
    private sealed class EmptyScope : IDisposable
    {
        public static readonly EmptyScope Instance = new();

        private EmptyScope() { }

        public void Dispose() { }
    }
}
