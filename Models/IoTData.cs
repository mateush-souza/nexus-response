using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nexus_response.Models
{
    public class IoTData
    {
        [Key]
        public int Id { get; set; }

        public int DeviceId { get; set; }

        [Required]
        [StringLength(30)]
        public string Type { get; set; }

        [Required]
        public string RawData { get; set; }

        public double? Temperature { get; set; }

        public double? Humidity { get; set; }

        public double? Distance { get; set; }

        public double? AccelerometerX { get; set; }

        public double? AccelerometerY { get; set; }

        public double? AccelerometerZ { get; set; }

        public DateTime Timestamp { get; set; }

        public int? IncidentId { get; set; }

        [ForeignKey("IncidentId")]
        public virtual Incident Incident { get; set; }

        [ForeignKey("DeviceId")]
        public virtual Device Device { get; set; }
    }
}

