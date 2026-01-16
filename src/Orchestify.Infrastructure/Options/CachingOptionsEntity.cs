namespace Orchestify.Infrastructure.Options;

/// <summary>
/// Configuration options for distributed caching behavior.
/// </summary>
public sealed class CachingOptionsEntity
{
    /// <summary>
    /// Gets or sets the cache connection string.
    /// Example format: "localhost:6379"
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the instance name prefix for all cache keys.
    /// Useful for multi-tenant deployments.
    /// </summary>
    public string InstanceName { get; set; } = "Orchestify";

    /// <summary>
    /// Gets or sets whether SSL/TLS should be enabled for the cache connection.
    /// </summary>
    public bool UseSsl { get; set; } = false;

    /// <summary>
    /// Gets or sets the password for authentication with the cache server.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Gets or sets whether to connect as a read-only replica.
    /// </summary>
    public bool IsReadOnly { get; set; } = false;

    /// <summary>
    /// Gets or sets the connect timeout in milliseconds.
    /// </summary>
    public int ConnectTimeoutMilliseconds { get; set; } = 5000;

    /// <summary>
    /// Gets or sets the sync timeout in milliseconds.
    /// </summary>
    public int SyncTimeoutMilliseconds { get; set; } = 5000;

    /// <summary>
    /// Gets or sets the response timeout in milliseconds.
    /// </summary>
    public int ResponseTimeoutMilliseconds { get; set; } = 5000;

    /// <summary>
    /// Gets or sets whether to allow admin operations.
    /// WARNING: Only enable in trusted environments.
    /// </summary>
    public bool AllowAdmin { get; set; } = false;

    /// <summary>
    /// Gets or sets the maximum number of concurrent connections.
    /// </summary>
    public int MaxConnectionCount { get; set; } = 50;

    /// <summary>
    /// Gets or sets the minimum number of concurrent connections.
    /// </summary>
    public int MinConnectionCount { get; set; } = 5;

    /// <summary>
    /// Gets or sets the default expiration time for cached items in seconds.
    /// </summary>
    public int DefaultExpirationSeconds { get; set; } = 3600;

    /// <summary>
    /// Gets or sets whether to use in-memory caching as a fallback.
    /// </summary>
    public bool EnableInMemoryFallback { get; set; } = false;
}
