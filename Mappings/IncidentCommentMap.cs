using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using nexus_response.Models;

namespace nexus_response.Mappings
{
    public class IncidentCommentMap : IEntityTypeConfiguration<IncidentComment>
    {
        public void Configure(EntityTypeBuilder<IncidentComment> builder)
        {
            builder.ToTable("IncidentComments");

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();

            builder.Property(c => c.Content)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(c => c.UserId)
                .IsRequired();

            builder.Property(c => c.UserName)
                .HasMaxLength(100);

            builder.Property(c => c.IncidentId)
                .IsRequired();

            builder.Property(c => c.Timestamp)
                .IsRequired();

            builder.HasOne(c => c.Incident)
                .WithMany(i => i.Comments)
                .HasForeignKey(c => c.IncidentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

