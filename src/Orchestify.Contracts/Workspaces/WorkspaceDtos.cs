using System;

namespace Orchestify.Contracts.Workspaces;

/// <summary>
/// DTO representing the full details of a workspace.
/// </summary>
public class WorkspaceDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string RepositoryPath { get; set; } = string.Empty;
    public string DefaultBranch { get; set; } = string.Empty;
    public int TotalTasks { get; set; }
    public int RunningTasks { get; set; }
    public DateTime? LastActivityAt { get; set; }
    public int ProgressPercent { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Response wrapper for a single workspace.
/// </summary>
public class WorkspaceResponseDto
{
    public WorkspaceDto Workspace { get; set; } = new();
}

/// <summary>
/// DTO for a workspace item in a list view.
/// </summary>
public class WorkspaceListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string RepositoryPath { get; set; } = string.Empty;
    public string DefaultBranch { get; set; } = string.Empty;
    public int TotalTasks { get; set; }
    public int RunningTasks { get; set; }
    public DateTime? LastActivityAt { get; set; }
    public int ProgressPercent { get; set; }
}

/// <summary>
/// Response wrapper for a list of workspaces.
/// </summary>
public class WorkspacesListResponseDto
{
    public List<WorkspaceListItemDto> Items { get; set; } = new();
}

/// <summary>
/// Request DTO for creating a new workspace.
/// </summary>
public class CreateWorkspaceRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string RepositoryPath { get; set; } = string.Empty;
    public string DefaultBranch { get; set; } = string.Empty;
}

/// <summary>
/// Request DTO for updating an existing workspace.
/// </summary>
public class UpdateWorkspaceRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string DefaultBranch { get; set; } = string.Empty;
}
