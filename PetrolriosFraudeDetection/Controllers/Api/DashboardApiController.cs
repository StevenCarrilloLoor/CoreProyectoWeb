using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetrolriosFraudeDetection.Data;

namespace PetrolriosFraudeDetection.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/DashboardApi/resumen
        [HttpGet("resumen")]
        public async Task<ActionResult<object>> GetResumen()
        {
            var hoy = DateTime.Today;
            var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);
            var finMes = inicioMes.AddMonths(1).AddDays(-1);

            // Métricas generales
            var totalEstaciones = await _context.Estaciones.CountAsync(e => e.Activo);
            var totalVentasHoy = await _context.Ventas.CountAsync(v => v.Fecha.Date == hoy);
            var totalVentasMes = await _context.Ventas.CountAsync(v => v.Fecha >= inicioMes && v.Fecha <= finMes);
            
            // Métricas de ventas - Buscar en los últimos 30 días si no hay datos del mes actual
            var ventasHoy = await _context.Ventas
                .Where(v => v.Fecha.Date == hoy)
                .SumAsync(v => (decimal?)v.MontoTotal) ?? 0;

            var ventasMes = await _context.Ventas
                .Where(v => v.Fecha >= inicioMes && v.Fecha <= finMes)
                .SumAsync(v => (decimal?)v.MontoTotal) ?? 0;

            // Si no hay ventas del mes actual, buscar en los últimos 30 días
            if (ventasMes == 0)
            {
                var hace30Dias = hoy.AddDays(-30);
                ventasMes = await _context.Ventas
                    .Where(v => v.Fecha >= hace30Dias)
                    .SumAsync(v => (decimal?)v.MontoTotal) ?? 0;
                totalVentasMes = await _context.Ventas.CountAsync(v => v.Fecha >= hace30Dias);
            }

            var litrosHoy = await _context.Ventas
                .Where(v => v.Fecha.Date == hoy)
                .SumAsync(v => (decimal?)v.LitrosVendidos) ?? 0;

            var litrosMes = await _context.Ventas
                .Where(v => v.Fecha >= inicioMes && v.Fecha <= finMes)
                .SumAsync(v => (decimal?)v.LitrosVendidos) ?? 0;

            if (litrosMes == 0)
            {
                var hace30Dias = hoy.AddDays(-30);
                litrosMes = await _context.Ventas
                    .Where(v => v.Fecha >= hace30Dias)
                    .SumAsync(v => (decimal?)v.LitrosVendidos) ?? 0;
            }

            // Alertas pendientes
            var alertasPendientes = await _context.AlertasFraude
                .CountAsync(a => a.Estado == "Pendiente");

            // Total de alertas
            var totalAlertas = await _context.AlertasFraude.CountAsync();

            return Ok(new
            {
                General = new
                {
                    TotalEstaciones = totalEstaciones,
                    AlertasPendientes = alertasPendientes,
                    TotalAlertas = totalAlertas
                },
                VentasHoy = new
                {
                    Cantidad = totalVentasHoy,
                    MontoTotal = ventasHoy,
                    LitrosVendidos = litrosHoy
                },
                VentasMes = new
                {
                    Cantidad = totalVentasMes,
                    MontoTotal = ventasMes,
                    LitrosVendidos = litrosMes
                }
            });
        }

        // GET: api/DashboardApi/ventas-por-dia
        [HttpGet("ventas-por-dia")]
        public async Task<ActionResult<object>> GetVentasPorDia([FromQuery] int dias = 7)
        {
            var fechaInicio = DateTime.Today.AddDays(-dias);

            var ventasPorDia = await _context.Ventas
                .Where(v => v.Fecha >= fechaInicio)
                .GroupBy(v => v.Fecha.Date)
                .Select(g => new
                {
                    Fecha = g.Key,
                    CantidadVentas = g.Count(),
                    MontoTotal = g.Sum(v => v.MontoTotal),
                    LitrosTotal = g.Sum(v => v.LitrosVendidos)
                })
                .OrderBy(v => v.Fecha)
                .ToListAsync();

            // Si no hay datos, buscar en los últimos 60 días
            if (!ventasPorDia.Any())
            {
                fechaInicio = DateTime.Today.AddDays(-60);
                ventasPorDia = await _context.Ventas
                    .Where(v => v.Fecha >= fechaInicio)
                    .GroupBy(v => v.Fecha.Date)
                    .Select(g => new
                    {
                        Fecha = g.Key,
                        CantidadVentas = g.Count(),
                        MontoTotal = g.Sum(v => v.MontoTotal),
                        LitrosTotal = g.Sum(v => v.LitrosVendidos)
                    })
                    .OrderBy(v => v.Fecha)
                    .Take(10) // Tomar las últimas 10 fechas con ventas
                    .ToListAsync();
            }

            return Ok(ventasPorDia);
        }

        // GET: api/DashboardApi/top-estaciones
        [HttpGet("top-estaciones")]
        public async Task<ActionResult<object>> GetTopEstaciones([FromQuery] int cantidad = 5)
        {
            var inicioMes = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            var topEstaciones = await _context.Ventas
                .Where(v => v.Fecha >= inicioMes)
                .GroupBy(v => v.Estacion)
                .Select(g => new
                {
                    EstacionId = g.Key.Id,
                    EstacionNombre = g.Key.Nombre,
                    TotalVentas = g.Count(),
                    MontoTotal = g.Sum(v => v.MontoTotal),
                    LitrosTotal = g.Sum(v => v.LitrosVendidos)
                })
                .OrderByDescending(e => e.MontoTotal)
                .Take(cantidad)
                .ToListAsync();

            // Si no hay datos del mes actual, buscar en los últimos 30 días
            if (!topEstaciones.Any())
            {
                var hace30Dias = DateTime.Today.AddDays(-30);
                topEstaciones = await _context.Ventas
                    .Where(v => v.Fecha >= hace30Dias)
                    .GroupBy(v => v.Estacion)
                    .Select(g => new
                    {
                        EstacionId = g.Key.Id,
                        EstacionNombre = g.Key.Nombre,
                        TotalVentas = g.Count(),
                        MontoTotal = g.Sum(v => v.MontoTotal),
                        LitrosTotal = g.Sum(v => v.LitrosVendidos)
                    })
                    .OrderByDescending(e => e.MontoTotal)
                    .Take(cantidad)
                    .ToListAsync();
            }

            return Ok(topEstaciones);
        }

        // GET: api/DashboardApi/alertas-recientes
        [HttpGet("alertas-recientes")]
        public async Task<ActionResult<object>> GetAlertasRecientes([FromQuery] int cantidad = 10)
        {
            var alertasRecientes = await _context.AlertasFraude
                .Include(a => a.Venta)
                .ThenInclude(v => v.Estacion)
                .OrderByDescending(a => a.FechaDeteccion)
                .Take(cantidad)
                .Select(a => new
                {
                    a.Id,
                    a.Tipo,
                    a.Descripcion,
                    a.Estado,
                    a.FechaDeteccion,
                    Estacion = a.Venta != null && a.Venta.Estacion != null ? a.Venta.Estacion.Nombre : "N/A"
                })
                .ToListAsync();

            return Ok(alertasRecientes);
        }

        // GET: api/DashboardApi/alertas-por-estacion
        [HttpGet("alertas-por-estacion")]
        public async Task<ActionResult<object>> GetAlertasPorEstacion()
        {
            var alertasPorEstacion = await _context.AlertasFraude
                .Where(a => a.Venta != null && a.Venta.Estacion != null)
                .GroupBy(a => a.Venta.Estacion.Nombre)
                .Select(g => new
                {
                    Estacion = g.Key,
                    TotalAlertas = g.Count(),
                    Pendientes = g.Count(a => a.Estado == "Pendiente"),
                    Confirmadas = g.Count(a => a.Estado == "Confirmado"),
                    FalsosPositivos = g.Count(a => a.Estado == "Falso Positivo")
                })
                .OrderByDescending(e => e.TotalAlertas)
                .ToListAsync();

            return Ok(alertasPorEstacion);
        }
    }
}