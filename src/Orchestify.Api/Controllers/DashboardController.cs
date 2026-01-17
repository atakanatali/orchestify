using MediatR;
using Microsoft.AspNetCore.Mvc;
using Orchestify.Application.Actions.Dashboard.GetDashboardStats;

namespace Orchestify.Api.Controllers;

/// <summary>
/// Dashboard statistics endpoint.
/// </summary>
[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets dashboard statistics.
    /// </summary>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(DashboardStatsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats()
    {
        var result = await _mediator.Send(new GetDashboardStatsQuery());
        return Ok(result.Value);
    }
}
