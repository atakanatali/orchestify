using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orchestify.Application.Common.Interfaces;
using Orchestify.Infrastructure;
using Orchestify.Infrastructure.Logging;
using Orchestify.Infrastructure.Messaging;
using Orchestify.Shared.Constants;
using Orchestify.Shared.Logging;
using Orchestify.Worker.Services;
using Orchestify.Worker.StepExecutors;
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
                    // Register Infrastructure services (Database, etc.)
                    services.AddDatabase(context.Configuration);
                    services.AddCaching(context.Configuration);

                    // Register the logging service
                    services.AddSingleton<ILogService>(sp =>
                        new SerilogLogService(LogConstants.WorkerServiceName));

                    // Register step executors
                    services.AddScoped<IStepExecutor, RestoreStepExecutor>();
                    services.AddScoped<IStepExecutor, BuildStepExecutor>();
                    services.AddScoped<IStepExecutor, TestStepExecutor>();
                    services.AddScoped<IStepExecutor, ReviewStepExecutor>();

                    // Add MassTransit with RabbitMQ and register the consumer
                    services.AddMassTransitWithRabbitMq(context.Configuration, x =>
                    {
                        x.AddConsumer<Consumers.RunTaskConsumer>();
                    });

                    // Register the background workers
                    services.AddHostedService<Worker>();
                    // TODO: Legacy polling service - disabled in favor of MassTransit
                    // Requires IAttemptQueueService which is not yet implemented
                    // services.AddHostedService<AttemptProcessorService>();
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
    /// Uses environment variables: Logging__Elasticsearch__Url and Logging__EnableElasticsearchSink
    /// </summary>
    private static void ConfigureSerilog()
    {
        // Docker passes these as Logging__Elasticsearch__Url (colon-separated becomes double underscore)
        var elasticsearchUrl = Environment.GetEnvironmentVariable("Logging__Elasticsearch__Url") 
            ?? Environment.GetEnvironmentVariable("Serilog__ElasticsearchUrl")
            ?? "http://elasticsearch:9200";
        
        var enableEs = Environment.GetEnvironmentVariable("Logging__EnableElasticsearchSink")?.Equals("true", StringComparison.OrdinalIgnoreCase) ?? false;
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Environments.Development;

        var loggerConfig = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("ServiceName", LogConstants.WorkerServiceName)
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{ServiceName}] {Message:lj}{NewLine}{Exception}");

        if (enableEs)
        {
            loggerConfig.WriteTo.Elasticsearch(new Serilog.Sinks.Elasticsearch.ElasticsearchSinkOptions(new Uri(elasticsearchUrl))
            {
                AutoRegisterTemplate = true,
                IndexFormat = "orchestify-logs-{0:yyyy.MM.dd}",
                NumberOfShards = 1,
                NumberOfReplicas = 0
            });
        }

        Log.Logger = loggerConfig.CreateLogger();
        Log.Information("Worker Serilog initialized. Elasticsearch sink enabled: {EsEnabled}, URL: {EsUrl}", enableEs, elasticsearchUrl);
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
