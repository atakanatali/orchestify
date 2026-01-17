namespace Orchestify.Domain.Enums;

/// <summary>
/// Represents the type of a run step in the pipeline.
/// </summary>
public enum RunStepType
{
    /// <summary>
    /// Restore dependencies step.
    /// </summary>
    Restore = 0,

    /// <summary>
    /// Build step.
    /// </summary>
    Build = 1,

    /// <summary>
    /// Test step.
    /// </summary>
    Test = 2,

    /// <summary>
    /// Agent execution step.
    /// </summary>
    Agent = 3,

    /// <summary>
    /// Code review step.
    /// </summary>
    Review = 4,

    /// <summary>
    /// Custom script step.
    /// </summary>
    Script = 5
}

/// <summary>
/// Represents the state of a run step.
/// </summary>
public enum RunStepState
{
    /// <summary>
    /// Step is pending execution.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Step is currently running.
    /// </summary>
    Running = 1,

    /// <summary>
    /// Step completed successfully.
    /// </summary>
    Succeeded = 2,

    /// <summary>
    /// Step failed.
    /// </summary>
    Failed = 3,

    /// <summary>
    /// Step was skipped.
    /// </summary>
    Skipped = 4
}
