using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IncidentAPI.Models
{
    public class Incident
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string IncidentName { get; set; }

        [Required]
        [StringLength(100)]
        public string Description { get; set; }

        public virtual Account Account { get; set; }
    }
}
