using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using nexus_response.Models;

namespace nexus_response.Mappings
{
    public class IoTDataMap : IEntityTypeConfiguration<IoTData>
    {
        public void Configure(EntityTypeBuilder<IoTData> builder)
        {
            builder.ToTable("IoTData");

            builder.HasKey(d => d.Id);
            builder.Property(d => d.Id).ValueGeneratedOnAdd();

            builder.Property(d => d.DeviceId)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(d => d.Type)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(d => d.RawData)
                .IsRequired()
                .HasColumnType("NCLOB"); 

            builder.Property(d => d.Temperature);
            builder.Property(d => d.Humidity);
            builder.Property(d => d.Distance);
            builder.Property(d => d.AccelerometerX);
            builder.Property(d => d.AccelerometerY);
            builder.Property(d => d.AccelerometerZ);

            builder.Property(d => d.Timestamp)
                .IsRequired();

            builder.HasOne(d => d.Incident)
                .WithMany(i => i.IoTData)
                .HasForeignKey(d => d.IncidentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(d => d.Device)
                .WithMany(dev => dev.IoTData)
                .HasForeignKey(d => d.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

