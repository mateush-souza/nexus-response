using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nexus_response.Models
{
    public class Device
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Type { get; set; }

        [StringLength(200)]
        public string Location { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public DateTime LastCommunication { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<IoTData> IoTData { get; set; }
    }
}

