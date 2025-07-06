using PetrolriosFraudeDetection.Models.Entities;

namespace PetrolriosFraudeDetection.Interfaces
{
    public interface IEstrategiaDeteccion
    {
        Task<List<AlertaFraude>> Detectar(DateTime fecha);
        string TipoDeteccion { get; }
    }
}