using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orchestify.Api;
using Orchestify.Infrastructure.Logging;
using Orchestify.Shared.Constants;
using Serilog;

namespace Orchestify.Api;

/// <summary>
/// Entry point for the Orchestify Web API application.
/// Configures the application pipeline, middleware, and dependency injection.
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
            Log.Information(LogServiceMessages.StartingApplication, LogConstants.ApiServiceName);

            var builder = WebApplication.CreateBuilder(args);

            // Use Serilog for logging
            builder.Host.UseSerilog();

            // Add services to the container
            var services = builder.Services;

            // Register middleware services (correlation ID, logging)
            services.AddMiddlewareServices();

            // Add health checks
            services.AddHealthChecks();

            // Add controllers
            services.AddControllers();

            var app = builder.Build();

            // IMPORTANT: Middleware registration order matters!
            // Correlation ID middleware must be first to ensure all requests have correlation tracking
            app.UseCorrelationId();

            // Global exception handling must come after correlation ID
            app.UseGlobalExceptionHandling();

            // Health check endpoint
            app.MapHealthChecks("/health");

            // Authorization and other middleware would go here
            // app.UseAuthorization();

            app.MapControllers();

            Log.Information(LogServiceMessages.ApplicationStarted, LogConstants.ApiServiceName);
            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, LogServiceMessages.ApplicationTerminatedUnexpectedly, LogConstants.ApiServiceName);
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
            LogConstants.ApiServiceName,
            elasticsearchUrl ?? "http://localhost:9200",
            environment);

        Log.Logger = logger;
    }
}

/// <summary>
/// Log message templates used by the application.
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
