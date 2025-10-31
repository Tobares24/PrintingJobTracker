using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrintingJobTracker.Domain.Entities;

namespace PrintingJobTracker.Infrastructure.Persistence.Configurations
{
    internal sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable($"{nameof(AuditLog)}Table", t =>
                t.HasComment("Stores all audit trail entries recording create, update, and delete operations across entities."));

            builder.HasKey(a => a.Id)
                   .HasName("PK_AuditLogs");

            builder.Property(a => a.Id)
                   .HasComment("Unique identifier for the audit record.");

            builder.Property(a => a.EntityName)
                   .IsRequired()
                   .HasMaxLength(150)
                   .HasComment("Name of the entity that was affected (e.g., Job, Client).");

            builder.Property(a => a.EntityId)
                   .IsRequired()
                   .HasMaxLength(100)
                   .HasComment("Identifier of the entity instance that was modified.");

            builder.Property(a => a.ActionType)
                   .IsRequired()
                   .HasMaxLength(50)
                   .HasComment("Type of action performed: Create, Update, or Delete.");

            builder.Property(a => a.NewRecord)
                   .HasColumnType("nvarchar(max)")
                   .HasComment("Serialized JSON of the entity state after the operation.");

            builder.Property(a => a.PreviousRecord)
                   .HasColumnType("nvarchar(max)")
                   .HasComment("Serialized JSON of the entity state before the operation.");

            builder.Property(a => a.OcurredOn)
                   .HasDefaultValueSql("GETUTCDATE()")
                   .HasComment("UTC timestamp indicating when the operation occurred.");

            builder.HasIndex(a => new { a.EntityName, a.EntityId })
                   .HasDatabaseName("IX_AuditLogs_EntityName_EntityId");

            builder.HasIndex(a => a.OcurredOn)
                   .HasDatabaseName("IX_AuditLogs_OcurredOn");
        }
    }
}
