using System.Diagnostics;

namespace Orchestify.Shared.Correlation;

/// <summary>
/// Generates correlation IDs using a high-performance random GUID generator.
/// Optimized for high-throughput scenarios with millions of concurrent requests.
/// </summary>
public static class CorrelationIdGenerator
{
    /// <summary>
    /// Generates a new unique correlation ID.
    /// Uses <see cref="Guid.NewGuid"/> which internally uses a cryptographically secure
    /// random number generator for uniqueness guarantees.
    /// </summary>
    /// <returns>A new GUID formatted as a string (lowercase, hyphenated).</returns>
    public static string Generate() => Guid.NewGuid().ToString("D");

    /// <summary>
    /// Generates a new correlation ID without hyphens for compact representation.
    /// Useful for URL-safe scenarios where 32-character hex format is preferred.
    /// </summary>
    /// <returns>A new GUID formatted as a 32-character hex string.</returns>
    public static string GenerateCompact() => Guid.NewGuid().ToString("N");

    /// <summary>
    /// Generates a correlation ID that includes a timestamp prefix for chronological sorting.
    /// Format: {timestamp}_{guid}
    /// Example: 20250116_143052_a1b2c3d4-e5f6-7890-abcd-ef1234567890
    /// </summary>
    /// <returns>A timestamp-prefixed correlation ID.</returns>
    public static string GenerateWithTimestamp()
    {
        string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        return $"{timestamp}_{Guid.NewGuid():D}";
    }

    /// <summary>
    /// Validates whether a string is a valid correlation ID format.
    /// Accepts both standard GUID format (with hyphens) and compact format (without hyphens).
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <returns>True if the value is a valid GUID format; otherwise, false.</returns>
    public static bool IsValid(string? value)
    {
        return Guid.TryParse(value, out _);
    }

    /// <summary>
    /// Normalizes a correlation ID to the standard format (lowercase, hyphenated).
    /// Returns null if the input is not a valid GUID.
    /// </summary>
    /// <param name="value">The correlation ID to normalize.</param>
    /// <returns>The normalized correlation ID, or null if invalid.</returns>
    public static string? Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (Guid.TryParse(value, out Guid guid))
        {
            return guid.ToString("D");
        }

        return null;
    }
}
