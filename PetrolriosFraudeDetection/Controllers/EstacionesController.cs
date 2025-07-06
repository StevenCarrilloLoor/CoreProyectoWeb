using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetrolriosFraudeDetection.Data;
using PetrolriosFraudeDetection.Models.Entities;
using PetrolriosFraudeDetection.Validators;

namespace PetrolriosFraudeDetection.Controllers
{
    public class EstacionesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly CodigoEstacionValidator _validator;
        
        public EstacionesController(ApplicationDbContext context, CodigoEstacionValidator validator)
        {
            _context = context;
            _validator = validator;
        }
        
        // GET: Estaciones
        public async Task<IActionResult> Index()
        {
            return View(await _context.Estaciones.ToListAsync());
        }
        
        // GET: Estaciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estacion = await _context.Estaciones
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (estacion == null)
            {
                return NotFound();
            }

            return View(estacion);
        }
        
        // GET: Estaciones/Create
        public IActionResult Create()
        {
            return View();
        }
        
        // POST: Estaciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Ubicacion,Codigo,Activo")] Estacion estacion)
        {
            // Validación de back-end para el código de estación
            if (!_validator.ValidarCodigo(estacion.Codigo))
            {
                ModelState.AddModelError("Codigo", "El código de estación no es válido o ya existe.");
                return View(estacion);
            }
            
            if (ModelState.IsValid)
            {
                _context.Add(estacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(estacion);
        }
        
        // GET: Estaciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estacion = await _context.Estaciones.FindAsync(id);
            if (estacion == null)
            {
                return NotFound();
            }
            return View(estacion);
        }
        
        // POST: Estaciones/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Ubicacion,Codigo,Activo")] Estacion estacion)
        {
            if (id != estacion.Id)
            {
                return NotFound();
            }
            
            // Validación de back-end para el código de estación
            if (!_validator.ValidarCodigo(estacion.Codigo, estacion.Id))
            {
                ModelState.AddModelError("Codigo", "El código de estación no es válido o ya existe.");
                return View(estacion);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(estacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EstacionExists(estacion.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(estacion);
        }
        
        // GET: Estaciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estacion = await _context.Estaciones
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (estacion == null)
            {
                return NotFound();
            }

            return View(estacion);
        }
        
        // POST: Estaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var estacion = await _context.Estaciones.FindAsync(id);
            
            if (estacion != null)
            {
                // Verificar si hay dependencias
                bool tieneVentas = await _context.Ventas.AnyAsync(v => v.EstacionId == id);
                bool tieneSensores = await _context.SensoresVolumen.AnyAsync(s => s.EstacionId == id);
                bool tieneInventarios = await _context.Inventarios.AnyAsync(i => i.EstacionId == id);
                
                if (tieneVentas || tieneSensores || tieneInventarios)
                {
                    ModelState.AddModelError(string.Empty, "No se puede eliminar la estación porque tiene registros asociados.");
                    return View(estacion);
                }
                
                _context.Estaciones.Remove(estacion);
                await _context.SaveChangesAsync();
            }
            
            return RedirectToAction(nameof(Index));
        }
        
        private bool EstacionExists(int id)
        {
            return _context.Estaciones.Any(e => e.Id == id);
        }
    }
}