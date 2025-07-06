using Microsoft.EntityFrameworkCore;
using PetrolriosFraudeDetection.Data;
using PetrolriosFraudeDetection.Factories;
using PetrolriosFraudeDetection.Interfaces;
using PetrolriosFraudeDetection.Models.Entities;

namespace PetrolriosFraudeDetection.Services.Estrategias
{
    public class EstrategiaDeteccionPrecios : IEstrategiaDeteccion
    {
        private readonly ApplicationDbContext _context;
        private readonly AlertaDiscrepanciaPreciosFactory _factory;
        private readonly decimal _precioOficialPorLitro = 1.50m;
        private readonly decimal _toleranciaPorcentaje = 3.0m;
        
        public string TipoDeteccion => "Discrepancia de Precios";
        
        public EstrategiaDeteccionPrecios(ApplicationDbContext context)
        {
            _context = context;
            _factory = new AlertaDiscrepanciaPreciosFactory();
        }
        
        public async Task<List<AlertaFraude>> Detectar(DateTime fecha)
        {
            var alertas = new List<AlertaFraude>();
            
            // Obtener ventas del día
            var ventas = await _context.Ventas
                .Include(v => v.Estacion)
                .Where(v => v.Fecha.Date == fecha.Date)
                .ToListAsync();
                
            foreach (var venta in ventas)
            {
                if (venta.LitrosVendidos > 0) // Evitar división por cero
                {
                    // Calcular precio por litro de esta venta
                    decimal precioPorLitroCalculado = venta.MontoTotal / venta.LitrosVendidos;
                    
                    // Calcular diferencia porcentual con el precio oficial
                    decimal diferenciaPorcentual = Math.Abs((precioPorLitroCalculado - _precioOficialPorLitro) / _precioOficialPorLitro * 100);
                    
                    // Si la diferencia excede la tolerancia, generar alerta
                    if (diferenciaPorcentual > _toleranciaPorcentaje)
                    {
                        var descripcion = $"Precio por litro calculado ({precioPorLitroCalculado:C2}) difiere en {diferenciaPorcentual:N2}% del precio oficial ({_precioOficialPorLitro:C2}) en la venta {venta.NumeroTransaccion} de {venta.Estacion.Nombre}.";
                        
                        var alerta = _factory.CrearAlertaCompleta(descripcion, venta.Id);
                        alertas.Add(alerta);
                    }
                }
            }
            
            return alertas;
        }
    }
}