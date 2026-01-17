using Microsoft.AspNetCore.Mvc;
using Orchestify.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Orchestify.Api.Controllers;

[ApiController]
[Route("api/discovery")]
public class WorkspaceDiscoveryController : ControllerBase
{
    private readonly IGitService _gitService;
    private readonly string _reposRoot;

    public WorkspaceDiscoveryController(IGitService gitService, IConfiguration configuration)
    {
        _gitService = gitService;
        // In Docker, we use a fixed volume at /repos. 
        // Fallback to local 'repos' for development outside Docker.
        _reposRoot = configuration["Workspace:ReposRoot"] ?? "repos";
        
        if (!Path.IsPathRooted(_reposRoot))
        {
             _reposRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../", _reposRoot));
        }

        if (!Directory.Exists(_reposRoot))
        {
            Directory.CreateDirectory(_reposRoot);
        }
    }

    [HttpGet("repos")]
    public IActionResult ListRepos()
    {
        if (!Directory.Exists(_reposRoot)) return Ok(new string[] { });

        var repos = Directory.GetDirectories(_reposRoot)
            .Select(path => new RepositoryInfo
            {
                Name = Path.GetFileName(path)!,
                FullPath = path
            })
            .Where(r => !r.Name.StartsWith("."))
            .ToList();

        return Ok(repos);
    }

    [HttpPost("repos")]
    public async Task<IActionResult> CreateRepo([FromBody] CreateRepoRequest request)
    {
        if (string.IsNullOrEmpty(request.Name)) return BadRequest("Repo name is required");

        var repoPath = Path.Combine(_reposRoot, request.Name);
        if (Directory.Exists(repoPath)) return BadRequest("Repository already exists");

        var result = await _gitService.InitAsync(repoPath, default);
        if (!result.Success) return BadRequest(new { error = result.Error });

        return Ok(new { name = request.Name, path = repoPath });
    }

    public class CreateRepoRequest
    {
        public string Name { get; set; } = string.Empty;
    }

    public class RepositoryInfo
    {
        public string Name { get; set; } = string.Empty;
        public string FullPath { get; set; } = string.Empty;
    }

    [HttpGet("branches")]
    public async Task<IActionResult> ListBranches([FromQuery] string repoName)
    {
        if (string.IsNullOrEmpty(repoName)) return BadRequest("Repo name is required");

        var repoPath = Path.Combine(_reposRoot, repoName);
        if (!Directory.Exists(repoPath)) return NotFound("Repository not found");

        var result = await _gitService.ListBranchesAsync(repoPath, default);
        if (!result.Success) return BadRequest(new { error = result.Error });

        var branches = result.Output.Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(b => b.Trim())
            .ToList();

        return Ok(branches);
    }
}
