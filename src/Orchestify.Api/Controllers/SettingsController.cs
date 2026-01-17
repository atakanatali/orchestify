using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestify.Application.Common.Interfaces;
using Orchestify.Domain.Entities;

namespace Orchestify.Api.Controllers;

/// <summary>
/// Settings management endpoints.
/// </summary>
[ApiController]
[Route("api/settings")]
public class SettingsController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public SettingsController(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets all settings, optionally filtered by category.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetSettings([FromQuery] string? category = null)
    {
        var query = _context.Settings.AsNoTracking();
        if (!string.IsNullOrEmpty(category))
            query = query.Where(s => s.Category == category);

        var settings = await query
            .Select(s => new SettingDto
            {
                Key = s.Key,
                Value = s.IsSecret ? "***" : s.Value,
                Category = s.Category,
                Description = s.Description,
                IsSecret = s.IsSecret,
                UpdatedAt = s.UpdatedAt
            })
            .ToListAsync();

        return Ok(new { Items = settings });
    }

    /// <summary>
    /// Gets a setting by key.
    /// </summary>
    [HttpGet("{key}")]
    public async Task<IActionResult> GetSetting(string key)
    {
        var setting = await _context.Settings.AsNoTracking().FirstOrDefaultAsync(s => s.Key == key);
        if (setting == null) return NotFound();

        return Ok(new SettingDto
        {
            Key = setting.Key,
            Value = setting.IsSecret ? "***" : setting.Value,
            Category = setting.Category,
            Description = setting.Description,
            IsSecret = setting.IsSecret,
            UpdatedAt = setting.UpdatedAt
        });
    }

    /// <summary>
    /// Creates or updates a setting.
    /// </summary>
    [HttpPut("{key}")]
    public async Task<IActionResult> UpsertSetting(string key, [FromBody] UpdateSettingDto request)
    {
        var setting = await _context.Settings.FirstOrDefaultAsync(s => s.Key == key);

        if (setting == null)
        {
            setting = new SettingEntity
            {
                Key = key,
                Value = request.Value,
                Category = request.Category ?? "General",
                Description = request.Description,
                IsSecret = request.IsSecret,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Settings.Add(setting);
        }
        else
        {
            setting.Value = request.Value;
            if (request.Category != null) setting.Category = request.Category;
            if (request.Description != null) setting.Description = request.Description;
            setting.IsSecret = request.IsSecret;
            setting.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(default);
        return Ok();
    }

    /// <summary>
    /// Deletes a setting.
    /// </summary>
    [HttpDelete("{key}")]
    public async Task<IActionResult> DeleteSetting(string key)
    {
        var setting = await _context.Settings.FirstOrDefaultAsync(s => s.Key == key);
        if (setting == null) return NotFound();

        _context.Settings.Remove(setting);
        await _context.SaveChangesAsync(default);
        return NoContent();
    }
}

public class SettingDto
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsSecret { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class UpdateSettingDto
{
    public string Value { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? Description { get; set; }
    public bool IsSecret { get; set; }
}
