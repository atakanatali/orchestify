using Microsoft.AspNetCore.Mvc;
using Orchestify.Application.Common.Interfaces;

namespace Orchestify.Api.Controllers;

/// <summary>
/// Git operations endpoint.
/// </summary>
[ApiController]
[Route("api/workspaces/{workspaceId:guid}/git")]
public class GitController : ControllerBase
{
    private readonly IGitService _gitService;
    private readonly IApplicationDbContext _context;

    public GitController(IGitService gitService, IApplicationDbContext context)
    {
        _gitService = gitService;
        _context = context;
    }

    /// <summary>
    /// Gets current branch for a workspace.
    /// </summary>
    [HttpGet("branch")]
    public async Task<IActionResult> GetCurrentBranch(Guid workspaceId)
    {
        var workspace = await _context.Workspaces.FindAsync(workspaceId);
        if (workspace == null) return NotFound();

        var result = await _gitService.GetCurrentBranchAsync(workspace.RepositoryPath, default);
        if (!result.Success) return BadRequest(new { error = result.Error });

        return Ok(new { branch = result.Output });
    }

    /// <summary>
    /// Pulls latest changes for a workspace.
    /// </summary>
    [HttpPost("pull")]
    public async Task<IActionResult> Pull(Guid workspaceId)
    {
        var workspace = await _context.Workspaces.FindAsync(workspaceId);
        if (workspace == null) return NotFound();

        var result = await _gitService.PullAsync(workspace.RepositoryPath, default);
        if (!result.Success) return BadRequest(new { error = result.Error });

        return Ok(new { output = result.Output });
    }

    /// <summary>
    /// Checks out a branch.
    /// </summary>
    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout(Guid workspaceId, [FromBody] CheckoutRequest request)
    {
        var workspace = await _context.Workspaces.FindAsync(workspaceId);
        if (workspace == null) return NotFound();

        var result = await _gitService.CheckoutAsync(workspace.RepositoryPath, request.Branch, default);
        if (!result.Success) return BadRequest(new { error = result.Error });

        return Ok(new { output = result.Output });
    }
}

public class CheckoutRequest
{
    public string Branch { get; set; } = string.Empty;
}
