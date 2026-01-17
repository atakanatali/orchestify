using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orchestify.Application.Common.Interfaces;

namespace Orchestify.Worker.Services;

/// <summary>
/// Background service that processes queued attempts from the database.
/// Polls the queue periodically and executes attempts step-by-step.
/// </summary>
public class AttemptProcessorService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AttemptProcessorService> _logger;
    private readonly TimeSpan _pollInterval = TimeSpan.FromSeconds(5);
    private readonly TimeSpan _heartbeatInterval = TimeSpan.FromSeconds(30);
    private readonly int _staleTimeoutSeconds = 120;

    /// <summary>
    /// Initializes a new instance of the <see cref="AttemptProcessorService"/> class.
    /// </summary>
    /// <param name="scopeFactory">Factory for creating DI scopes.</param>
    /// <param name="logger">Logger instance.</param>
    public AttemptProcessorService(
        IServiceScopeFactory scopeFactory,
        ILogger<AttemptProcessorService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("AttemptProcessorService started. Poll interval: {Interval}s", _pollInterval.TotalSeconds);

        // Start stale recovery task
        _ = RecoverStaleAttemptsLoop(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessNextAttemptAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing attempt");
            }

            await Task.Delay(_pollInterval, stoppingToken);
        }

        _logger.LogInformation("AttemptProcessorService stopping");
    }

    private async Task ProcessNextAttemptAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var queueService = scope.ServiceProvider.GetRequiredService<IAttemptQueueService>();
        var pipelineService = scope.ServiceProvider.GetRequiredService<IStepPipelineService>();
        var context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

        var attempt = await queueService.DequeueNextAsync(cancellationToken);

        if (attempt == null)
        {
            _logger.LogDebug("No queued attempts found");
            return;
        }

        _logger.LogInformation("Processing attempt {AttemptId} for task {TaskId}", attempt.Id, attempt.TaskId);

        try
        {
            // Mark as running
            await queueService.MarkAsRunningAsync(attempt.Id, cancellationToken);

            // Start heartbeat task
            using var heartbeatCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var heartbeatTask = HeartbeatLoop(queueService, attempt.Id, heartbeatCts.Token);

            try
            {
                // Create pipeline steps if not exists
                var existingSteps = await context.RunSteps
                    .Where(s => s.AttemptId == attempt.Id)
                    .AnyAsync(cancellationToken);

                if (!existingSteps)
                {
                    await pipelineService.CreatePipelineStepsAsync(attempt, cancellationToken);
                }

                // Execute the pipeline
                var success = await pipelineService.ExecutePipelineAsync(attempt, cancellationToken);

                if (success)
                {
                    await queueService.MarkAsSucceededAsync(attempt.Id, cancellationToken);
                    _logger.LogInformation("Attempt {AttemptId} completed successfully", attempt.Id);
                }
                else
                {
                    await queueService.MarkAsFailedAsync(attempt.Id, "Pipeline execution failed", cancellationToken);
                    _logger.LogWarning("Attempt {AttemptId} failed", attempt.Id);
                }
            }
            finally
            {
                heartbeatCts.Cancel();
                try { await heartbeatTask; } catch { /* Ignore */ }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Attempt {AttemptId} was cancelled", attempt.Id);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Attempt {AttemptId} failed", attempt.Id);
            await queueService.MarkAsFailedAsync(attempt.Id, ex.Message, cancellationToken);
        }
    }

    private async Task HeartbeatLoop(IAttemptQueueService queueService, Guid attemptId, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(_heartbeatInterval, cancellationToken);
                await queueService.UpdateHeartbeatAsync(attemptId, CancellationToken.None);
                _logger.LogDebug("Updated heartbeat for attempt {AttemptId}", attemptId);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to update heartbeat for attempt {AttemptId}", attemptId);
            }
        }
    }

    private async Task RecoverStaleAttemptsLoop(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);

                using var scope = _scopeFactory.CreateScope();
                var queueService = scope.ServiceProvider.GetRequiredService<IAttemptQueueService>();

                var recovered = await queueService.RecoverStaleAttemptsAsync(_staleTimeoutSeconds, cancellationToken);
                if (recovered > 0)
                {
                    _logger.LogWarning("Recovered {Count} stale attempts", recovered);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recovering stale attempts");
            }
        }
    }
}
