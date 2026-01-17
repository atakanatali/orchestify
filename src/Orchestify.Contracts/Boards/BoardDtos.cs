namespace Orchestify.Contracts.Boards;

/// <summary>
/// Full board details DTO.
/// </summary>
public class BoardDto
{
    /// <summary>
    /// Unique identifier for the board.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Parent workspace identifier.
    /// </summary>
    public Guid WorkspaceId { get; set; }

    /// <summary>
    /// Human-readable name of the board.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional description of the board.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Total number of tasks in this board.
    /// </summary>
    public int TotalTasks { get; set; }

    /// <summary>
    /// Number of completed tasks in this board.
    /// </summary>
    public int CompletedTasks { get; set; }

    /// <summary>
    /// Date when the board was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Response DTO containing a single board.
/// </summary>
public class BoardResponseDto
{
    /// <summary>
    /// The board details.
    /// </summary>
    public BoardDto Board { get; set; } = new();
}

/// <summary>
/// List item DTO for boards.
/// </summary>
public class BoardListItemDto
{
    /// <summary>
    /// Unique identifier for the board.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Human-readable name of the board.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional description of the board.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Total number of tasks in this board.
    /// </summary>
    public int TotalTasks { get; set; }

    /// <summary>
    /// Number of completed tasks.
    /// </summary>
    public int CompletedTasks { get; set; }
}

/// <summary>
/// Response DTO containing a list of boards.
/// </summary>
public class BoardsListResponseDto
{
    /// <summary>
    /// List of boards.
    /// </summary>
    public List<BoardListItemDto> Items { get; set; } = new();
}

/// <summary>
/// Request DTO for creating a new board.
/// </summary>
public class CreateBoardRequestDto
{
    /// <summary>
    /// Human-readable name of the board. Must be unique within the workspace.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional description of the board.
    /// </summary>
    public string? Description { get; set; }
}

/// <summary>
/// Request DTO for updating an existing board.
/// </summary>
public class UpdateBoardRequestDto
{
    /// <summary>
    /// Updated name of the board.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Updated description of the board.
    /// </summary>
    public string? Description { get; set; }
}
