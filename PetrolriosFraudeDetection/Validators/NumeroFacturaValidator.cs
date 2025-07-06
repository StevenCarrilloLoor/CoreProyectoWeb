using System.Text.RegularExpressions;
using PetrolriosFraudeDetection.Data;

namespace PetrolriosFraudeDetection.Validators
{
    public class NumeroFacturaValidator
    {
        private readonly ApplicationDbContext _context;
        
        public NumeroFacturaValidator(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public bool ValidarNumeroFactura(string numeroFactura, int? facturaId = null)
        {
            // Validar formato (ejemplo: 001-001-000000001)
            if (!Regex.IsMatch(numeroFactura, @"^\d{3}-\d{3}-\d{9}$"))
                return false;
                
            // Validar unicidad
            var existeNumero = facturaId.HasValue 
                ? _context.Facturas.Any(f => f.NumeroFactura == numeroFactura && f.Id != facturaId.Value)
                : _context.Facturas.Any(f => f.NumeroFactura == numeroFactura);
                
            return !existeNumero;
        }
    }
}