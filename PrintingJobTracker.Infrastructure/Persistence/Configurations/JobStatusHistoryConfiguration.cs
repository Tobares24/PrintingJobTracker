using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrintingJobTracker.Domain.Entities;

namespace PrintingJobTracker.Infrastructure.Persistence.Configurations
{
    internal sealed class JobStatusHistoryConfiguration : IEntityTypeConfiguration<JobStatusHistory>
    {
        public void Configure(EntityTypeBuilder<JobStatusHistory> builder)
        {
            builder.ToTable($"{nameof(JobStatusHistory)}Table", t => t.HasComment("Stores the historical records of job status changes over time."))
                   .HasIndex(h => h.Id)
                   .IsUnique();

            builder.HasKey(h => h.Id)
                   .HasName("PK_JobStatusHistory");

            builder.Property(h => h.Id)
                   .HasComment("Unique identifier for the job status history record.");

            builder.HasOne(h => h.Job)
                   .WithMany(j => j.StatusHistory)
                   .HasForeignKey(h => h.JobId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .HasConstraintName("FK_JobStatusHistory_Job");

            builder.Property(h => h.JobId)
                   .IsRequired()
                   .HasComment("Foreign key referencing the related Job entity.");

            builder.Property(h => h.Status)
                   .HasConversion<string>()
                   .IsRequired()
                   .HasMaxLength(50)
                   .HasComment("Represents the job status at this point in time (e.g., Received, Printing, Mailed, etc.).");

            builder.Property(h => h.Note)
                   .HasMaxLength(500)
                   .HasComment("Optional note explaining details or exceptions related to the status change.");

            builder.Property(h => h.ChangedAt)
                   .HasComment("UTC timestamp when this status change occurred.");

            builder.HasIndex(h => new { h.JobId, h.Status, h.ChangedAt })
                   .HasDatabaseName("IX_JobStatusHistory_JobId_Status_ChangedAt");
        }
    }
}
