using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orchestify.Domain.Entities;

namespace Orchestify.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for WorkspaceEntity.
/// </summary>
public class WorkspaceConfiguration : IEntityTypeConfiguration<WorkspaceEntity>
{
    public void Configure(EntityTypeBuilder<WorkspaceEntity> builder)
    {
        builder.HasKey(w => w.Id);

        builder.HasMany(w => w.Boards)
            .WithOne(b => b.Workspace)
            .HasForeignKey(b => b.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
