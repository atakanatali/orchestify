using Microsoft.AspNetCore.Mvc;
using Orchestify.Application.Common.Interfaces;

namespace Orchestify.Api.Controllers;

/// <summary>
/// Build and test operations endpoint.
/// </summary>
[ApiController]
[Route("api/workspaces/{workspaceId:guid}/build")]
public class BuildController : ControllerBase
{
    private readonly IProcessRunner _processRunner;
    private readonly IApplicationDbContext _context;

    public BuildController(IProcessRunner processRunner, IApplicationDbContext context)
    {
        _processRunner = processRunner;
        _context = context;
    }

    /// <summary>
    /// Restores dependencies.
    /// </summary>
    [HttpPost("restore")]
    public async Task<IActionResult> Restore(Guid workspaceId)
    {
        var workspace = await _context.Workspaces.FindAsync(workspaceId);
        if (workspace == null) return NotFound();

        var result = await _processRunner.RunDotnetAsync("restore", workspace.RepositoryPath, default);
        return Ok(new { success = result.Success, output = result.Output, error = result.Error, durationMs = result.Duration.TotalMilliseconds });
    }

    /// <summary>
    /// Builds the project.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Build(Guid workspaceId)
    {
        var workspace = await _context.Workspaces.FindAsync(workspaceId);
        if (workspace == null) return NotFound();

        var result = await _processRunner.RunDotnetAsync("build --no-restore", workspace.RepositoryPath, default);
        return Ok(new { success = result.Success, output = result.Output, error = result.Error, durationMs = result.Duration.TotalMilliseconds });
    }

    /// <summary>
    /// Runs tests.
    /// </summary>
    [HttpPost("test")]
    public async Task<IActionResult> Test(Guid workspaceId)
    {
        var workspace = await _context.Workspaces.FindAsync(workspaceId);
        if (workspace == null) return NotFound();

        var result = await _processRunner.RunDotnetAsync("test --no-build", workspace.RepositoryPath, default);
        return Ok(new { success = result.Success, output = result.Output, error = result.Error, durationMs = result.Duration.TotalMilliseconds });
    }
}
