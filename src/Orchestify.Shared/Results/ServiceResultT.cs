namespace Orchestify.Shared.Results;

/// <summary>
/// Standardized result type for application service operations that return a value.
/// Provides consistent error handling and response formatting across the platform.
/// </summary>
/// <typeparam name="T">The type of the value returned on success.</typeparam>
public sealed class ServiceResult<T>
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
    /// Gets the value returned by a successful operation.
    /// Null when the operation fails.
    /// </summary>
    public T? Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceResult{T}"/> class.
    /// Private constructor to enforce factory method usage.
    /// </summary>
    private ServiceResult(bool isSuccess, T? value, string? errorCode, string? errorMessage, string? correlationId)
    {
        IsSuccess = isSuccess;
        Value = value;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
        CorrelationId = correlationId;
    }

    /// <summary>
    /// Creates a successful result with the specified value.
    /// </summary>
    /// <param name="value">The value returned by the operation.</param>
    /// <returns>A successful <see cref="ServiceResult{T}"/> instance.</returns>
    public static ServiceResult<T> Success(T value)
    {
        return new ServiceResult<T>(true, value, null, null, null);
    }

    /// <summary>
    /// Creates a successful result with the specified value and correlation ID.
    /// </summary>
    /// <param name="value">The value returned by the operation.</param>
    /// <param name="correlationId">The correlation ID for request tracking.</param>
    /// <returns>A successful <see cref="ServiceResult{T}"/> instance.</returns>
    public static ServiceResult<T> Success(T value, string correlationId)
    {
        return new ServiceResult<T>(true, value, null, null, correlationId);
    }

    /// <summary>
    /// Creates a failed result with the specified error code and message.
    /// </summary>
    /// <param name="errorCode">The error code from <see cref="ServiceError"/>.</param>
    /// <param name="errorMessage">The error message describing the failure.</param>
    /// <returns>A failed <see cref="ServiceResult{T}"/> instance.</returns>
    public static ServiceResult<T> Failure(string errorCode, string errorMessage)
    {
        return new ServiceResult<T>(false, default, errorCode, errorMessage, null);
    }

    /// <summary>
    /// Creates a failed result with the specified error code, message, and correlation ID.
    /// </summary>
    /// <param name="errorCode">The error code from <see cref="ServiceError"/>.</param>
    /// <param name="errorMessage">The error message describing the failure.</param>
    /// <param name="correlationId">The correlation ID for request tracking.</param>
    /// <returns>A failed <see cref="ServiceResult{T}"/> instance.</returns>
    public static ServiceResult<T> Failure(string errorCode, string errorMessage, string correlationId)
    {
        return new ServiceResult<T>(false, default, errorCode, errorMessage, correlationId);
    }

    /// <summary>
    /// Implicitly converts a non-generic <see cref="ServiceResult"/> to <see cref="ServiceResult{T}"/>.
    /// This allows seamless integration between generic and non-generic result types.
    /// </summary>
    /// <param name="result">The non-generic result to convert.</param>
    public static implicit operator ServiceResult<T>(ServiceResult result)
    {
        return result.IsSuccess
            ? new ServiceResult<T>(true, default, null, null, result.CorrelationId)
            : new ServiceResult<T>(false, default, result.ErrorCode, result.ErrorMessage, result.CorrelationId);
    }
}
