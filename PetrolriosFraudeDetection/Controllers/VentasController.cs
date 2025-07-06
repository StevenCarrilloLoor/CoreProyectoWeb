using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetrolriosFraudeDetection.Data;
using PetrolriosFraudeDetection.Models.Entities;

namespace PetrolriosFraudeDetection.Controllers
{
    public class VentasController : Controller
    {
        private readonly ApplicationDbContext _context;
        
        public VentasController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        // GET: Ventas
        public async Task<IActionResult> Index()
        {
            var ventas = await _context.Ventas
                .Include(v => v.Estacion)
                .ToListAsync();
            return View(ventas);
        }
        
        // GET: Ventas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venta = await _context.Ventas
                .Include(v => v.Estacion)
                .Include(v => v.Facturas)
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (venta == null)
            {
                return NotFound();
            }

            return View(venta);
        }
        
        // GET: Ventas/Create
        public async Task<IActionResult> Create()
        {
            // Dropdown para seleccionar la estación (cumpliendo requisito 2)
            ViewBag.Estaciones = new SelectList(await _context.Estaciones.Where(e => e.Activo).ToListAsync(), "Id", "Nombre");
            return View();
        }
        
        // POST: Ventas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Fecha,LitrosVendidos,MontoTotal,NumeroTransaccion,EstacionId")] Venta venta)
        {
            // Validación del número de transacción en back-end
            if (_context.Ventas.Any(v => v.NumeroTransaccion == venta.NumeroTransaccion))
            {
                ModelState.AddModelError("NumeroTransaccion", "El número de transacción ya existe.");
                ViewBag.Estaciones = new SelectList(await _context.Estaciones.Where(e => e.Activo).ToListAsync(), "Id", "Nombre");
                return View(venta);
            }
            
            if (ModelState.IsValid)
            {
                _context.Add(venta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Estaciones = new SelectList(await _context.Estaciones.Where(e => e.Activo).ToListAsync(), "Id", "Nombre");
            return View(venta);
        }
        
        // GET: Ventas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venta = await _context.Ventas.FindAsync(id);
            if (venta == null)
            {
                return NotFound();
            }
            
            ViewBag.Estaciones = new SelectList(await _context.Estaciones.Where(e => e.Activo).ToListAsync(), "Id", "Nombre", venta.EstacionId);
            return View(venta);
        }
        
        // POST: Ventas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Fecha,LitrosVendidos,MontoTotal,NumeroTransaccion,EstacionId")] Venta venta)
        {
            if (id != venta.Id)
            {
                return NotFound();
            }
            
            // Validación del número de transacción en back-end
            if (_context.Ventas.Any(v => v.NumeroTransaccion == venta.NumeroTransaccion && v.Id != venta.Id))
            {
                ModelState.AddModelError("NumeroTransaccion", "El número de transacción ya existe.");
                ViewBag.Estaciones = new SelectList(await _context.Estaciones.Where(e => e.Activo).ToListAsync(), "Id", "Nombre", venta.EstacionId);
                return View(venta);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(venta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VentaExists(venta.Id))
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
            ViewBag.Estaciones = new SelectList(await _context.Estaciones.Where(e => e.Activo).ToListAsync(), "Id", "Nombre", venta.EstacionId);
            return View(venta);
        }
        
        // GET: Ventas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venta = await _context.Ventas
                .Include(v => v.Estacion)
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (venta == null)
            {
                return NotFound();
            }

            return View(venta);
        }
        
        // POST: Ventas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venta = await _context.Ventas.FindAsync(id);
            
            if (venta != null)
            {
                // Verificar si hay facturas asociadas
                bool tieneFacturas = await _context.Facturas.AnyAsync(f => f.VentaId == id);
                bool tieneAlertas = await _context.AlertasFraude.AnyAsync(a => a.VentaId == id);
                
                if (tieneFacturas || tieneAlertas)
                {
                    ModelState.AddModelError(string.Empty, "No se puede eliminar la venta porque tiene registros asociados.");
                    return View(venta);
                }
                
                _context.Ventas.Remove(venta);
                await _context.SaveChangesAsync();
            }
            
            return RedirectToAction(nameof(Index));
        }
        
        private bool VentaExists(int id)
        {
            return _context.Ventas.Any(e => e.Id == id);
        }
    }
}