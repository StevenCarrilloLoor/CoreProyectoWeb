using PetrolriosFraudeDetection.Models.Entities;

namespace PetrolriosFraudeDetection.Interfaces
{
    public interface IMotorDeteccion
    {
        Task<List<AlertaFraude>> AnalizarPatrones(DateTime fecha);
    }
}