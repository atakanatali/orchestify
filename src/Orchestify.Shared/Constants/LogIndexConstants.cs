namespace Orchestify.Shared.Constants;

/// <summary>
/// Defines Elasticsearch index names and patterns for logging.
/// Ensures consistency between log producers and consumers.
/// Indices follow the logs-* naming convention.
/// </summary>
public static class LogIndexConstants
{
    /// <summary>
    /// Prefix for all log indices.
    /// All indices must follow the pattern: logs-{purpose}-{date}
    /// </summary>
    public const string LogIndexPrefix = "logs";

    /// <summary>
    /// Index pattern for application logs.
    /// Matches: logs-application-YYYY.MM.DD
    /// </summary>
    public const string ApplicationIndexPattern = "logs-application-*";

    /// <summary>
    /// Index name format for application logs with date placeholder.
    /// Use with: string.Format(ApplicationIndexFormat, date)
    /// </summary>
    public const string ApplicationIndexFormat = "logs-application-{0:yyyy.MM.dd}";

    /// <summary>
    /// Index pattern for audit logs.
    /// Matches: logs-audit-YYYY.MM.DD
    /// </summary>
    public const string AuditIndexPattern = "logs-audit-*";

    /// <summary>
    /// Index name format for audit logs with date placeholder.
    /// Use with: string.Format(AuditIndexFormat, date)
    /// </summary>
    public const string AuditIndexFormat = "logs-audit-{0:yyyy.MM.dd}";

    /// <summary>
    /// Index pattern for error logs.
    /// Matches: logs-error-YYYY.MM.DD
    /// </summary>
    public const string ErrorIndexPattern = "logs-error-*";

    /// <summary>
    /// Index name format for error logs with date placeholder.
    /// Use with: string.Format(ErrorIndexFormat, date)
    /// </summary>
    public const string ErrorIndexFormat = "logs-error-{0:yyyy.MM.dd}";

    /// <summary>
    /// Index pattern for agent execution logs.
    /// Matches: logs-agent-execution-YYYY.MM.DD
    /// </summary>
    public const string AgentExecutionIndexPattern = "logs-agent-execution-*";

    /// <summary>
    /// Index name format for agent execution logs with date placeholder.
    /// Use with: string.Format(AgentExecutionIndexFormat, date)
    /// </summary>
    public const string AgentExecutionIndexFormat = "logs-agent-execution-{0:yyyy.MM.dd}";

    /// <summary>
    /// Index pattern for queue processing logs.
    /// Matches: logs-queue-YYYY.MM.DD
    /// </summary>
    public const string QueueIndexPattern = "logs-queue-*";

    /// <summary>
    /// Index name format for queue processing logs with date placeholder.
    /// Use with: string.Format(QueueIndexFormat, date)
    /// </summary>
    public const string QueueIndexFormat = "logs-queue-{0:yyyy.MM.dd}";

    /// <summary>
    /// Index pattern for API logs.
    /// Matches: logs-orchestify-api-*
    /// </summary>
    public const string ApiIndexPattern = "logs-orchestify-api-*";

    /// <summary>
    /// Index name format for API logs with date placeholder.
    /// Use with: string.Format(ApiIndexFormat, date)
    /// </summary>
    public const string ApiIndexFormat = "logs-orchestify-api-{0:yyyy.MM.dd}";

    /// <summary>
    /// Index pattern for Worker logs.
    /// Matches: logs-orchestify-worker-*
    /// </summary>
    public const string WorkerIndexPattern = "logs-orchestify-worker-*";

    /// <summary>
    /// Index name format for Worker logs with date placeholder.
    /// Use with: string.Format(WorkerIndexFormat, date)
    /// </summary>
    public const string WorkerIndexFormat = "logs-orchestify-worker-{0:yyyy.MM.dd}";

    /// <summary>
    /// Wildcard pattern matching all Orchestify logs.
    /// Use this in Kibana index patterns to query all application logs.
    /// </summary>
    public const string AllLogsPattern = "logs-*";
}
