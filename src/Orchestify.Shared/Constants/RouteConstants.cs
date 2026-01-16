namespace Orchestify.Shared.Constants;

/// <summary>
/// Defines standard route prefixes and patterns used throughout the Orchestify API.
/// Centralizes route definitions to ensure consistency and enable easier route management.
/// </summary>
public static class RouteConstants
{
    /// <summary>
    /// Base API route prefix.
    /// </summary>
    public const string ApiBase = "api";

    /// <summary>
    /// API version prefix.
    /// </summary>
    public const string ApiV1 = "v1";

    /// <summary>
    /// Full API base path including version.
    /// </summary>
    public const string ApiBaseV1 = "api/v1";

    /// <summary>
    /// Workspaces route prefix.
    /// </summary>
    public const string Workspaces = "workspaces";

    /// <summary>
    /// Boards route prefix.
    /// </summary>
    public const string Boards = "boards";

    /// <summary>
    /// Tasks route prefix.
    /// </summary>
    public const string Tasks = "tasks";

    /// <summary>
    /// Agents route prefix.
    /// </summary>
    public const string Agents = "agents";

    /// <summary>
    /// Queue route prefix.
    /// </summary>
    public const string Queue = "queue";

    /// <summary>
    /// Health check route.
    /// </summary>
    public const string Health = "health";

    /// <summary>
    /// Metrics route prefix.
    /// </summary>
    public const string Metrics = "metrics";

    /// <summary>
    /// Route parameter name for workspace identifier.
    /// </summary>
    public const string WorkspaceIdParam = "workspaceId";

    /// <summary>
    /// Route parameter name for board identifier.
    /// </summary>
    public const string BoardIdParam = "boardId";

    /// <summary>
    /// Route parameter name for task identifier.
    /// </summary>
    public const string TaskIdParam = "taskId";

    /// <summary>
    /// Route parameter name for agent identifier.
    /// </summary>
    public const string AgentIdParam = "agentId";

    /// <summary>
    /// Route parameter name for queue identifier.
    /// </summary>
    public const string QueueIdParam = "queueId";
}
