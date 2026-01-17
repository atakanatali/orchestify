using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orchestify.Domain.Entities;

namespace Orchestify.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for RunStepEntity.
/// </summary>
public class RunStepConfiguration : IEntityTypeConfiguration<RunStepEntity>
{
    /// <summary>
    /// Configures the RunStepEntity.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<RunStepEntity> builder)
    {
        builder.ToTable("RunSteps");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.StepType)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(s => s.State)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(s => s.ErrorMessage)
            .HasMaxLength(5000);

        builder.Property(s => s.Output)
            .HasColumnType("text");

        builder.HasIndex(s => s.AttemptId);
        builder.HasIndex(s => new { s.AttemptId, s.SequenceNumber });

        builder.HasOne(s => s.Attempt)
            .WithMany()
            .HasForeignKey(s => s.AttemptId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
