namespace Orchestify.Shared.Errors;

/// <summary>
/// Agent domain error codes for the Orchestify platform.
/// Covers AI agent management, execution, and provider operations.
/// </summary>
public static partial class ServiceError
{
    /// <summary>
    /// Agent domain identifier for error code formatting.
    /// </summary>
    private const string AgentDomain = "AGENT";

    /// <summary>
    /// Error codes and messages for agent operations.
    /// </summary>
    public static class Agents
    {
        /// <summary>
        /// Agent not found.
        /// </summary>
        public static string NotFound => FormatErrorCode(AgentDomain, "NOT_FOUND");

        /// <summary>
        /// Agent name already exists.
        /// </summary>
        public static string NameAlreadyExists => FormatErrorCode(AgentDomain, "NAME_EXISTS");

        /// <summary>
        /// Agent name is invalid.
        /// </summary>
        public static string InvalidName => FormatErrorCode(AgentDomain, "INVALID_NAME");

        /// <summary>
        /// Invalid agent type specified.
        /// </summary>
        public static string InvalidType => FormatErrorCode(AgentDomain, "INVALID_TYPE");

        /// <summary>
        /// Invalid agent configuration.
        /// </summary>
        public static string InvalidConfiguration => FormatErrorCode(AgentDomain, "INVALID_CONFIGURATION");

        /// <summary>
        /// Agent provider not configured.
        /// </summary>
        public static string ProviderNotConfigured => FormatErrorCode(AgentDomain, "PROVIDER_NOT_CONFIGURED");

        /// <summary>
        /// Agent provider not found.
        /// </summary>
        public static string ProviderNotFound => FormatErrorCode(AgentDomain, "PROVIDER_NOT_FOUND");

        /// <summary>
        /// Agent is disabled.
        /// </summary>
        public static string IsDisabled => FormatErrorCode(AgentDomain, "IS_DISABLED");

        /// <summary>
        /// Agent is already running.
        /// </summary>
        public static string AlreadyRunning => FormatErrorCode(AgentDomain, "ALREADY_RUNNING");

        /// <summary>
        /// Agent execution failed.
        /// </summary>
        public static string ExecutionFailed => FormatErrorCode(AgentDomain, "EXECUTION_FAILED");

        /// <summary>
        /// Agent execution timed out.
        /// </summary>
        public static string ExecutionTimeout => FormatErrorCode(AgentDomain, "EXECUTION_TIMEOUT");

        /// <summary>
        /// Agent execution was cancelled.
        /// </summary>
        public static string ExecutionCancelled => FormatErrorCode(AgentDomain, "EXECUTION_CANCELLED");

        /// <summary>
        /// Agent has insufficient permissions.
        /// </summary>
        public static string InsufficientPermissions => FormatErrorCode(AgentDomain, "INSUFFICIENT_PERMISSIONS");

        /// <summary>
        /// Agent quota exceeded.
        /// </summary>
        public static string QuotaExceeded => FormatErrorCode(AgentDomain, "QUOTA_EXCEEDED");

        /// <summary>
        /// Agent rate limit exceeded.
        /// </summary>
        public static string RateLimitExceeded => FormatErrorCode(AgentDomain, "RATE_LIMIT_EXCEEDED");

        /// <summary>
        /// Invalid agent prompt or instruction.
        /// </summary>
        public static string InvalidPrompt => FormatErrorCode(AgentDomain, "INVALID_PROMPT");

        /// <summary>
        /// Agent tool not found.
        /// </summary>
        public static string ToolNotFound => FormatErrorCode(AgentDomain, "TOOL_NOT_FOUND");

        /// <summary>
        /// Agent tool execution failed.
        /// </summary>
        public static string ToolExecutionFailed => FormatErrorCode(AgentDomain, "TOOL_EXECUTION_FAILED");

        /// <summary>
        /// Agent memory context limit exceeded.
        /// </summary>
        public static string MemoryLimitExceeded => FormatErrorCode(AgentDomain, "MEMORY_LIMIT_EXCEEDED");

        /// <summary>
        /// Agent response parsing failed.
        /// </summary>
        public static string ResponseParseFailed => FormatErrorCode(AgentDomain, "RESPONSE_PARSE_FAILED");

        /// <summary>
        /// Agent execution history not found.
        /// </summary>
        public static string HistoryNotFound => FormatErrorCode(AgentDomain, "HISTORY_NOT_FOUND");
    }
}
