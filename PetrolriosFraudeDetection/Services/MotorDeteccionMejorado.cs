using PetrolriosFraudeDetection.Data;
using PetrolriosFraudeDetection.Interfaces;
using PetrolriosFraudeDetection.Models.Entities;
using PetrolriosFraudeDetection.Services.Estrategias;

namespace PetrolriosFraudeDetection.Services
{
    public class MotorDeteccionMejorado : IMotorDeteccion
    {
        private readonly List<IEstrategiaDeteccion> _estrategias;
        
        public MotorDeteccionMejorado(ApplicationDbContext context)
        {
            // Inicializar todas las estrategias disponibles
            _estrategias = new List<IEstrategiaDeteccion>
            {
                new EstrategiaDeteccionFacturas(context),
                new EstrategiaDeteccionCombustible(context),
                new EstrategiaDeteccionPrecios(context)
            };
        }
        
        public async Task<List<AlertaFraude>> AnalizarPatrones(DateTime fecha)
        {
            var todasLasAlertas = new List<AlertaFraude>();
            
            // Ejecutar todas las estrategias de detección
            foreach (var estrategia in _estrategias)
            {
                try
                {
                    var alertas = await estrategia.Detectar(fecha);
                    todasLasAlertas.AddRange(alertas);
                }
                catch (Exception ex)
                {
                    // Log del error (en producción usaríamos ILogger)
                    Console.WriteLine($"Error en estrategia {estrategia.TipoDeteccion}: {ex.Message}");
                }
            }
            
            return todasLasAlertas;
        }
        
        /// Método para agregar nuevas estrategias dinámicamente
        public void AgregarEstrategia(IEstrategiaDeteccion estrategia)
        {
            _estrategias.Add(estrategia);
        }
        
        /// Método para obtener las estrategias activas
        public IEnumerable<string> ObtenerEstrategiasActivas()
        {
            return _estrategias.Select(e => e.TipoDeteccion);
        }
    }
}