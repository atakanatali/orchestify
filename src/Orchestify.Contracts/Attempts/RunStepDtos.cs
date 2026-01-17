namespace Orchestify.Contracts.Attempts;

/// <summary>
/// DTO for a run step.
/// </summary>
public class RunStepDto
{
    public Guid Id { get; set; }
    public Guid AttemptId { get; set; }
    public string StepType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public int SequenceNumber { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public long? DurationMs { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Response DTO for list of run steps.
/// </summary>
public class RunStepsListResponseDto
{
    public List<RunStepDto> Items { get; set; } = new();
}
