using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orchestify.Application.Common.Interfaces;
using Orchestify.Domain.Entities;
using Orchestify.Domain.Enums;

namespace Orchestify.Infrastructure.Services;

/// <summary>
/// Implements step pipeline creation and execution.
/// </summary>
public class StepPipelineService : IStepPipelineService
{
    private readonly IApplicationDbContext _context;
    private readonly IEnumerable<IStepExecutor> _stepExecutors;
    private readonly ILogger<StepPipelineService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="StepPipelineService"/> class.
    /// </summary>
    public StepPipelineService(
        IApplicationDbContext context,
        IEnumerable<IStepExecutor> stepExecutors,
        ILogger<StepPipelineService> logger)
    {
        _context = context;
        _stepExecutors = stepExecutors;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Guid>> CreatePipelineStepsAsync(AttemptEntity attempt, CancellationToken cancellationToken)
    {
        var steps = new List<RunStepEntity>
        {
            new()
            {
                Id = Guid.NewGuid(),
                AttemptId = attempt.Id,
                StepType = RunStepType.Restore,
                Name = "Restore Dependencies",
                State = RunStepState.Pending,
                SequenceNumber = 1
            },
            new()
            {
                Id = Guid.NewGuid(),
                AttemptId = attempt.Id,
                StepType = RunStepType.Build,
                Name = "Build Project",
                State = RunStepState.Pending,
                SequenceNumber = 2
            },
            new()
            {
                Id = Guid.NewGuid(),
                AttemptId = attempt.Id,
                StepType = RunStepType.Test,
                Name = "Run Tests",
                State = RunStepState.Pending,
                SequenceNumber = 3
            },
            new()
            {
                Id = Guid.NewGuid(),
                AttemptId = attempt.Id,
                StepType = RunStepType.Agent,
                Name = "Execute Agent",
                State = RunStepState.Pending,
                SequenceNumber = 4
            },
            new()
            {
                Id = Guid.NewGuid(),
                AttemptId = attempt.Id,
                StepType = RunStepType.Review,
                Name = "Generate Review",
                State = RunStepState.Pending,
                SequenceNumber = 5
            }
        };

        _context.RunSteps.AddRange(steps);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created {Count} pipeline steps for attempt {AttemptId}", steps.Count, attempt.Id);

        return steps.Select(s => s.Id).ToList();
    }

    /// <inheritdoc />
    public async Task<bool> ExecutePipelineAsync(AttemptEntity attempt, CancellationToken cancellationToken)
    {
        var steps = await _context.RunSteps
            .Where(s => s.AttemptId == attempt.Id)
            .OrderBy(s => s.SequenceNumber)
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Executing {Count} pipeline steps for attempt {AttemptId}", steps.Count, attempt.Id);

        foreach (var step in steps)
        {
            if (step.State != RunStepState.Pending)
            {
                continue;
            }

            var executor = _stepExecutors.FirstOrDefault(e => e.StepType == step.StepType);
            if (executor == null)
            {
                _logger.LogWarning("No executor found for step type {StepType}, skipping", step.StepType);
                step.State = RunStepState.Skipped;
                await _context.SaveChangesAsync(cancellationToken);
                continue;
            }

            step.State = RunStepState.Running;
            step.StartedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Executing step {StepName} ({StepType})", step.Name, step.StepType);

            try
            {
                var result = await executor.ExecuteAsync(step, attempt, cancellationToken);

                step.CompletedAt = DateTime.UtcNow;
                step.DurationMs = (long)(step.CompletedAt.Value - step.StartedAt!.Value).TotalMilliseconds;
                step.Output = result.Output;
                step.ExitCode = result.ExitCode;

                if (result.Success)
                {
                    step.State = RunStepState.Succeeded;
                    _logger.LogInformation("Step {StepName} succeeded", step.Name);
                }
                else
                {
                    step.State = RunStepState.Failed;
                    step.ErrorMessage = result.ErrorMessage;
                    _logger.LogError("Step {StepName} failed: {Error}", step.Name, result.ErrorMessage);
                    await _context.SaveChangesAsync(cancellationToken);
                    return false;
                }

                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                step.State = RunStepState.Failed;
                step.CompletedAt = DateTime.UtcNow;
                step.DurationMs = (long)(step.CompletedAt.Value - step.StartedAt!.Value).TotalMilliseconds;
                step.ErrorMessage = ex.Message;
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogError(ex, "Step {StepName} threw exception", step.Name);
                return false;
            }
        }

        return true;
    }
}
