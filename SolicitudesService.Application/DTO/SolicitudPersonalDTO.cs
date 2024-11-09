namespace SolicitudesService.Application.DTO
{
    public class SolicitudPersonalDTO
    {
        public int Id { get; set; }
        public int IdEmpleado { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public DateTime FechaSolicitud { get; set; }
        public string Estado { get; set; } = "Pendiente";
        public DateTime? FechaCambioEstado { get; set; }
        public string MotivoRechazo { get; set; } = string.Empty;
        public string? ModificadoPor { get; set; } 
    }
}
