namespace Orchestify.Shared.Errors;

/// <summary>
/// Central repository of error codes and messages for the Orchestify platform.
/// Organized by domain to maintain clear separation of concerns.
/// All partial classes combine to form a single ServiceError type.
/// </summary>
public static partial class ServiceError
{
    /// <summary>
    /// Prefix for all error codes to ensure consistency and prevent collisions.
    /// Format: ORC_{DOMAIN}_{SPECIFIC_ERROR}
    /// </summary>
    private const string ErrorPrefix = "ORC";

    /// <summary>
    /// Formats an error code with the standard prefix.
    /// </summary>
    private static string FormatErrorCode(string domain, string specificError) =>
        $"{ErrorPrefix}_{domain}_{specificError}";
}
