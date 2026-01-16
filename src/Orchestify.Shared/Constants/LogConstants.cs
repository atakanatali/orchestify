namespace Orchestify.Shared.Constants;

/// <summary>
/// Defines logging-related constants used across the application.
/// Ensures consistency in log property names and service identifiers.
/// </summary>
public static class LogConstants
{
    /// <summary>
    /// Property name for the service name in log entries.
    /// </summary>
    public const string ServiceNamePropertyName = "ServiceName";

    /// <summary>
    /// Property name for the environment in log entries.
    /// </summary>
    public const string EnvironmentPropertyName = "Environment";

    /// <summary>
    /// Property name for the machine name in log entries.
    /// </summary>
    public const string MachineNamePropertyName = "MachineName";

    /// <summary>
    /// Property name for the process ID in log entries.
    /// </summary>
    public const string ProcessIdPropertyName = "ProcessId";

    /// <summary>
    /// Property name for the thread ID in log entries.
    /// </summary>
    public const string ThreadIdPropertyName = "ThreadId";

    /// <summary>
    /// Property name for the correlation ID in log entries.
    /// </summary>
    public const string CorrelationIdPropertyName = "CorrelationId";

    /// <summary>
    /// Property name for the log event code.
    /// </summary>
    public const string CodePropertyName = "Code";

    /// <summary>
    /// Property name for structured data in log entries.
    /// </summary>
    public const string DataPropertyName = "Data";

    /// <summary>
    /// Property name for exception type in error logs.
    /// </summary>
    public const string ExceptionTypePropertyName = "ExceptionType";

    /// <summary>
    /// Property name for exception message in error logs.
    /// </summary>
    public const string ExceptionMessagePropertyName = "ExceptionMessage";

    /// <summary>
    /// Property name for exception stack trace in error logs.
    /// </summary>
    public const string ExceptionStackTracePropertyName = "ExceptionStackTrace";

    /// <summary>
    /// Property name for serialized exception details in error logs.
    /// </summary>
    public const string ExceptionDetailsPropertyName = "ExceptionDetails";

    /// <summary>
    /// Service name identifier for the API.
    /// </summary>
    public const string ApiServiceName = "orchestify-api";

    /// <summary>
    /// Service name identifier for the Worker.
    /// </summary>
    public const string WorkerServiceName = "orchestify-worker";
}
