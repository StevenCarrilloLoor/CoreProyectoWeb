using System.ComponentModel.DataAnnotations;

namespace PetrolriosFraudeDetection.Models.Entities
{
    public class Ubicacion
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Ciudad { get; set; }

        [Required]
        [StringLength(100)]
        public string Provincia { get; set; }
    }
}