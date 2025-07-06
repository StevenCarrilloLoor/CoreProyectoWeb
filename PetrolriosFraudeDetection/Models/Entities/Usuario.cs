using System.ComponentModel.DataAnnotations;

namespace PetrolriosFraudeDetection.Models.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres")]
        public string Nombre { get; set; }
        
        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(50, ErrorMessage = "El apellido no puede tener más de 50 caracteres")]
        public string Apellido { get; set; }
        
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El email no es válido")]
        [StringLength(100, ErrorMessage = "El email no puede tener más de 100 caracteres")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(100, ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string PasswordHash { get; set; }
        
        [Required(ErrorMessage = "El rol es obligatorio")]
        [StringLength(20, ErrorMessage = "El rol no puede tener más de 20 caracteres")]
        public string Rol { get; set; } // "Administrador", "Analista"
        
        // Relaciones
        public virtual ICollection<AlertaFraude> AlertasRevisadas { get; set; }
    }
}