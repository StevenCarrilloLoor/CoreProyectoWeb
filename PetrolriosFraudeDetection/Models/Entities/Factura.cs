using System.ComponentModel.DataAnnotations;

namespace PetrolriosFraudeDetection.Models.Entities
{
    public class Factura
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El número de factura es obligatorio")]
        [Display(Name = "Número de Factura")]
        [StringLength(15, ErrorMessage = "El número de factura no puede tener más de 15 caracteres")]
        [RegularExpression(@"^\d{3}-\d{3}-\d{9}$", ErrorMessage = "El número de factura debe seguir el formato 000-000-000000000")]
        public string NumeroFactura { get; set; }
        
        [Required(ErrorMessage = "La fecha de emisión es obligatoria")]
        [Display(Name = "Fecha de Emisión")]
        [DataType(DataType.Date)]
        public DateTime FechaEmision { get; set; }
        
        [Required(ErrorMessage = "El monto es obligatorio")]
        [Range(0.1, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        public decimal Monto { get; set; }
        
        public bool Anulada { get; set; }
        
        // Relaciones
        [Required(ErrorMessage = "La venta es obligatoria")]
        [Display(Name = "Venta")]
        public int VentaId { get; set; }
        public virtual Venta Venta { get; set; }
    }
}