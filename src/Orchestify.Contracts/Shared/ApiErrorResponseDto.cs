namespace Orchestify.Contracts.Shared;

/// <summary>
/// Standardized error response DTO for API failures.
/// Used by controllers to return consistent error responses to clients.
/// Maps from <see cref="Orchestify.Shared.Results.ServiceResult"/> failures.
/// </summary>
public sealed class ApiErrorResponseDto
{
    /// <summary>
    /// Gets the error code from <see cref="Orchestify.Shared.Errors.ServiceError"/>.
    /// Examples: ORC_CORE_NOT_FOUND, ORC_WS_NOT_MEMBER, ORC_TASK_CIRCULAR_DEPENDENCY
    /// </summary>
    public string Code { get; init; } = string.Empty;

    /// <summary>
    /// Gets the human-readable error message describing what went wrong.
    /// This message is safe to display to clients and does not contain sensitive information.
    /// </summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// Gets the correlation ID for tracing the request across service boundaries.
    /// Null when correlation tracking is not enabled for the request.
    /// </summary>
    public string? CorrelationId { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiErrorResponseDto"/> class.
    /// </summary>
    public ApiErrorResponseDto()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiErrorResponseDto"/> class.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <param name="correlationId">The correlation ID for request tracking.</param>
    public ApiErrorResponseDto(string code, string message, string? correlationId = null)
    {
        Code = code;
        Message = message;
        CorrelationId = correlationId;
    }

    /// <summary>
    /// Creates an <see cref="ApiErrorResponseDto"/> from a <see cref="Orchestify.Shared.Results.ServiceResult"/>.
    /// </summary>
    /// <param name="result">The failed service result.</param>
    /// <returns>An <see cref="ApiErrorResponseDto"/> instance.</returns>
    public static ApiErrorResponseDto FromResult(Orchestify.Shared.Results.ServiceResult result)
    {
        if (result.IsSuccess)
        {
            throw new ArgumentException("Cannot create error response from successful result.", nameof(result));
        }

        return new ApiErrorResponseDto(
            result.ErrorCode ?? Orchestify.Shared.Errors.ServiceError.Core.Unknown,
            result.ErrorMessage ?? "An unknown error occurred.",
            result.CorrelationId
        );
    }
}
