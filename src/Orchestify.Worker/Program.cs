using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orchestify.Infrastructure.Logging;
using Orchestify.Shared.Constants;
using Orchestify.Shared.Logging;
using Serilog;

namespace Orchestify.Worker;

/// <summary>
/// Entry point for the Orchestify Worker service.
/// </summary>
public static class Program
{
    /// <summary>
    /// Main entry point.
    /// </summary>
    public static void Main(string[] args)
    {
        // Configure Serilog before building the host
        ConfigureSerilog();

        try
        {
            Log.Information(LogServiceMessages.StartingApplication, LogConstants.WorkerServiceName);

            var host = Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureServices((context, services) =>
                {
                    // Register the logging service
                    services.AddSingleton<ILogService>(sp =>
                        new SerilogLogService(LogConstants.WorkerServiceName));

                    // Register the background worker
                    services.AddHostedService<Worker>();
                })
                .Build();

            Log.Information(LogServiceMessages.ApplicationStarted, LogConstants.WorkerServiceName);
            host.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, LogServiceMessages.ApplicationTerminatedUnexpectedly, LogConstants.WorkerServiceName);
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    /// <summary>
    /// Configures Serilog with Console and Elasticsearch sinks.
    /// </summary>
    private static void ConfigureSerilog()
    {
        var elasticsearchUrl = Environment.GetEnvironmentVariable("Serilog__ElasticsearchUrl");
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Environments.Development;

        var logger = SerilogConfiguration.CreateDefaultLogger(
            LogConstants.WorkerServiceName,
            elasticsearchUrl ?? "http://localhost:9200",
            environment);

        Log.Logger = logger;
    }
}

/// <summary>
/// Log message templates used by the worker service.
/// </summary>
internal static class LogServiceMessages
{
    /// <summary>
    /// Message template for application startup.
    /// </summary>
    public const string StartingApplication = "Starting {ServiceName} application";

    /// <summary>
    /// Message template for application started.
    /// </summary>
    public const string ApplicationStarted = "{ServiceName} application started successfully";

    /// <summary>
    /// Message template for unexpected application termination.
    /// </summary>
    public const string ApplicationTerminatedUnexpectedly = "{ServiceName} application terminated unexpectedly";
}
