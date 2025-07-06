using System.ComponentModel.DataAnnotations;

namespace PetrolriosFraudeDetection.Models.Entities
{
    public class Estacion
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres")]
        public string Nombre { get; set; }
        
        [Required(ErrorMessage = "La ubicación es obligatoria")]
        [StringLength(200, ErrorMessage = "La ubicación no puede tener más de 200 caracteres")]
        public string Ubicacion { get; set; }
        
        [Required(ErrorMessage = "El código es obligatorio")]
        [StringLength(10, ErrorMessage = "El código no puede tener más de 10 caracteres")]
        [RegularExpression(@"^EST-\d{6}$", ErrorMessage = "El código debe seguir el formato EST-XXXXXX donde X son dígitos")]
        public string Codigo { get; set; }
        
        public bool Activo { get; set; }
        
        // Relaciones
        public virtual ICollection<Venta> Ventas { get; set; }
        public virtual ICollection<SensorVolumen> Sensores { get; set; }
        public virtual ICollection<Inventario> Inventarios { get; set; }
    }
}