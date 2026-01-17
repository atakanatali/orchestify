namespace Orchestify.Contracts.Tasks;

/// <summary>
/// Request DTO for moving a task to a new position.
/// </summary>
public class MoveTaskRequestDto
{
    /// <summary>
    /// New status for the task (optional, for column change).
    /// </summary>
    public string? NewStatus { get; set; }

    /// <summary>
    /// Target order key for positioning.
    /// </summary>
    public int TargetOrderKey { get; set; }

    /// <summary>
    /// ID of the task to position after (optional).
    /// </summary>
    public Guid? AfterTaskId { get; set; }

    /// <summary>
    /// ID of the task to position before (optional).
    /// </summary>
    public Guid? BeforeTaskId { get; set; }
}
