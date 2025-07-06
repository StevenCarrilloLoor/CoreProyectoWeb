using Microsoft.EntityFrameworkCore;
using PetrolriosFraudeDetection.Data;
using PetrolriosFraudeDetection.Models.Entities;

namespace PetrolriosFraudeDetection.Models.Services
{
    public class MotorDeteccion
    {
        private readonly ApplicationDbContext _context;
        
        public MotorDeteccion(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<List<AlertaFraude>> AnalizarPatrones(DateTime fecha)
        {
            var alertas = new List<AlertaFraude>();
            
            // Detección de facturas duplicadas
            var facturasDuplicadas = await DetectarFacturasDuplicadas(fecha);
            alertas.AddRange(facturasDuplicadas);
            
            // Detección de desaparición de combustible
            var perdidaCombustible = await DetectarPerdidaCombustible(fecha);
            alertas.AddRange(perdidaCombustible);
            
            // Detección de anomalías entre ventas y movimientos físicos
            var anomaliasVentasMovimientos = await DetectarAnomaliasVentasMovimientos(fecha);
            alertas.AddRange(anomaliasVentasMovimientos);
            
            // Detección de discrepancias en precios
            var discrepanciasPrecios = await DetectarDiscrepanciaPrecios(fecha);
            alertas.AddRange(discrepanciasPrecios);
            
            return alertas;
        }
        
        private async Task<List<AlertaFraude>> DetectarFacturasDuplicadas(DateTime fecha)
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
                    alertas.Add(new AlertaFraude
                    {
                        Tipo = "Facturación Duplicada",
                        Descripcion = $"Se detectaron {grupo.Count()} facturas con el mismo monto ({grupo.Key:C2}) en diferentes ventas.",
                        Estado = "Pendiente",
                        FechaDeteccion = DateTime.Now,
                        VentaId = ventas.First() // Asociamos con la primera venta
                    });
                }
            }
            
            return alertas;
        }
        
        private async Task<List<AlertaFraude>> DetectarPerdidaCombustible(DateTime fecha)
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
                    
                    alertas.Add(new AlertaFraude
                    {
                        Tipo = "Desaparición de Combustible",
                        Descripcion = $"Pérdida no justificada de {perdidaTeorica - litrosVendidos:N2} litros en la estación {inventario.Estacion.Nombre}.",
                        Estado = "Pendiente",
                        FechaDeteccion = DateTime.Now,
                        VentaId = ventaMasReciente?.Id
                    });
                }
            }
            
            return alertas;
        }
        
        private async Task<List<AlertaFraude>> DetectarAnomaliasVentasMovimientos(DateTime fecha)
        {
            var alertas = new List<AlertaFraude>();
            
            // Obtener lecturas de sensores del día
            var lecturasSensores = await _context.SensoresVolumen
                .Include(s => s.Estacion)
                .Where(s => s.Timestamp.Date == fecha.Date)
                .ToListAsync();
                
            // Agrupar por estación
            var lecturasPorEstacion = lecturasSensores
                .GroupBy(s => s.EstacionId)
                .ToList();
                
            foreach (var grupo in lecturasPorEstacion)
            {
                var estacionId = grupo.Key;
                var estacion = grupo.First().Estacion;
                
                // Calcular la diferencia neta en volumen según los sensores
                var diferenciaVolumen = grupo.Sum(s => s.NivelInicial - s.NivelFinal);
                
                // Obtener las ventas de la estación en esa fecha
                var ventasEstacion = await _context.Ventas
                    .Where(v => v.EstacionId == estacionId && v.Fecha.Date == fecha.Date)
                    .ToListAsync();
                    
                var litrosVendidos = ventasEstacion.Sum(v => v.LitrosVendidos);
                
                // Si hay una discrepancia significativa (más del 5%)
                var discrepancia = Math.Abs(diferenciaVolumen - litrosVendidos);
                var porcentajeDiscrepancia = (litrosVendidos > 0) ? (discrepancia / litrosVendidos) * 100 : 0;
                
                if (porcentajeDiscrepancia > 5 && discrepancia > 50) // Solo alertar si la diferencia es significativa
                {
                    var ventaMasReciente = ventasEstacion.OrderByDescending(v => v.Fecha).FirstOrDefault();
                    
                    alertas.Add(new AlertaFraude
                    {
                        Tipo = "Anomalía Ventas-Movimientos",
                        Descripcion = $"Discrepancia del {porcentajeDiscrepancia:N2}% entre ventas registradas ({litrosVendidos:N2} L) y movimientos físicos ({diferenciaVolumen:N2} L) en {estacion.Nombre}.",
                        Estado = "Pendiente",
                        FechaDeteccion = DateTime.Now,
                        VentaId = ventaMasReciente?.Id
                    });
                }
            }
            
            return alertas;
        }

        private async Task<List<AlertaFraude>> DetectarDiscrepanciaPrecios(DateTime fecha)
        {
            var alertas = new List<AlertaFraude>();
            
            // Obtener precios oficiales (esto podría venir de una tabla de configuración)
            decimal precioOficialPorLitro = 1.50m; // Precio de referencia, podría variar por tipo de combustible
            decimal toleranciaPorcentaje = 3.0m; // Tolerancia del 3%
            
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
                    decimal diferenciaPorcentual = Math.Abs((precioPorLitroCalculado - precioOficialPorLitro) / precioOficialPorLitro * 100);
                    
                    // Si la diferencia excede la tolerancia, generar alerta
                    if (diferenciaPorcentual > toleranciaPorcentaje)
                    {
                        alertas.Add(new AlertaFraude
                        {
                            Tipo = "Discrepancia de Precios",
                            Descripcion = $"Precio por litro calculado ({precioPorLitroCalculado:C2}) difiere en {diferenciaPorcentual:N2}% del precio oficial ({precioOficialPorLitro:C2}) en la venta {venta.NumeroTransaccion} de {venta.Estacion.Nombre}.",
                            Estado = "Pendiente",
                            FechaDeteccion = DateTime.Now,
                            VentaId = venta.Id
                        });
                    }
                }
            }
            
            return alertas;
        }
    }
}