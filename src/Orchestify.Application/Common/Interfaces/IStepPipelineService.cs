using Orchestify.Domain.Entities;

namespace Orchestify.Application.Common.Interfaces;

/// <summary>
/// Service for executing step pipelines.
/// </summary>
public interface IStepPipelineService
{
    /// <summary>
    /// Creates the default pipeline steps for an attempt.
    /// </summary>
    /// <param name="attempt">The attempt to create steps for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of created step IDs.</returns>
    Task<IReadOnlyList<Guid>> CreatePipelineStepsAsync(AttemptEntity attempt, CancellationToken cancellationToken);

    /// <summary>
    /// Executes all pending steps for an attempt.
    /// </summary>
    /// <param name="attempt">The attempt to execute.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if all steps succeeded, false otherwise.</returns>
    Task<bool> ExecutePipelineAsync(AttemptEntity attempt, CancellationToken cancellationToken);
}
