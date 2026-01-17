using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orchestify.Domain.Entities;

namespace Orchestify.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for BoardEntity.
/// </summary>
public class BoardConfiguration : IEntityTypeConfiguration<BoardEntity>
{
    /// <summary>
    /// Configures the BoardEntity.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<BoardEntity> builder)
    {
        builder.ToTable("Boards");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.Description)
            .HasMaxLength(1000);

        builder.HasIndex(b => new { b.WorkspaceId, b.Name })
            .IsUnique();

        builder.HasOne(b => b.Workspace)
            .WithMany()
            .HasForeignKey(b => b.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
