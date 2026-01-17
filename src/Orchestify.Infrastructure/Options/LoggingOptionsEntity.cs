using Serilog.Events;

namespace Orchestify.Infrastructure.Options;

/// <summary>
/// Configuration options for structured logging with Serilog.
/// </summary>
public sealed class LoggingOptionsEntity
{
    /// <summary>
    /// Gets or sets the service name for log identification.
    /// Must be either "orchestify-api" or "orchestify-worker".
    /// </summary>
    public string ServiceName { get; set; } = "orchestify-api";

    /// <summary>
    /// Gets or sets the environment name (e.g., Development, Production, Staging).
    /// </summary>
    public string Environment { get; set; } = "Development";

    /// <summary>
    /// Gets or sets the minimum log level to capture.
    /// Valid values: Verbose, Debug, Information, Warning, Error, Fatal.
    /// </summary>
    public LogEventLevel MinimumLogLevel { get; set; } = LogEventLevel.Information;

    /// <summary>
    /// Gets or sets whether to output logs to the console.
    /// </summary>
    public bool EnableConsoleSink { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to output logs to Elasticsearch.
    /// </summary>
    public bool EnableElasticsearchSink { get; set; } = false;

    /// <summary>
    /// Gets or sets the Elasticsearch connection URL.
    /// Example: http://localhost:9200
    /// </summary>
    public string? ElasticsearchUrl { get; set; }

    /// <summary>
    /// Gets or sets the Elasticsearch index format.
    /// If null, uses the default format based on service name.
    /// Example: "logs-orchestify-api-{0:yyyy.MM.dd}"
    /// </summary>
    public string? IndexFormat { get; set; }

    /// <summary>
    /// Gets or sets the number of shards for Elasticsearch indices.
    /// </summary>
    public int NumberOfShards { get; set; } = 1;

    /// <summary>
    /// Gets or sets the number of replicas for Elasticsearch indices.
    /// </summary>
    public int NumberOfReplicas { get; set; } = 0;

    /// <summary>
    /// Gets or sets whether to include assembly version information in logs.
    /// </summary>
    public bool IncludeAssemblyVersion { get; set; } = true;

    /// <summary>
    /// Gets or sets the machine name for log identification.
    /// If null, uses Environment.MachineName.
    /// </summary>
    public string? MachineName { get; set; }

    /// <summary>
    /// Gets or sets the process ID for log identification.
    /// If null, uses Environment.ProcessId.
    /// </summary>
    public int? ProcessId { get; set; }

    /// <summary>
    /// Gets or sets the request timeout for Elasticsearch in seconds.
    /// </summary>
    public int ElasticsearchRequestTimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Gets or sets whether to enable HTTP compression for Elasticsearch.
    /// </summary>
    public bool EnableHttpCompression { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to disable direct streaming for Elasticsearch.
    /// </summary>
    public bool DisableDirectStreaming { get; set; } = true;
}
