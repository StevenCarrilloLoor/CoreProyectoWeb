using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetrolriosFraudeDetection.Data;
using PetrolriosFraudeDetection.Interfaces;
using PetrolriosFraudeDetection.Models.Entities;

namespace PetrolriosFraudeDetection.Controllers
{

    public class AlertasFraudeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMotorDeteccion _motorDeteccion;
        
        public AlertasFraudeController(ApplicationDbContext context, IMotorDeteccion motorDeteccion)
        {
            _context = context;
            _motorDeteccion = motorDeteccion;
        }
        
        // GET: AlertasFraude
        public async Task<IActionResult> Index()
        {
            var alertas = await _context.AlertasFraude
                .Include(a => a.Venta)
                .Include(a => a.Venta.Estacion)
                .Include(a => a.Usuario)
                .OrderByDescending(a => a.FechaDeteccion)
                .ToListAsync();
            return View(alertas);
        }
        
        // GET: AlertasFraude/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alerta = await _context.AlertasFraude
                .Include(a => a.Venta)
                .Include(a => a.Venta.Estacion)
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (alerta == null)
            {
                return NotFound();
            }

            return View(alerta);
        }
        
        // POST: AlertasFraude/ResolverAlerta
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResolverAlerta(int id, string estado, string comentario)
        {
            var alerta = await _context.AlertasFraude.FindAsync(id);
            if (alerta == null)
            {
                return NotFound();
            }
            
            // Actualizar el estado de la alerta
            alerta.Estado = estado; // "Confirmado" o "Falso Positivo"
            alerta.FechaResolucion = DateTime.Now;
            
            // Obtener el usuario actual del sistema de autenticación (simplificado para el ejemplo)
            var usuarioId = 1; // Esto debería obtenerse del sistema de autenticación
            alerta.UsuarioId = usuarioId;
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        // GET: AlertasFraude/Analizar
        public IActionResult Analizar()
        {
            return View();
        }
        
        // POST: AlertasFraude/Analizar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Analizar(DateTime fecha)
        {
            // Usando la interfaz IMotorDeteccion en lugar de la implementación concreta
            var alertas = await _motorDeteccion.AnalizarPatrones(fecha);
            
            if (alertas.Any())
            {
                foreach (var alerta in alertas)
                {
                    _context.AlertasFraude.Add(alerta);
                }
                await _context.SaveChangesAsync();
                TempData["Message"] = $"Se detectaron {alertas.Count} alertas para la fecha {fecha.ToShortDateString()}.";
            }
            else
            {
                TempData["Message"] = $"No se detectaron alertas para la fecha {fecha.ToShortDateString()}.";
            }
            
            return RedirectToAction(nameof(Index));
        }
    }
}