using System.ComponentModel.DataAnnotations;

namespace IncidentAPI.Models
{
    public class DTO
    {
        [Required]
        public string AccountName { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
