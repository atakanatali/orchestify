namespace Orchestify.Shared.Results;

/// <summary>
/// Standardized result type for application service operations.
/// Provides consistent error handling and response formatting across the platform.
/// </summary>
public sealed class ServiceResult
{
    /// <summary>
    /// Gets a value indicating whether the operation completed successfully.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the error code associated with a failed operation.
    /// Null when the operation is successful.
    /// </summary>
    public string? ErrorCode { get; }

    /// <summary>
    /// Gets the error message associated with a failed operation.
    /// Null when the operation is successful.
    /// </summary>
    public string? ErrorMessage { get; }

    /// <summary>
    /// Gets the correlation ID for tracking the request across service boundaries.
    /// Useful for distributed tracing and debugging.
    /// </summary>
    public string? CorrelationId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceResult"/> class.
    /// Private constructor to enforce factory method usage.
    /// </summary>
    private ServiceResult(bool isSuccess, string? errorCode, string? errorMessage, string? correlationId)
    {
        IsSuccess = isSuccess;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
        CorrelationId = correlationId;
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <returns>A successful <see cref="ServiceResult"/> instance.</returns>
    public static ServiceResult Success()
    {
        return new ServiceResult(true, null, null, null);
    }

    /// <summary>
    /// Creates a successful result with a correlation ID.
    /// </summary>
    /// <param name="correlationId">The correlation ID for request tracking.</param>
    /// <returns>A successful <see cref="ServiceResult"/> instance.</returns>
    public static ServiceResult Success(string correlationId)
    {
        return new ServiceResult(true, null, null, correlationId);
    }

    /// <summary>
    /// Creates a failed result with the specified error code and message.
    /// </summary>
    /// <param name="errorCode">The error code from <see cref="ServiceError"/>.</param>
    /// <param name="errorMessage">The error message describing the failure.</param>
    /// <returns>A failed <see cref="ServiceResult"/> instance.</returns>
    public static ServiceResult Failure(string errorCode, string errorMessage)
    {
        return new ServiceResult(false, errorCode, errorMessage, null);
    }

    /// <summary>
    /// Creates a failed result with the specified error code, message, and correlation ID.
    /// </summary>
    /// <param name="errorCode">The error code from <see cref="ServiceError"/>.</param>
    /// <param name="errorMessage">The error message describing the failure.</param>
    /// <param name="correlationId">The correlation ID for request tracking.</param>
    /// <returns>A failed <see cref="ServiceResult"/> instance.</returns>
    public static ServiceResult Failure(string errorCode, string errorMessage, string correlationId)
    {
        return new ServiceResult(false, errorCode, errorMessage, correlationId);
    }
}
