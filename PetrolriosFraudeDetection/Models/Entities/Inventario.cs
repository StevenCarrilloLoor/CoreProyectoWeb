using System.ComponentModel.DataAnnotations;

namespace PetrolriosFraudeDetection.Models.Entities
{
    public class Inventario
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "La fecha es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }
        
        [Required(ErrorMessage = "El nivel inicial es obligatorio")]
        [Display(Name = "Nivel Inicial (L)")]
        [Range(0, double.MaxValue, ErrorMessage = "El nivel inicial no puede ser negativo")]
        public decimal NivelInicial { get; set; }
        
        [Required(ErrorMessage = "El nivel final es obligatorio")]
        [Display(Name = "Nivel Final (L)")]
        [Range(0, double.MaxValue, ErrorMessage = "El nivel final no puede ser negativo")]
        public decimal NivelFinal { get; set; }
        
        [Display(Name = "Cantidad Recibida (L)")]
        [Range(0, double.MaxValue, ErrorMessage = "La cantidad recibida no puede ser negativa")]
        public decimal CantidadRecibida { get; set; }
        
        // Relaciones
        [Required(ErrorMessage = "La estación es obligatoria")]
        [Display(Name = "Estación")]
        public int EstacionId { get; set; }
        public virtual Estacion Estacion { get; set; }
    }
}