using PetrolriosFraudeDetection.Models.Entities;

namespace PetrolriosFraudeDetection.Factories
{
    public abstract class AlertaFraudeFactory
    {
        public abstract AlertaFraude CrearAlerta(string descripcion, int? ventaId);
        
        public AlertaFraude CrearAlertaCompleta(string descripcion, int? ventaId)
        {
            var alerta = CrearAlerta(descripcion, ventaId);
            alerta.FechaDeteccion = DateTime.Now;
            alerta.Estado = "Pendiente";
            return alerta;
        }
    }
    
    public class AlertaFacturacionDuplicadaFactory : AlertaFraudeFactory
    {
        public override AlertaFraude CrearAlerta(string descripcion, int? ventaId)
        {
            return new AlertaFraude
            {
                Tipo = "Facturación Duplicada",
                Descripcion = descripcion,
                VentaId = ventaId
            };
        }
    }
    
    public class AlertaDesaparicionCombustibleFactory : AlertaFraudeFactory
    {
        public override AlertaFraude CrearAlerta(string descripcion, int? ventaId)
        {
            return new AlertaFraude
            {
                Tipo = "Desaparición de Combustible",
                Descripcion = descripcion,
                VentaId = ventaId
            };
        }
    }
    
    public class AlertaAnomaliaVentasFactory : AlertaFraudeFactory
    {
        public override AlertaFraude CrearAlerta(string descripcion, int? ventaId)
        {
            return new AlertaFraude
            {
                Tipo = "Anomalía Ventas-Movimientos",
                Descripcion = descripcion,
                VentaId = ventaId
            };
        }
    }
    
    public class AlertaDiscrepanciaPreciosFactory : AlertaFraudeFactory
    {
        public override AlertaFraude CrearAlerta(string descripcion, int? ventaId)
        {
            return new AlertaFraude
            {
                Tipo = "Discrepancia de Precios",
                Descripcion = descripcion,
                VentaId = ventaId
            };
        }
    }
}