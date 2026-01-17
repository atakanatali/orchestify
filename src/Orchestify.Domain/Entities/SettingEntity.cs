using System;

namespace Orchestify.Domain.Entities;

/// <summary>
/// System settings entity for configuration storage.
/// </summary>
public class SettingEntity
{
    /// <summary>
    /// Unique key for the setting.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Setting value (JSON or plain text).
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Category for grouping settings.
    /// </summary>
    public string Category { get; set; } = "General";

    /// <summary>
    /// Description of the setting.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Whether the setting is sensitive.
    /// </summary>
    public bool IsSecret { get; set; }

    /// <summary>
    /// Last updated timestamp.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
