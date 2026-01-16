using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Orchestify.Api.Middlewares;
using Orchestify.Api.Services;
using Orchestify.Infrastructure.Logging;
using Orchestify.Shared.Correlation;
using Orchestify.Shared.Constants;
using Orchestify.Shared.Logging;
using Serilog;

namespace Orchestify.Api;

/// <summary>
/// Extension methods for registering custom middleware in the application pipeline.
/// Provides fluent, strongly-typed registration methods.
/// </summary>
public static class MiddlewareExtensions
{
    /// <summary>
    /// Adds the correlation ID middleware to the application pipeline.
    /// This middleware should be registered first to ensure correlation tracking
    /// is available for all subsequent middleware and services.
    /// </summary>
    /// <param name="builder">The application builder.</param>
    /// <returns>The application builder for method chaining.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="builder"/> is null.
    /// </exception>
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.UseMiddleware<CorrelationIdMiddleware>();
    }

    /// <summary>
    /// Adds the global exception handling middleware to the application pipeline.
    /// This middleware catches all unhandled exceptions and returns standardized error responses.
    /// Must be registered after correlation ID middleware.
    /// </summary>
    /// <param name="builder">The application builder.</param>
    /// <returns>The application builder for method chaining.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="builder"/> is null.
    /// </exception>
    public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.UseMiddleware<GlobalExceptionMiddleware>();
    }
}

/// <summary>
/// Extension methods for registering services required by middleware.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds correlation ID services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for method chaining.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="services"/> is null.
    /// </exception>
    public static IServiceCollection AddCorrelationId(this IServiceCollection services)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        // Register IHttpContextAccessor for accessing HttpContext in services
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        // Register the correlation ID provider
        services.TryAddSingleton<ICorrelationIdProvider, Orchestify.Api.Services.CorrelationIdProvider>();

        return services;
    }

    /// <summary>
    /// Adds the logging service to the dependency injection container.
    /// Uses Serilog-based implementation with correlation tracking.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for method chaining.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="services"/> is null.
    /// </exception>
    public static IServiceCollection AddLogService(this IServiceCollection services)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        // Register correlation provider first for injection into log service
        var correlationProvider = services.BuildServiceProvider().GetService<ICorrelationIdProvider>();
        services.TryAddSingleton<ILogService>(sp => new SerilogLogService(LogConstants.ApiServiceName, sp.GetService<ICorrelationIdProvider>()));

        return services;
    }

    /// <summary>
    /// Adds all required services for middleware (correlation ID and logging).
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for method chaining.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="services"/> is null.
    /// </exception>
    public static IServiceCollection AddMiddlewareServices(this IServiceCollection services)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        return services
            .AddCorrelationId()
            .AddLogService();
    }

    /// <summary>
    /// Adds and configures Serilog logging with the specified settings.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for method chaining.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="services"/> or <paramref name="configuration"/> is null.
    /// </exception>
    public static IServiceCollection AddSeriLogging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configuration is null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        // Bind configuration section to options entity
        services.Configure<Orchestify.Infrastructure.Options.LoggingOptionsEntity>(
            configuration.GetSection("Logging"));

        // Validate and configure Serilog immediately with options
        var loggingOptions = new Orchestify.Infrastructure.Options.LoggingOptionsEntity();
        configuration.GetSection("Logging").Bind(loggingOptions);

        if (loggingOptions.EnableElasticsearchSink && string.IsNullOrWhiteSpace(loggingOptions.ElasticsearchUrl))
        {
            throw new InvalidOperationException(
                "Elasticsearch URL is required when Elasticsearch sink is enabled. " +
                "Please configure the 'Logging:ElasticsearchUrl' value in appsettings.json.");
        }

        // Configure Serilog with options
        var serilogOptions = new SerilogConfigurationOptions
        {
            ServiceName = loggingOptions.ServiceName,
            Environment = loggingOptions.Environment,
            MinimumLogLevel = loggingOptions.MinimumLogLevel,
            EnableConsoleSink = loggingOptions.EnableConsoleSink,
            EnableElasticsearchSink = loggingOptions.EnableElasticsearchSink,
            ElasticsearchUrl = loggingOptions.ElasticsearchUrl,
            IndexFormat = loggingOptions.IndexFormat,
            IncludeAssemblyVersion = loggingOptions.IncludeAssemblyVersion,
            MachineName = loggingOptions.MachineName,
            ProcessId = loggingOptions.ProcessId
        };

        Log.Logger = SerilogConfiguration.Configure(serilogOptions).CreateLogger();

        // Register options for DI
        services.AddSingleton<Orchestify.Infrastructure.Options.LoggingOptionsEntity>(loggingOptions);

        return services;
    }
}
