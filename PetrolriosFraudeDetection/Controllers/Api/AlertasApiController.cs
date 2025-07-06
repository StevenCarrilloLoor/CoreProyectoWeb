using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetrolriosFraudeDetection.Data;
using PetrolriosFraudeDetection.Interfaces;

namespace PetrolriosFraudeDetection.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlertasApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMotorDeteccion _motorDeteccion;

        public AlertasApiController(ApplicationDbContext context, IMotorDeteccion motorDeteccion)
        {
            _context = context;
            _motorDeteccion = motorDeteccion;
        }

        // GET: api/AlertasApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAlertas([FromQuery] string? estado = null, [FromQuery] string? tipo = null)
        {
            var query = _context.AlertasFraude
                .Include(a => a.Venta)
                .ThenInclude(v => v.Estacion)
                .AsQueryable();

            if (!string.IsNullOrEmpty(estado))
            {
                query = query.Where(a => a.Estado == estado);
            }

            if (!string.IsNullOrEmpty(tipo))
            {
                query = query.Where(a => a.Tipo == tipo);
            }

            var alertas = await query
                .OrderByDescending(a => a.FechaDeteccion)
                .Select(a => new
                {
                    a.Id,
                    a.Tipo,
                    a.Descripcion,
                    a.Estado,
                    a.FechaDeteccion,
                    a.FechaResolucion,
                    Estacion = a.Venta != null && a.Venta.Estacion != null ? a.Venta.Estacion.Nombre : "N/A",
                    Venta = a.Venta != null ? new
                    {
                        a.Venta.Id,
                        a.Venta.NumeroTransaccion,
                        a.Venta.Fecha,
                        a.Venta.LitrosVendidos,
                        a.Venta.MontoTotal
                    } : null
                })
                .ToListAsync();

            return Ok(alertas);
        }

        // GET: api/AlertasApi/estadisticas
        [HttpGet("estadisticas")]
        public async Task<ActionResult<object>> GetEstadisticas()
        {
            var totalAlertas = await _context.AlertasFraude.CountAsync();
            var alertasPendientes = await _context.AlertasFraude.CountAsync(a => a.Estado == "Pendiente");
            var alertasConfirmadas = await _context.AlertasFraude.CountAsync(a => a.Estado == "Confirmado");
            var alertasFalsosPositivos = await _context.AlertasFraude.CountAsync(a => a.Estado == "Falso Positivo");

            var alertasPorTipo = await _context.AlertasFraude
                .GroupBy(a => a.Tipo)
                .Select(g => new
                {
                    Tipo = g.Key,
                    Cantidad = g.Count()
                })
                .ToListAsync();

            var alertasPorEstacion = await _context.AlertasFraude
                .Where(a => a.Venta != null)
                .GroupBy(a => a.Venta.Estacion.Nombre)
                .Select(g => new
                {
                    Estacion = g.Key,
                    Cantidad = g.Count()
                })
                .ToListAsync();

            return Ok(new
            {
                TotalAlertas = totalAlertas,
                Pendientes = alertasPendientes,
                Confirmadas = alertasConfirmadas,
                FalsosPositivos = alertasFalsosPositivos,
                PorTipo = alertasPorTipo,
                PorEstacion = alertasPorEstacion
            });
        }

        // POST: api/AlertasApi/analizar
        [HttpPost("analizar")]
        public async Task<ActionResult<object>> AnalizarFecha([FromBody] AnalizarRequest request)
        {
            if (request.Fecha == default)
            {
                return BadRequest(new { mensaje = "La fecha es requerida" });
            }

            var alertas = await _motorDeteccion.AnalizarPatrones(request.Fecha);

            if (alertas.Any())
            {
                foreach (var alerta in alertas)
                {
                    _context.AlertasFraude.Add(alerta);
                }
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    mensaje = $"Se detectaron {alertas.Count} alertas para la fecha {request.Fecha:yyyy-MM-dd}",
                    alertasCreadas = alertas.Count,
                    alertas = alertas.Select(a => new
                    {
                        a.Tipo,
                        a.Descripcion,
                        a.Estado
                    })
                });
            }

            return Ok(new
            {
                mensaje = $"No se detectaron alertas para la fecha {request.Fecha:yyyy-MM-dd}",
                alertasCreadas = 0
            });
        }

        // PUT: api/AlertasApi/5/resolver
        [HttpPut("{id}/resolver")]
        public async Task<IActionResult> ResolverAlerta(int id, [FromBody] ResolverAlertaRequest request)
        {
            var alerta = await _context.AlertasFraude.FindAsync(id);
            if (alerta == null)
            {
                return NotFound();
            }

            alerta.Estado = request.Estado;
            alerta.FechaResolucion = DateTime.Now;
            alerta.UsuarioId = 1; // En producción, obtener del sistema de autenticación

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Alerta resuelta exitosamente" });
        }
    }

    // DTOs para las requests
    public class AnalizarRequest
    {
        public DateTime Fecha { get; set; }
    }

    public class ResolverAlertaRequest
    {
        public required string Estado { get; set; }
        public string? Comentario { get; set; }
    }
}