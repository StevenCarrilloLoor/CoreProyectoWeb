using Microsoft.EntityFrameworkCore;
using PetrolriosFraudeDetection.Data;
using PetrolriosFraudeDetection.Factories;
using PetrolriosFraudeDetection.Interfaces;
using PetrolriosFraudeDetection.Models.Entities;

namespace PetrolriosFraudeDetection.Services.Estrategias
{
    public class EstrategiaDeteccionCombustible : IEstrategiaDeteccion
    {
        private readonly ApplicationDbContext _context;
        private readonly AlertaDesaparicionCombustibleFactory _factory;
        
        public string TipoDeteccion => "Desaparición de Combustible";
        
        public EstrategiaDeteccionCombustible(ApplicationDbContext context)
        {
            _context = context;
            _factory = new AlertaDesaparicionCombustibleFactory();
        }
        
        public async Task<List<AlertaFraude>> Detectar(DateTime fecha)
        {
            var alertas = new List<AlertaFraude>();
            
            // Obtener inventarios del día
            var inventarios = await _context.Inventarios
                .Include(i => i.Estacion)
                .Where(i => i.Fecha.Date == fecha.Date)
                .ToListAsync();
                
            foreach (var inventario in inventarios)
            {
                // Calcular la pérdida teórica
                var perdidaTeorica = inventario.NivelInicial + inventario.CantidadRecibida - inventario.NivelFinal;
                
                // Obtener las ventas de la estación en esa fecha
                var ventasEstacion = await _context.Ventas
                    .Where(v => v.EstacionId == inventario.EstacionId && v.Fecha.Date == fecha.Date)
                    .ToListAsync();
                    
                var litrosVendidos = ventasEstacion.Sum(v => v.LitrosVendidos);
                
                // Si la pérdida teórica es mayor que los litros vendidos + un margen de error (1%)
                var margenError = litrosVendidos * 0.01m;
                
                if (perdidaTeorica > (litrosVendidos + margenError) && perdidaTeorica > 100) // Solo alertar si la diferencia es significativa
                {
                    var ventaMasReciente = ventasEstacion.OrderByDescending(v => v.Fecha).FirstOrDefault();
                    var descripcion = $"Pérdida no justificada de {perdidaTeorica - litrosVendidos:N2} litros en la estación {inventario.Estacion.Nombre}.";
                    
                    var alerta = _factory.CrearAlertaCompleta(descripcion, ventaMasReciente?.Id);
                    alertas.Add(alerta);
                }
            }
            
            return alertas;
        }
    }
}