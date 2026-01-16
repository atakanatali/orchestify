using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orchestify.Api;

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
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container
        var services = builder.Services;

        // Register middleware services (correlation ID, logging)
        services.AddMiddlewareServices();

        // Add controllers
        services.AddControllers();

        var app = builder.Build();

        // IMPORTANT: Middleware registration order matters!
        // Correlation ID middleware must be first to ensure all requests have correlation tracking
        app.UseCorrelationId();

        // Global exception handling must come after correlation ID
        app.UseGlobalExceptionHandling();

        // Authorization and other middleware would go here
        // app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
