using MediatR;
using Microsoft.EntityFrameworkCore;
using Orchestify.Application.Common.Interfaces;
using Orchestify.Domain.Enums;
using Orchestify.Shared.Results;
using TaskStatus = Orchestify.Domain.Enums.TaskStatus;

namespace Orchestify.Application.Actions.Dashboard.GetDashboardStats;

/// <summary>
/// Handler for getting dashboard stats.
/// </summary>
public class GetDashboardStatsHandler : IRequestHandler<GetDashboardStatsQuery, ServiceResult<DashboardStatsDto>>
{
    private readonly IApplicationDbContext _context;

    public GetDashboardStatsHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResult<DashboardStatsDto>> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        var totalWorkspaces = await _context.Workspaces.CountAsync(cancellationToken);
        var totalBoards = await _context.Boards.CountAsync(cancellationToken);
        var totalTasks = await _context.Tasks.CountAsync(cancellationToken);

        var pendingTasks = await _context.Tasks.CountAsync(t => t.Status == TaskStatus.Todo, cancellationToken);
        var inProgressTasks = await _context.Tasks.CountAsync(t => t.Status == TaskStatus.InProgress, cancellationToken);
        var completedTasks = await _context.Tasks.CountAsync(t => t.Status == TaskStatus.Done, cancellationToken);

        var queuedAttempts = await _context.Attempts.CountAsync(a => a.State == AttemptState.Queued, cancellationToken);
        var runningAttempts = await _context.Attempts.CountAsync(a => a.State == AttemptState.Running, cancellationToken);
        var failedAttempts = await _context.Attempts.CountAsync(a => a.State == AttemptState.Failed, cancellationToken);

        return ServiceResult<DashboardStatsDto>.Success(new DashboardStatsDto
        {
            TotalWorkspaces = totalWorkspaces,
            TotalBoards = totalBoards,
            TotalTasks = totalTasks,
            PendingTasks = pendingTasks,
            InProgressTasks = inProgressTasks,
            CompletedTasks = completedTasks,
            QueuedAttempts = queuedAttempts,
            RunningAttempts = runningAttempts,
            FailedAttempts = failedAttempts
        });
    }
}
