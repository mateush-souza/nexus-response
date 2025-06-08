using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using nexus_response.Models;

namespace nexus_response.Mappings
{
    public class DeviceMap : IEntityTypeConfiguration<Device>
    {
        public void Configure(EntityTypeBuilder<Device> builder)
        {
            builder.ToTable("Devices");

            builder.HasKey(d => d.Id);
            builder.Property(d => d.Id).ValueGeneratedOnAdd();

            builder.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(d => d.Type)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(d => d.Location)
                .HasMaxLength(200);

            builder.Property(d => d.Status)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(d => d.LastCommunication);

            builder.Property(d => d.CreatedAt)
                .IsRequired();

            builder.HasMany(d => d.IoTData)
                .WithOne(data => data.Device)
                .HasForeignKey(data => data.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

