using Microsoft.Extensions.DependencyInjection;
using Orchestify.Shared.Constants;
using Orchestify.Shared.Logging;
using Orchestify.Shared.Correlation;

namespace Orchestify.Infrastructure.Logging;

/// <summary>
/// Extension methods for registering Serilog-based logging services.
/// </summary>
public static class LoggingServiceExtensions
{
    /// <summary>
    /// Adds the Serilog-based <see cref="ILogService"/> to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceName">The name of the service (e.g., "orchestify-api" or "orchestify-worker").</param>
    /// <returns>The service collection for method chaining.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="services"/> or <paramref name="serviceName"/> is null.
    /// </exception>
    public static IServiceCollection AddSerilogLogService(
        this IServiceCollection services,
        string serviceName)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (string.IsNullOrWhiteSpace(serviceName))
        {
            throw new ArgumentException("Service name cannot be null or empty.", nameof(serviceName));
        }

        services.AddSingleton<ILogService>(sp =>
        {
            var correlationProvider = sp.GetService<ICorrelationIdProvider>();
            return new SerilogLogService(serviceName, correlationProvider);
        });

        return services;
    }
}
