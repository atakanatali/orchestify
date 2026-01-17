using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orchestify.Domain.Entities;

namespace Orchestify.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF configuration for SettingEntity.
/// </summary>
public class SettingConfiguration : IEntityTypeConfiguration<SettingEntity>
{
    public void Configure(EntityTypeBuilder<SettingEntity> builder)
    {
        builder.ToTable("Settings");
        builder.HasKey(s => s.Key);
        builder.Property(s => s.Key).HasMaxLength(200);
        builder.Property(s => s.Value).HasColumnType("text");
        builder.Property(s => s.Category).HasMaxLength(100);
        builder.Property(s => s.Description).HasMaxLength(500);
        builder.HasIndex(s => s.Category);
    }
}
