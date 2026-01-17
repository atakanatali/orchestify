using System;

namespace Orchestify.Domain.Entities;

/// <summary>
/// Represents a single step in an execution attempt's pipeline.
/// Steps are executed sequentially and track their own state.
/// </summary>
public class RunStepEntity
{
    /// <summary>
    /// Unique identifier for the step.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key to the parent attempt.
    /// </summary>
    public Guid AttemptId { get; set; }

    /// <summary>
    /// Type of step to execute.
    /// </summary>
    public Enums.RunStepType StepType { get; set; }

    /// <summary>
    /// Human-readable name of the step.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Current state of the step.
    /// </summary>
    public Enums.RunStepState State { get; set; } = Enums.RunStepState.Pending;

    /// <summary>
    /// Sequence number for ordering.
    /// </summary>
    public int SequenceNumber { get; set; }

    /// <summary>
    /// Date when the step started.
    /// </summary>
    public DateTime? StartedAt { get; set; }

    /// <summary>
    /// Date when the step completed.
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Duration of the step in milliseconds.
    /// </summary>
    public long? DurationMs { get; set; }

    /// <summary>
    /// Error message if the step failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Output/log content from the step.
    /// </summary>
    public string? Output { get; set; }

    /// <summary>
    /// Exit code if applicable.
    /// </summary>
    public int? ExitCode { get; set; }

    /// <summary>
    /// Navigation property to the parent attempt.
    /// </summary>
    public AttemptEntity? Attempt { get; set; }
}
