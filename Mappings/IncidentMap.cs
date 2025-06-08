using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using nexus_response.Models;

namespace nexus_response.Mappings
{
    public class IncidentMap : IEntityTypeConfiguration<Incident>
    {
        public void Configure(EntityTypeBuilder<Incident> builder)
        {
            builder.ToTable("Incidents");

            builder.HasKey(i => i.Id);
            builder.Property(i => i.Id).ValueGeneratedOnAdd();

            builder.Property(i => i.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(i => i.Latitude)
                .IsRequired();

            builder.Property(i => i.Longitude)
                .IsRequired();

            builder.Property(i => i.StatusReport)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(i => i.Source)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(i => i.UrgencyLevel)
                .HasMaxLength(20);

            builder.Property(i => i.UrgencyScore);

            builder.Property(i => i.UrgencyFactors)
                .HasMaxLength(500);

            builder.Property(i => i.Status)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(i => i.Timestamp)
                .IsRequired();

            builder.HasMany(i => i.Comments)
                .WithOne(c => c.Incident)
                .HasForeignKey(c => c.IncidentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(i => i.IoTData)
                .WithOne(d => d.Incident)
                .HasForeignKey(d => d.IncidentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

