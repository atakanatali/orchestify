using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Orchestify.Infrastructure.Options;
using StackExchange.Redis;
using Orchestify.Infrastructure.Persistence;
using Orchestify.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Orchestify.Infrastructure;

/// <summary>
/// Extension methods for registering infrastructure services with dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds and configures database services with the specified connection settings.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for method chaining.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="services"/> or <paramref name="configuration"/> is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the connection string is missing or invalid.
    /// </exception>
    public static IServiceCollection AddDatabase(
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
        services.Configure<DatabaseOptionsEntity>(options =>
            configuration.GetSection("Database").Bind(options));

        // Validate configuration on startup
        services.AddSingleton<DatabaseOptionsEntity>(resolver =>
        {
            var options = resolver.GetRequiredService<IOptions<DatabaseOptionsEntity>>().Value;

            if (string.IsNullOrWhiteSpace(options.ConnectionString))
            {
                throw new InvalidOperationException(
                    "Database connection string is missing. " +
                    "Please configure the 'Database:ConnectionString' value in appsettings.json.");
            }

            return options;
        });

        var dbConnectionString = configuration.GetSection("Database")["ConnectionString"];
        if (string.IsNullOrWhiteSpace(dbConnectionString))
        {
             throw new InvalidOperationException("Database connection string is missing.");
        }

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(dbConnectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
            });
        });

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        // AI Infrastructure Services
        services.AddSingleton<Services.MetricsService>();
        services.AddScoped<IAgentOrchestrator, Services.AgentOrchestrator>();

        return services;
    }

    /// <summary>
    /// Adds and configures distributed caching services with the specified connection settings.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for method chaining.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="services"/> or <paramref name="configuration"/> is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the connection string is missing or invalid.
    /// </exception>
    public static IServiceCollection AddCaching(
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
        services.Configure<CachingOptionsEntity>(options =>
            configuration.GetSection("Caching").Bind(options));

        // Validate configuration on startup
        services.AddSingleton<CachingOptionsEntity>(resolver =>
        {
            var options = resolver.GetRequiredService<IOptions<CachingOptionsEntity>>().Value;

            if (string.IsNullOrWhiteSpace(options.ConnectionString))
            {
                throw new InvalidOperationException(
                    "Caching connection string is missing. " +
                    "Please configure the 'Caching:ConnectionString' value in appsettings.json.");
            }

            return options;
        });

        // Register IConnectionMultiplexer as singleton for Redis
        services.AddSingleton<IConnectionMultiplexer>(resolver =>
        {
            var options = resolver.GetRequiredService<IOptions<CachingOptionsEntity>>().Value;

            var configOptions = ConfigurationOptions.Parse(options.ConnectionString!);
            configOptions.ClientName = options.InstanceName;
            configOptions.Ssl = options.UseSsl;
            configOptions.Password = options.Password;
            configOptions.ConnectTimeout = options.ConnectTimeoutMilliseconds;
            configOptions.SyncTimeout = options.SyncTimeoutMilliseconds;
            configOptions.AsyncTimeout = options.ResponseTimeoutMilliseconds;
            configOptions.AllowAdmin = options.AllowAdmin;

            return ConnectionMultiplexer.Connect(configOptions);
        });

        // Register distributed cache
        services.AddStackExchangeRedisCache(options =>
        {
            options.InstanceName = "Orchestify";
        });

        return services;
    }
}
