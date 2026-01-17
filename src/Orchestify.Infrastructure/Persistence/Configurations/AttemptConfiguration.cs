using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orchestify.Domain.Entities;

namespace Orchestify.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for AttemptEntity.
/// </summary>
public class AttemptConfiguration : IEntityTypeConfiguration<AttemptEntity>
{
    /// <summary>
    /// Configures the AttemptEntity.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<AttemptEntity> builder)
    {
        builder.ToTable("Attempts");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.State)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(a => a.ErrorMessage)
            .HasMaxLength(5000);

        builder.Property(a => a.CancellationReason)
            .HasMaxLength(1000);

        builder.Property(a => a.CorrelationId)
            .HasMaxLength(100);

        builder.HasIndex(a => a.TaskId);
        builder.HasIndex(a => a.State);
        builder.HasIndex(a => new { a.State, a.QueuedAt }); // For queue dequeue

        builder.HasOne(a => a.Task)
            .WithMany()
            .HasForeignKey(a => a.TaskId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
