using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetrolriosFraudeDetection.Data;
using PetrolriosFraudeDetection.Models.Entities;

namespace PetrolriosFraudeDetection.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstacionesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EstacionesApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/EstacionesApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetEstaciones()
        {
            var estaciones = await _context.Estaciones
                .Where(e => e.Activo)
                .Select(e => new
                {
                    e.Id,
                    e.Nombre,
                    e.Ubicacion,
                    e.Codigo,
                    e.Activo
                })
                .ToListAsync();

            return Ok(estaciones);
        }

        // GET: api/EstacionesApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetEstacion(int id)
        {
            var estacion = await _context.Estaciones
                .Where(e => e.Id == id)
                .Select(e => new
                {
                    e.Id,
                    e.Nombre,
                    e.Ubicacion,
                    e.Codigo,
                    e.Activo,
                    VentasRecientes = e.Ventas
                        .OrderByDescending(v => v.Fecha)
                        .Take(5)
                        .Select(v => new
                        {
                            v.Id,
                            v.Fecha,
                            v.LitrosVendidos,
                            v.MontoTotal,
                            v.NumeroTransaccion
                        })
                })
                .FirstOrDefaultAsync();

            if (estacion == null)
            {
                return NotFound();
            }

            return Ok(estacion);
        }

        // POST: api/EstacionesApi
        [HttpPost]
        public async Task<ActionResult<Estacion>> PostEstacion(Estacion estacion)
        {
            _context.Estaciones.Add(estacion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEstacion), new { id = estacion.Id }, estacion);
        }

        // PUT: api/EstacionesApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEstacion(int id, Estacion estacion)
        {
            if (id != estacion.Id)
            {
                return BadRequest();
            }

            _context.Entry(estacion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EstacionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/EstacionesApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEstacion(int id)
        {
            var estacion = await _context.Estaciones.FindAsync(id);
            if (estacion == null)
            {
                return NotFound();
            }

            estacion.Activo = false; // Soft delete
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EstacionExists(int id)
        {
            return _context.Estaciones.Any(e => e.Id == id);
        }
    }
}