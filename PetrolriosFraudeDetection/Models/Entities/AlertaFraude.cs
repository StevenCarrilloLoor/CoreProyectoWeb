using System.ComponentModel.DataAnnotations;

namespace PetrolriosFraudeDetection.Models.Entities
{
    public class AlertaFraude
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El tipo de alerta es obligatorio")]
        [StringLength(50, ErrorMessage = "El tipo no puede tener más de 50 caracteres")]
        public string Tipo { get; set; }
        
        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(500, ErrorMessage = "La descripción no puede tener más de 500 caracteres")]
        public string Descripcion { get; set; }
        
        [Required(ErrorMessage = "El estado es obligatorio")]
        [StringLength(20, ErrorMessage = "El estado no puede tener más de 20 caracteres")]
        public string Estado { get; set; } // "Pendiente", "Confirmado", "Falso Positivo"
        
        [Required(ErrorMessage = "La fecha de detección es obligatoria")]
        [Display(Name = "Fecha de Detección")]
        [DataType(DataType.DateTime)]
        public DateTime FechaDeteccion { get; set; }
        
        [Display(Name = "Fecha de Resolución")]
        [DataType(DataType.DateTime)]
        public DateTime? FechaResolucion { get; set; }
        
        // Relaciones
        [Display(Name = "Venta")]
        public int? VentaId { get; set; }
        public virtual Venta Venta { get; set; }
        
        [Display(Name = "Usuario")]
        public int? UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}