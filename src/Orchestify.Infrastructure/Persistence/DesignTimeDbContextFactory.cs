using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Orchestify.Infrastructure.Persistence;

/// <summary>
/// Design-time factory for the <see cref="ApplicationDbContext"/>.
/// This allows Entity Framework Core tools to create the DbContext independently of the startup project.
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // Try to find the appsettings.json in the Api project
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../Orchestify.Api");
        
        // If that doesn't exist, try local project (Infrastructure)
        if (!Directory.Exists(basePath))
        {
            basePath = Directory.GetCurrentDirectory();
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetSection("Database")["ConnectionString"];

        // Fallback for local development if not found in configuration
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = "Host=localhost;Port=5432;Database=orchestify;Username=orchestify;Password=orchestify_dev";
        }

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql(connectionString, npgsqlOptions =>
        {
            npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
        });

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
