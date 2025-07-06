using System.Text.RegularExpressions;
using PetrolriosFraudeDetection.Data;

namespace PetrolriosFraudeDetection.Validators
{
    public class CodigoEstacionValidator
    {
        private readonly ApplicationDbContext _context;
        
        public CodigoEstacionValidator(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public bool ValidarCodigo(string codigo, int? estacionId = null)
        {
            // Validar formato (ejemplo: debe comenzar con 'EST-' seguido de 6 dígitos)
            if (!Regex.IsMatch(codigo, @"^EST-\d{6}$"))
                return false;
                
            // Validar unicidad
            var existeCodigo = estacionId.HasValue 
                ? _context.Estaciones.Any(e => e.Codigo == codigo && e.Id != estacionId.Value)
                : _context.Estaciones.Any(e => e.Codigo == codigo);
                
            return !existeCodigo;
        }
    }
}