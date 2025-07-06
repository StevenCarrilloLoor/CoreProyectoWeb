using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetrolriosFraudeDetection.Data;
using PetrolriosFraudeDetection.Models.Entities;
using PetrolriosFraudeDetection.Validators;

namespace PetrolriosFraudeDetection.Controllers
{
    public class FacturasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly NumeroFacturaValidator _validator;
        
        public FacturasController(ApplicationDbContext context, NumeroFacturaValidator validator)
        {
            _context = context;
            _validator = validator;
        }
        
        // GET: Facturas
        public async Task<IActionResult> Index()
        {
            var facturas = await _context.Facturas
                .Include(f => f.Venta)
                .Include(f => f.Venta.Estacion)
                .ToListAsync();
            return View(facturas);
        }
        
        // GET: Facturas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var factura = await _context.Facturas
                .Include(f => f.Venta)
                .Include(f => f.Venta.Estacion)
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (factura == null)
            {
                return NotFound();
            }

            return View(factura);
        }
        
        // GET: Facturas/Create
        public async Task<IActionResult> Create()
        {
            // Primero dropdown para estaciones (requisito 2)
            ViewBag.Estaciones = new SelectList(await _context.Estaciones.Where(e => e.Activo).ToListAsync(), "Id", "Nombre");
            
            // El dropdown de ventas se cargará vía AJAX cuando se seleccione una estación
            ViewBag.Ventas = new SelectList(new List<Venta>(), "Id", "NumeroTransaccion");
            
            return View();
        }
        
        // POST: Facturas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NumeroFactura,FechaEmision,Monto,Anulada,VentaId")] Factura factura)
        {
            // Validación de back-end para el número de factura
            if (!_validator.ValidarNumeroFactura(factura.NumeroFactura))
            {
                ModelState.AddModelError("NumeroFactura", "El número de factura no es válido o ya existe.");
                
                // Recargar los dropdowns
                var venta = await _context.Ventas.FindAsync(factura.VentaId);
                if (venta != null)
                {
                    ViewBag.Estaciones = new SelectList(await _context.Estaciones.Where(e => e.Activo).ToListAsync(), "Id", "Nombre", venta.EstacionId);
                    ViewBag.Ventas = new SelectList(await _context.Ventas.Where(v => v.EstacionId == venta.EstacionId).ToListAsync(), "Id", "NumeroTransaccion", factura.VentaId);
                }
                else
                {
                    ViewBag.Estaciones = new SelectList(await _context.Estaciones.Where(e => e.Activo).ToListAsync(), "Id", "Nombre");
                    ViewBag.Ventas = new SelectList(new List<Venta>(), "Id", "NumeroTransaccion");
                }
                
                return View(factura);
            }
            
            if (ModelState.IsValid)
            {
                _context.Add(factura);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            // Recargar los dropdowns
            var ventaModel = await _context.Ventas.FindAsync(factura.VentaId);
            if (ventaModel != null)
            {
                ViewBag.Estaciones = new SelectList(await _context.Estaciones.Where(e => e.Activo).ToListAsync(), "Id", "Nombre", ventaModel.EstacionId);
                ViewBag.Ventas = new SelectList(await _context.Ventas.Where(v => v.EstacionId == ventaModel.EstacionId).ToListAsync(), "Id", "NumeroTransaccion", factura.VentaId);
            }
            else
            {
                ViewBag.Estaciones = new SelectList(await _context.Estaciones.Where(e => e.Activo).ToListAsync(), "Id", "Nombre");
                ViewBag.Ventas = new SelectList(new List<Venta>(), "Id", "NumeroTransaccion");
            }
            
            return View(factura);
        }
        
        // GET: Facturas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var factura = await _context.Facturas
                .Include(f => f.Venta)
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (factura == null)
            {
                return NotFound();
            }
            
            ViewBag.Estaciones = new SelectList(await _context.Estaciones.Where(e => e.Activo).ToListAsync(), "Id", "Nombre", factura.Venta.EstacionId);
            ViewBag.Ventas = new SelectList(await _context.Ventas.Where(v => v.EstacionId == factura.Venta.EstacionId).ToListAsync(), "Id", "NumeroTransaccion", factura.VentaId);
            
            return View(factura);
        }
        
        // POST: Facturas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NumeroFactura,FechaEmision,Monto,Anulada,VentaId")] Factura factura)
        {
            if (id != factura.Id)
            {
                return NotFound();
            }
            
            // Validación de back-end para el número de factura
            if (!_validator.ValidarNumeroFactura(factura.NumeroFactura, factura.Id))
            {
                ModelState.AddModelError("NumeroFactura", "El número de factura no es válido o ya existe.");
                
                // Recargar los dropdowns
                var venta = await _context.Ventas.FindAsync(factura.VentaId);
                if (venta != null)
                {
                    ViewBag.Estaciones = new SelectList(await _context.Estaciones.Where(e => e.Activo).ToListAsync(), "Id", "Nombre", venta.EstacionId);
                    ViewBag.Ventas = new SelectList(await _context.Ventas.Where(v => v.EstacionId == venta.EstacionId).ToListAsync(), "Id", "NumeroTransaccion", factura.VentaId);
                }
                else
                {
                    ViewBag.Estaciones = new SelectList(await _context.Estaciones.Where(e => e.Activo).ToListAsync(), "Id", "Nombre");
                    ViewBag.Ventas = new SelectList(new List<Venta>(), "Id", "NumeroTransaccion");
                }
                
                return View(factura);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(factura);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FacturaExists(factura.Id))
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
            
            // Recargar los dropdowns
            var ventaModel = await _context.Ventas.FindAsync(factura.VentaId);
            if (ventaModel != null)
            {
                ViewBag.Estaciones = new SelectList(await _context.Estaciones.Where(e => e.Activo).ToListAsync(), "Id", "Nombre", ventaModel.EstacionId);
                ViewBag.Ventas = new SelectList(await _context.Ventas.Where(v => v.EstacionId == ventaModel.EstacionId).ToListAsync(), "Id", "NumeroTransaccion", factura.VentaId);
            }
            else
            {
                ViewBag.Estaciones = new SelectList(await _context.Estaciones.Where(e => e.Activo).ToListAsync(), "Id", "Nombre");
                ViewBag.Ventas = new SelectList(new List<Venta>(), "Id", "NumeroTransaccion");
            }
            
            return View(factura);
        }
        
        // GET: Facturas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var factura = await _context.Facturas
                .Include(f => f.Venta)
                .Include(f => f.Venta.Estacion)
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (factura == null)
            {
                return NotFound();
            }

            return View(factura);
        }
        
        // POST: Facturas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var factura = await _context.Facturas.FindAsync(id);
            
            if (factura != null)
            {
                _context.Facturas.Remove(factura);
                await _context.SaveChangesAsync();
            }
            
            return RedirectToAction(nameof(Index));
        }
        
        // Acción AJAX para cargar las ventas de una estación
        [HttpGet]
        public async Task<JsonResult> GetVentasByEstacion(int estacionId)
        {
            var ventas = await _context.Ventas
                .Where(v => v.EstacionId == estacionId)
                .Select(v => new { Id = v.Id, NumeroTransaccion = v.NumeroTransaccion })
                .ToListAsync();
                
            return Json(ventas);
        }
        
        private bool FacturaExists(int id)
        {
            return _context.Facturas.Any(e => e.Id == id);
        }
    }
}