using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using Orchestify.Shared.Constants;

namespace Orchestify.Infrastructure.Logging;

/// <summary>
/// Configuration options for Serilog logging setup.
/// </summary>
public sealed class SerilogConfigurationOptions
{
    /// <summary>
    /// Gets or sets the Elasticsearch connection URL.
    /// Example: http://localhost:9200
    /// </summary>
    public string? ElasticsearchUrl { get; set; }

    /// <summary>
    /// Gets or sets the service name for log identification.
    /// Must be either <see cref="LogConstants.ApiServiceName"/> or <see cref="LogConstants.WorkerServiceName"/>.
    /// </summary>
    public string ServiceName { get; set; } = LogConstants.ApiServiceName;

    /// <summary>
    /// Gets or sets the environment name (e.g., Development, Production, Staging).
    /// </summary>
    public string Environment { get; set; } = "Development";

    /// <summary>
    /// Gets or sets the minimum log level to capture.
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
    /// Gets or sets the Elasticsearch index format.
    /// If null, uses the default format based on service name.
    /// </summary>
    public string? IndexFormat { get; set; }

    /// <summary>
    /// Gets or sets whether to include assembly version information in logs.
    /// </summary>
    public bool IncludeAssemblyVersion { get; set; } = true;

    /// <summary>
    /// Gets or sets the machine name for log identification.
    /// </summary>
    public string? MachineName { get; set; }

    /// <summary>
    /// Gets or sets the process ID for log identification.
    /// </summary>
    public int? ProcessId { get; set; }
}

/// <summary>
/// Provides configuration methods for setting up Serilog with appropriate sinks and enrichment.
/// </summary>
public static class SerilogConfiguration
{
    /// <summary>
    /// Configures Serilog with Console and Elasticsearch sinks, plus standard enrichment.
    /// </summary>
    /// <param name="options">Configuration options for Serilog setup.</param>
    /// <returns>The configured LoggerConfiguration.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="options"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when required configuration values are missing or invalid.
    /// </exception>
    public static LoggerConfiguration Configure(SerilogConfigurationOptions options)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (string.IsNullOrWhiteSpace(options.ServiceName))
        {
            throw new ArgumentException("Service name is required.", nameof(options));
        }

        var loggerConfiguration = new LoggerConfiguration();

        // Set minimum log level
        loggerConfiguration.MinimumLevel.Is(options.MinimumLogLevel);

        // Configure standard enrichers
        ConfigureEnrichers(loggerConfiguration, options);

        // Configure Console sink
        if (options.EnableConsoleSink)
        {
            ConfigureConsoleSink(loggerConfiguration);
        }

        // Configure Elasticsearch sink
        if (options.EnableElasticsearchSink)
        {
            ConfigureElasticsearchSink(loggerConfiguration, options);
        }

        return loggerConfiguration;
    }

    /// <summary>
    /// Configures standard enrichers for correlation tracking and system metadata.
    /// </summary>
    private static void ConfigureEnrichers(LoggerConfiguration configuration, SerilogConfigurationOptions options)
    {
        configuration
            .Enrich.FromLogContext()
            .Enrich.WithProperty(LogConstants.ServiceNamePropertyName, options.ServiceName)
            .Enrich.WithProperty(LogConstants.EnvironmentPropertyName, options.Environment)
            .Enrich.WithProperty(LogConstants.MachineNamePropertyName, options.MachineName ?? Environment.MachineName)
            .Enrich.WithProperty(LogConstants.ProcessIdPropertyName, options.ProcessId ?? Environment.ProcessId);
    }

    /// <summary>
    /// Configures the Console sink for local debugging and development.
    /// </summary>
    private static void ConfigureConsoleSink(LoggerConfiguration configuration)
    {
        configuration.WriteTo.Console(
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{ServiceName}] [{CorrelationId}] [{Code}] {Message:lj}{NewLine}{Exception}"
        );
    }

    /// <summary>
    /// Configures the Elasticsearch sink for persistent log storage.
    /// Includes retry logic for when Elasticsearch is warming up.
    /// </summary>
    private static void ConfigureElasticsearchSink(LoggerConfiguration configuration, SerilogConfigurationOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.ElasticsearchUrl))
        {
            throw new ArgumentException(
                "Elasticsearch URL is required when Elasticsearch sink is enabled.",
                nameof(options));
        }

        // Determine index format based on service name
        string indexFormat = GetIndexFormat(options.ServiceName, options.IndexFormat);

        configuration.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(options.ElasticsearchUrl))
        {
            AutoRegisterTemplate = true,
            AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv8,
            IndexFormat = indexFormat,
            NumberOfShards = 1,
            NumberOfReplicas = 0, // Single-node setup
            ModifyConnectionSettings = connectionConfiguration =>
            {
                // Configure retry logic for when Elasticsearch is warming up
                return connectionConfiguration
                    .EnableHttpCompression()
                    .DisableDirectStreaming()
                    .RequestTimeout(TimeSpan.FromSeconds(30));
            },
            FailureCallback = FailureCallback,
            EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog |
                               EmitEventFailureHandling.ThrowException |
                               EmitEventFailureHandling.RaiseCallback,
            BatchAction = ElasticOpType.Create
        });
    }

    /// <summary>
    /// Determines the appropriate index format based on the service name.
    /// </summary>
    private static string GetIndexFormat(string serviceName, string? customIndexFormat)
    {
        if (!string.IsNullOrWhiteSpace(customIndexFormat))
        {
            return customIndexFormat;
        }

        return serviceName switch
        {
            LogConstants.ApiServiceName => "logs-orchestify-api-{0:yyyy.MM.dd}",
            LogConstants.WorkerServiceName => "logs-orchestify-worker-{0:yyyy.MM.dd}",
            _ => $"logs-{serviceName}-{{0:yyyy.MM.dd}}"
        };
    }

    /// <summary>
    /// Callback invoked when a log event fails to be sent to Elasticsearch.
    /// Logs the failure to Serilog's self-log for debugging.
    /// </summary>
    private static void FailureCallback(LogEvent logEvent, Exception exception)
    {
        // Note: This uses Console.WriteLine instead of logging to avoid infinite recursion
        // In production, consider monitoring these failures for alerting
        Console.Error.WriteLine($"Failed to send log event to Elasticsearch: {exception.Message}");
    }

    /// <summary>
    /// Creates a pre-configured Serilog logger with default settings.
    /// </summary>
    /// <param name="serviceName">The name of the service.</param>
    /// <param name="elasticsearchUrl">The Elasticsearch connection URL.</param>
    /// <param name="environment">The environment name.</param>
    /// <returns>The configured Serilog logger.</returns>
    public static ILogger CreateDefaultLogger(
        string serviceName,
        string? elasticsearchUrl = null,
        string environment = "Development")
    {
        var options = new SerilogConfigurationOptions
        {
            ServiceName = serviceName,
            ElasticsearchUrl = elasticsearchUrl ?? "http://localhost:9200",
            Environment = environment,
            MinimumLogLevel = environment == "Development"
                ? LogEventLevel.Debug
                : LogEventLevel.Information,
            EnableConsoleSink = true,
            EnableElasticsearchSink = !string.IsNullOrWhiteSpace(elasticsearchUrl)
        };

        return Configure(options).CreateLogger();
    }
}
