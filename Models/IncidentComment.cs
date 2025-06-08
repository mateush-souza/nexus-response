using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nexus_response.Models
{
    public class IncidentComment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Content { get; set; }

        public int UserId { get; set; }

        [StringLength(100)]
        public string UserName { get; set; }

        public int IncidentId { get; set; }

        [ForeignKey("IncidentId")]
        public virtual Incident Incident { get; set; }

        public DateTime Timestamp { get; set; }
    }
}

