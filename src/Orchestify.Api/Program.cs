using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orchestify.Api;
using Orchestify.Api.Middlewares;
using Orchestify.Shared.Constants;
using Serilog;

// Import Infrastructure extension methods
using Orchestify.Infrastructure;
using Orchestify.Infrastructure.Services;
using Orchestify.Infrastructure.Messaging;
using Orchestify.Application;
using Orchestify.Application.Common.Interfaces;

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
        try
        {
            // Bootstrap Serilog before anything else
            var bootstrapConfig = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var esUrl = bootstrapConfig["Logging:Elasticsearch:Url"] ?? "http://localhost:9200";
            var enableEs = bootstrapConfig.GetValue<bool>("Logging:EnableElasticsearchSink");

            var loggerConfig = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("ServiceName", LogConstants.ApiServiceName)
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{ServiceName}] {Message:lj}{NewLine}{Exception}");

            if (enableEs)
            {
                loggerConfig.WriteTo.Elasticsearch(new Serilog.Sinks.Elasticsearch.ElasticsearchSinkOptions(new Uri(esUrl))
                {
                    AutoRegisterTemplate = true,
                    IndexFormat = "orchestify-logs-{0:yyyy.MM.dd}",
                    NumberOfShards = 1,
                    NumberOfReplicas = 0
                });
            }

            Log.Logger = loggerConfig.CreateLogger();
            Log.Information("Serilog initialized. Elasticsearch sink enabled: {EsEnabled}", enableEs);

            var builder = WebApplication.CreateBuilder(args);

            // Add infrastructure services
            var services = builder.Services;
            var configuration = builder.Configuration;

            // Configure infrastructure using extension methods
            services.AddScoped<IGitService, GitService>();
            services.AddDatabase(builder.Configuration);
            services.AddCaching(configuration);
            services.AddApplication();
            services.AddScoped<Application.Common.Interfaces.ITaskExecutionNotifier, Api.Hubs.SignalRTaskExecutionNotifier>();
            services.AddSignalR();

            // Add MassTransit with RabbitMQ for task queue
            services.AddMassTransitWithRabbitMq(configuration);

            // Add Redis to SignalR bridge for real-time events from workers
            services.AddHostedService<Api.Services.RedisToSignalRBridge>();

            // Use Serilog for logging
            builder.Host.UseSerilog();

            // Register middleware services (correlation ID, logging)
            services.AddMiddlewareServices();

            // Add health checks
            services.AddHealthChecks();

            // Add controllers
            services.AddControllers();

            // Enable CORS
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins("http://localhost:3001", "http://localhost:3000")
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials();
                });
            });

            var app = builder.Build();

            // IMPORTANT: Middleware registration order matters!
            // Correlation ID middleware must be first to ensure all requests have correlation tracking
            app.UseCorrelationId();

            // Global exception handling must come after correlation ID
            app.UseGlobalExceptionHandling();

            app.UseCors();

            // Health check endpoint
            app.MapHealthChecks("/health");

            // Authorization and other middleware would go here
            // app.UseAuthorization();

            app.MapHub<Api.Hubs.TaskExecutionHub>("/hubs/task-execution");
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
}

/// <summary>
/// Log message templates used by the application.
/// </summary>
internal static class LogServiceMessages
{
    /// <summary>
    /// Message template for application started.
    /// </summary>
    public const string ApplicationStarted = "{ServiceName} application started successfully";

    /// <summary>
    /// Message template for unexpected application termination.
    /// </summary>
    public const string ApplicationTerminatedUnexpectedly = "{ServiceName} application terminated unexpectedly";
}
