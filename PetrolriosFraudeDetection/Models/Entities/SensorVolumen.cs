using System.ComponentModel.DataAnnotations;

namespace PetrolriosFraudeDetection.Models.Entities
{
    public class SensorVolumen
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El código del sensor es obligatorio")]
        [Display(Name = "Código del Sensor")]
        [StringLength(20, ErrorMessage = "El código del sensor no puede tener más de 20 caracteres")]
        public string CodigoSensor { get; set; }
        
        [Required(ErrorMessage = "El nivel inicial es obligatorio")]
        [Display(Name = "Nivel Inicial (L)")]
        [Range(0, double.MaxValue, ErrorMessage = "El nivel inicial no puede ser negativo")]
        public decimal NivelInicial { get; set; }
        
        [Required(ErrorMessage = "El nivel final es obligatorio")]
        [Display(Name = "Nivel Final (L)")]
        [Range(0, double.MaxValue, ErrorMessage = "El nivel final no puede ser negativo")]
        public decimal NivelFinal { get; set; }
        
        [Required(ErrorMessage = "La fecha y hora son obligatorias")]
        [Display(Name = "Fecha y Hora")]
        [DataType(DataType.DateTime)]
        public DateTime Timestamp { get; set; }
        
        // Relaciones
        [Required(ErrorMessage = "La estación es obligatoria")]
        [Display(Name = "Estación")]
        public int EstacionId { get; set; }
        public virtual Estacion Estacion { get; set; }
    }
}