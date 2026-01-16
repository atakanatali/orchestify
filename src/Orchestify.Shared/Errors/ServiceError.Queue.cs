namespace Orchestify.Shared.Errors;

/// <summary>
/// Queue domain error codes for the Orchestify platform.
/// Covers background job queue, message queue, and processing operations.
/// </summary>
public static partial class ServiceError
{
    /// <summary>
    /// Queue domain identifier for error code formatting.
    /// </summary>
    private const string QueueDomain = "QUEUE";

    /// <summary>
    /// Error codes and messages for queue operations.
    /// </summary>
    public static class Queue
    {
        /// <summary>
        /// Queue job not found.
        /// </summary>
        public static string JobNotFound => FormatErrorCode(QueueDomain, "JOB_NOT_FOUND");

        /// <summary>
        /// Queue job already exists.
        /// </summary>
        public static string JobAlreadyExists => FormatErrorCode(QueueDomain, "JOB_ALREADY_EXISTS");

        /// <summary>
        /// Queue job is already queued.
        /// </summary>
        public static string AlreadyQueued => FormatErrorCode(QueueDomain, "ALREADY_QUEUED");

        /// <summary>
        /// Queue job is already processing.
        /// </summary>
        public static string AlreadyProcessing => FormatErrorCode(QueueDomain, "ALREADY_PROCESSING");

        /// <summary>
        /// Queue job execution failed.
        /// </summary>
        public static string ExecutionFailed => FormatErrorCode(QueueDomain, "EXECUTION_FAILED");

        /// <summary>
        /// Queue job execution timed out.
        /// </summary>
        public static string ExecutionTimeout => FormatErrorCode(QueueDomain, "EXECUTION_TIMEOUT");

        /// <summary>
        /// Queue job was cancelled.
        /// </summary>
        public static string Cancelled => FormatErrorCode(QueueDomain, "CANCELLED");

        /// <summary>
        /// Queue job retry limit exceeded.
        /// </summary>
        public static string RetryLimitExceeded => FormatErrorCode(QueueDomain, "RETRY_LIMIT_EXCEEDED");

        /// <summary>
        /// Queue job is in invalid state for operation.
        /// </summary>
        public static string InvalidState => FormatErrorCode(QueueDomain, "INVALID_STATE");

        /// <summary>
        /// Queue is full.
        /// </summary>
        public static string QueueFull => FormatErrorCode(QueueDomain, "QUEUE_FULL");

        /// <summary>
        /// Invalid queue job payload.
        /// </summary>
        public static string InvalidPayload => FormatErrorCode(QueueDomain, "INVALID_PAYLOAD");

        /// <summary>
        /// Queue job priority is invalid.
        /// </summary>
        public static string InvalidPriority => FormatErrorCode(QueueDomain, "INVALID_PRIORITY");

        /// <summary>
        /// Queue job schedule is invalid.
        /// </summary>
        public static string InvalidSchedule => FormatErrorCode(QueueDomain, "INVALID_SCHEDULE");

        /// <summary>
        /// Dead letter queue job not found.
        /// </summary>
        public static string DeadLetterNotFound => FormatErrorCode(QueueDomain, "DEAD_LETTER_NOT_FOUND");

        /// <summary>
        /// Cannot retry dead letter job.
        /// </summary>
        public static string CannotRetryDeadLetter => FormatErrorCode(QueueDomain, "CANNOT_RETRY_DEAD_LETTER");

        /// <summary>
        /// Queue worker not found.
        /// </summary>
        public static string WorkerNotFound => FormatErrorCode(QueueDomain, "WORKER_NOT_FOUND");

        /// <summary>
        /// Queue worker is unhealthy.
        /// </summary>
        public static string WorkerUnhealthy => FormatErrorCode(QueueDomain, "WORKER_UNHEALTHY");

        /// <summary>
        /// Queue processing delayed.
        /// </summary>
        public static string ProcessingDelayed => FormatErrorCode(QueueDomain, "PROCESSING_DELAYED");
    }
}
