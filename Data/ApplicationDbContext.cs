using Microsoft.EntityFrameworkCore;
using nexus_response.Models;
using nexus_response.Mappings;

namespace nexus_response.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Incident> Incidents { get; set; }

        public DbSet<IoTData> IoTData { get; set; }

        public DbSet<IncidentComment> IncidentComments { get; set; }

        public DbSet<Device> Devices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserMap());
            modelBuilder.ApplyConfiguration(new IncidentMap());
            modelBuilder.ApplyConfiguration(new IoTDataMap());
            modelBuilder.ApplyConfiguration(new IncidentCommentMap());
            modelBuilder.ApplyConfiguration(new DeviceMap());
        }
    }
}

