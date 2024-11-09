namespace SolicitudesService.Application.DTO
{
    public class SolicitudHorasExtraDTO
    {
        public int Id { get; set; }
        public int IdEmpleado { get; set; }
        public int CantidadHoras { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public DateTime FechaTrabajo { get; set; }
        public string Estado { get; set; } = "Pendiente";
        public DateTime? FechaCambioEstado { get; set; }
        public string MotivoRechazo { get; set; } = string.Empty;
        public string? ModificadoPor { get; set; } 
    }
}
