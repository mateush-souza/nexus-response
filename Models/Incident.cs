using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nexus_response.Models
{
    public class Incident
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        [Required]
        [StringLength(50)]
        public string StatusReport { get; set; }

        [Required]
        [StringLength(20)]
        public string Source { get; set; }

        [StringLength(20)]
        public string UrgencyLevel { get; set; }

        public int UrgencyScore { get; set; }

        [StringLength(500)]
        public string UrgencyFactors { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; }

        public DateTime Timestamp { get; set; }

        public int ReportedById { get; set; }

        [ForeignKey("ReportedById")]
        public virtual User ReportedBy { get; set; }

        public virtual ICollection<IncidentComment> Comments { get; set; }

        public virtual ICollection<IoTData> IoTData { get; set; }
    }
}

