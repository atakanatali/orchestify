using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Orchestify.Infrastructure.Messaging;

/// <summary>
/// Extension methods for configuring MassTransit with RabbitMQ.
/// </summary>
public static class MassTransitExtensions
{
    /// <summary>
    /// Adds MassTransit with RabbitMQ configuration.
    /// Supports configuration of concurrency settings via appsettings:
    /// - RabbitMQ:PrefetchCount (default: 16)
    /// - RabbitMQ:ConcurrentMessageLimit (default: 8)
    /// </summary>
    public static IServiceCollection AddMassTransitWithRabbitMq(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IBusRegistrationConfigurator>? configureConsumers = null)
    {
        var rabbitMqConfig = configuration.GetSection("RabbitMQ");
        var host = rabbitMqConfig["Host"] ?? "localhost";
        var username = rabbitMqConfig["Username"] ?? "orchestify";
        var password = rabbitMqConfig["Password"] ?? "orchestify_dev";
        var virtualHost = rabbitMqConfig["VirtualHost"] ?? "/";

        // Concurrency settings for 120 parallel tasks
        // PrefetchCount: Number of messages RabbitMQ sends to consumer before acknowledgment
        // ConcurrentMessageLimit: Max number of messages processed concurrently per consumer
        var prefetchCount = rabbitMqConfig.GetValue<int?>("PrefetchCount") ?? 16;
        var concurrentMessageLimit = rabbitMqConfig.GetValue<int?>("ConcurrentMessageLimit") ?? 8;

        services.AddMassTransit(x =>
        {
            // Allow consumers to be registered by the caller
            configureConsumers?.Invoke(x);

            // Set endpoint name formatter
            x.SetKebabCaseEndpointNameFormatter();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(host, virtualHost, h =>
                {
                    h.Username(username);
                    h.Password(password);
                });

                // Configure global prefetch count
                cfg.PrefetchCount = prefetchCount;

                // Configure global concurrent message limit
                cfg.ConcurrentMessageLimit = concurrentMessageLimit;

                // Configure endpoints for all registered consumers
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
