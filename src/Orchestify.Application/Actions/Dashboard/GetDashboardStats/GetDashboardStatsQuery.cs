using MediatR;
using Orchestify.Shared.Results;

namespace Orchestify.Application.Actions.Dashboard.GetDashboardStats;

/// <summary>
/// Query to get dashboard statistics.
/// </summary>
public record GetDashboardStatsQuery() : IRequest<ServiceResult<DashboardStatsDto>>;

/// <summary>
/// Dashboard statistics DTO.
/// </summary>
public class DashboardStatsDto
{
    public int TotalWorkspaces { get; set; }
    public int TotalBoards { get; set; }
    public int TotalTasks { get; set; }
    public int PendingTasks { get; set; }
    public int InProgressTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int QueuedAttempts { get; set; }
    public int RunningAttempts { get; set; }
    public int FailedAttempts { get; set; }
}
