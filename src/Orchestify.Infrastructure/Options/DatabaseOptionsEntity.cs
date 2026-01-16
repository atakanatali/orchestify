namespace Orchestify.Infrastructure.Options;

/// <summary>
/// Configuration options for database connectivity and behavior.
/// </summary>
public sealed class DatabaseOptionsEntity
{
    /// <summary>
    /// Gets or sets the database connection string.
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the command timeout in seconds.
    /// </summary>
    public int CommandTimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Gets or sets the maximum pool size for database connections.
    /// </summary>
    public int MaxPoolSize { get; set; } = 100;

    /// <summary>
    /// Gets or sets the minimum pool size for database connections.
    /// </summary>
    public int MinPoolSize { get; set; } = 5;

    /// <summary>
    /// Gets or sets the connection lifetime in seconds.
    /// </summary>
    public int ConnectionLifetimeSeconds { get; set; } = 0;

    /// <summary>
    /// Gets or sets whether to enable sensitive data logging.
    /// WARNING: Only enable in development environments.
    /// </summary>
    public bool EnableSensitiveDataLogging { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to enable detailed error information.
    /// WARNING: Only enable in development environments.
    /// </summary>
    public bool EnableDetailedErrors { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to enable automatic migration on startup.
    /// WARNING: Use with caution in production.
    /// </summary>
    public bool AutoApplyMigrations { get; set; } = false;

    /// <summary>
    /// Gets or sets the maximum retry count for transient failures.
    /// </summary>
    public int MaxRetryCount { get; set; } = 3;

    /// <summary>
    /// Gets or sets the maximum delay between retries in seconds.
    /// </summary>
    public int MaxRetryDelaySeconds { get; set; } = 30;
}
