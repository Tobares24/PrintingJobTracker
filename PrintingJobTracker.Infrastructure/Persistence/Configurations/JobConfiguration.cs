using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrintingJobTracker.Domain.Entities;

namespace PrintingJobTracker.Infrastructure.Persistence.Configurations
{
    internal sealed class JobConfiguration : IEntityTypeConfiguration<Job>
    {
        public void Configure(EntityTypeBuilder<Job> builder)
        {
            builder.ToTable($"{nameof(Job)}Table", t => t.HasComment("Stores information about printing jobs and their current status in the production process."))
                   .HasIndex(x => x.Id)
                   .IsUnique();

            builder.HasKey(j => j.Id)
                   .HasName("PK_Jobs");

            builder.Property(j => j.Id)
                   .HasComment("Unique identifier for the job.");

            builder.HasMany(j => j.StatusHistory)
                   .WithOne()
                   .HasForeignKey(h => h.JobId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(j => j.Client)
                   .WithMany(c => c.Jobs)
                   .HasForeignKey(j => j.ClientId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .HasConstraintName("FK_Jobs_Clients");

            builder.Property(j => j.ClientId)
                   .IsRequired()
                   .HasComment("Unique identifier of the client associated with the job.");

            builder.Property(j => j.JobName)
                   .IsRequired()
                   .HasMaxLength(200)
                   .HasComment("Descriptive name of the printing job.");

            builder.Property(j => j.Quantity)
                   .IsRequired()
                   .HasComment("Number of items or copies to be produced for this job.");

            builder.Property(j => j.Carrier)
                   .HasConversion<string>()
                   .IsRequired()
                   .HasMaxLength(50)
                   .HasComment("Name of the carrier handling the delivery (e.g., USPS, UPS, FedEx).");

            builder.Property(j => j.CurrentStatus)
                   .HasConversion<string>()
                   .IsRequired()
                   .HasMaxLength(50)
                   .HasComment("Current processing status of the job (Received, Printing, Inserting, etc.).");

            builder.Property(j => j.CreatedAt)
                   .HasComment("UTC date and time when the job was created.");

            builder.Property(j => j.MailDeadline)
                   .IsRequired()
                   .HasComment("Deadline by which the job must be mailed according to the SLA (Service Level Agreement).");
        }
    }
}
