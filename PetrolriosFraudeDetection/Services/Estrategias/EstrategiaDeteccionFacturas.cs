using Microsoft.EntityFrameworkCore;
using PetrolriosFraudeDetection.Data;
using PetrolriosFraudeDetection.Factories;
using PetrolriosFraudeDetection.Interfaces;
using PetrolriosFraudeDetection.Models.Entities;

namespace PetrolriosFraudeDetection.Services.Estrategias
{
    public class EstrategiaDeteccionFacturas : IEstrategiaDeteccion
    {
        private readonly ApplicationDbContext _context;
        private readonly AlertaFacturacionDuplicadaFactory _factory;
        
        public string TipoDeteccion => "Facturación Duplicada";
        
        public EstrategiaDeteccionFacturas(ApplicationDbContext context)
        {
            _context = context;
            _factory = new AlertaFacturacionDuplicadaFactory();
        }
        
        public async Task<List<AlertaFraude>> Detectar(DateTime fecha)
        {
            var alertas = new List<AlertaFraude>();
            
            // Obtener facturas del día
            var facturas = await _context.Facturas
                .Include(f => f.Venta)
                .Where(f => f.FechaEmision.Date == fecha.Date && !f.Anulada)
                .ToListAsync();
            
            // Agrupar por monto para detectar montos repetidos inusuales
            var gruposPorMonto = facturas
                .GroupBy(f => f.Monto)
                .Where(g => g.Count() > 3) // Si hay más de 3 facturas con el mismo monto exacto
                .ToList();
                
            foreach (var grupo in gruposPorMonto)
            {
                var ventas = grupo.Select(f => f.VentaId).Distinct().ToList();
                
                if (ventas.Count > 1) // Si son de diferentes ventas
                {
                    var descripcion = $"Se detectaron {grupo.Count()} facturas con el mismo monto ({grupo.Key:C2}) en diferentes ventas.";
                    var alerta = _factory.CrearAlertaCompleta(descripcion, ventas.First());
                    alertas.Add(alerta);
                }
            }
            
            return alertas;
        }
    }
}