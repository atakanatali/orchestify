namespace Orchestify.Contracts.Tasks;

/// <summary>
/// Full task details DTO.
/// </summary>
public class TaskDto
{
    /// <summary>
    /// Unique identifier for the task.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Parent board identifier.
    /// </summary>
    public Guid BoardId { get; set; }

    /// <summary>
    /// Human-readable title of the task.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of the task.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Current status of the task.
    /// </summary>
    public string Status { get; set; } = "Todo";

    /// <summary>
    /// Order key for Kanban positioning.
    /// </summary>
    public int OrderKey { get; set; }

    /// <summary>
    /// Priority level (1 = highest, 5 = lowest).
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// Number of execution attempts.
    /// </summary>
    public int AttemptCount { get; set; }

    /// <summary>
    /// Date when the task was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date when the task was completed.
    /// </summary>
    public DateTime? CompletedAt { get; set; }
}

/// <summary>
/// Response DTO containing a single task.
/// </summary>
public class TaskResponseDto
{
    /// <summary>
    /// The task details.
    /// </summary>
    public TaskDto Task { get; set; } = new();
}

/// <summary>
/// List item DTO for tasks.
/// </summary>
public class TaskListItemDto
{
    /// <summary>
    /// Unique identifier for the task.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Human-readable title of the task.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Current status of the task.
    /// </summary>
    public string Status { get; set; } = "Todo";

    /// <summary>
    /// Order key for Kanban positioning.
    /// </summary>
    public int OrderKey { get; set; }

    /// <summary>
    /// Priority level.
    /// </summary>
    public int Priority { get; set; }
}

/// <summary>
/// Response DTO containing a list of tasks.
/// </summary>
public class TasksListResponseDto
{
    /// <summary>
    /// List of tasks.
    /// </summary>
    public List<TaskListItemDto> Items { get; set; } = new();
}

/// <summary>
/// Request DTO for creating a new task.
/// </summary>
public class CreateTaskRequestDto
{
    /// <summary>
    /// Human-readable title of the task.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of the task.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Priority level (1 = highest, 5 = lowest). Default is 3.
    /// </summary>
    public int Priority { get; set; } = 3;
}

/// <summary>
/// Request DTO for updating an existing task.
/// </summary>
public class UpdateTaskRequestDto
{
    /// <summary>
    /// Updated title of the task.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Updated description of the task.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Updated status of the task.
    /// </summary>
    public string Status { get; set; } = "Todo";

    /// <summary>
    /// Updated priority level.
    /// </summary>
    public int Priority { get; set; } = 3;
}
