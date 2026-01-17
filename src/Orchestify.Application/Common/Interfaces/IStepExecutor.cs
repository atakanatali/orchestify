using Orchestify.Domain.Entities;
using Orchestify.Domain.Enums;

namespace Orchestify.Application.Common.Interfaces;

/// <summary>
/// Interface for step executors.
/// Each step type has its own executor implementation.
/// </summary>
public interface IStepExecutor
{
    /// <summary>
    /// Gets the step type this executor handles.
    /// </summary>
    RunStepType StepType { get; }

    /// <summary>
    /// Executes the step.
    /// </summary>
    /// <param name="step">The step entity to execute.</param>
    /// <param name="attempt">The parent attempt.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result indicating success or failure with optional output.</returns>
    Task<StepExecutionResult> ExecuteAsync(RunStepEntity step, AttemptEntity attempt, CancellationToken cancellationToken);
}

/// <summary>
/// Result of a step execution.
/// </summary>
public class StepExecutionResult
{
    /// <summary>
    /// Whether the step succeeded.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Output from the step.
    /// </summary>
    public string? Output { get; set; }

    /// <summary>
    /// Error message if failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Exit code if applicable.
    /// </summary>
    public int? ExitCode { get; set; }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static StepExecutionResult Succeeded(string? output = null) => new()
    {
        Success = true,
        Output = output
    };

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    public static StepExecutionResult Failed(string errorMessage, string? output = null, int? exitCode = null) => new()
    {
        Success = false,
        ErrorMessage = errorMessage,
        Output = output,
        ExitCode = exitCode
    };
}
