using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetrolriosFraudeDetection.Data;

namespace PetrolriosFraudeDetection.Controllers
{
    public class DiagnosticController : Controller
    {
        private readonly ILogger<DiagnosticController> _logger;
        private readonly ApplicationDbContext _context;

        public DiagnosticController(ILogger<DiagnosticController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            try
            {
                ViewBag.ConnectionString = _context.Database.GetConnectionString() ?? "No connection string available";
                
                // Test database connection
                bool canConnect = false;
                try
                {
                    canConnect = _context.Database.CanConnect();
                    ViewBag.CanConnect = canConnect;
                }
                catch (Exception ex)
                {
                    ViewBag.ConnectionError = ex.Message;
                }

                if (canConnect)
                {
                    // Test table existence
                    try
                    {
                        var estaciones = _context.Estaciones.ToList();
                        ViewBag.EstacionesCount = estaciones.Count;
                        ViewBag.EstacionesData = estaciones;
                    }
                    catch (Exception ex)
                    {
                        ViewBag.EstacionesError = ex.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.MainError = ex.Message;
                ViewBag.MainStackTrace = ex.StackTrace;
            }

            return View();
        }
    }
}