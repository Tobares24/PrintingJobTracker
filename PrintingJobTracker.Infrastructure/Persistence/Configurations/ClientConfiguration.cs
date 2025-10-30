using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrintingJobTracker.Domain.Entities;

namespace PrintingJobTracker.Infrastructure.Persistence.Configurations
{
    internal sealed class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.ToTable($"{nameof(Client)}Table", t => t.HasComment("Stores information about clients who request printing jobs."))
                   .HasIndex(c => c.Id)
                   .IsUnique();

            builder.HasKey(c => c.Id)
                   .HasName("PK_Clients");

            builder.Property(c => c.Id)
                   .ValueGeneratedOnAdd()
                   .HasComment("Unique identifier for the client.");

            builder.Property(c => c.IdentityCard)
                   .IsRequired()
                   .HasMaxLength(50)
                   .HasComment("National ID or business identification number for the client.");

            builder.Property(c => c.FirstName)
                   .IsRequired()
                   .HasMaxLength(100)
                   .HasComment("Client's first name or company legal name.");

            builder.Property(c => c.LastName)
                   .HasMaxLength(100)
                   .HasComment("Client's last name (if applicable).");

            builder.Property(c => c.SecondLastName)
                   .HasMaxLength(100)
                   .HasComment("Client's second last name (optional).");

            builder.Property(c => c.IsDeleted)
                   .HasDefaultValue(false)
                   .HasComment("Indicates whether the client record is logically deleted.");

            builder.HasMany(c => c.Jobs)
                   .WithOne(j => j.Client)
                   .HasForeignKey(j => j.ClientId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .HasConstraintName("FK_Clients_Jobs");

            builder.HasIndex(c => c.IdentityCard)
                   .IsUnique()
                   .HasDatabaseName("IX_Clients_IdentityCard");

            builder.HasIndex(c => new { c.FirstName, c.LastName })
                   .HasDatabaseName("IX_Clients_FirstName_LastName");

            builder.HasIndex(c => c.IsDeleted)
                   .HasDatabaseName("IX_Clients_IsDeleted");
        }
    }
}
