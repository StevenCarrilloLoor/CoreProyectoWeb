using System.ComponentModel.DataAnnotations;

namespace PetrolriosFraudeDetection.Models.Entities
{
    public class Venta
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "La fecha es obligatoria")]
        [Display(Name = "Fecha de Venta")]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }
        
        [Required(ErrorMessage = "Los litros vendidos son obligatorios")]
        [Display(Name = "Litros Vendidos")]
        [Range(0.1, double.MaxValue, ErrorMessage = "Los litros vendidos deben ser mayores a 0")]
        public decimal LitrosVendidos { get; set; }
        
        [Required(ErrorMessage = "El monto total es obligatorio")]
        [Display(Name = "Monto Total")]
        [Range(0.1, double.MaxValue, ErrorMessage = "El monto total debe ser mayor a 0")]
        public decimal MontoTotal { get; set; }
        
        [Required(ErrorMessage = "El número de transacción es obligatorio")]
        [Display(Name = "Número de Transacción")]
        [StringLength(20, ErrorMessage = "El número de transacción no puede tener más de 20 caracteres")]
        public string NumeroTransaccion { get; set; }
        
        // Relaciones
        [Required(ErrorMessage = "La estación es obligatoria")]
        [Display(Name = "Estación")]
        public int EstacionId { get; set; }
        public virtual Estacion Estacion { get; set; }
        
        public virtual ICollection<Factura> Facturas { get; set; }
        public virtual ICollection<AlertaFraude> Alertas { get; set; }
    }
}