namespace SolicitudesService.Application.DTO
{
    public class SolicitudVacacionesDTO
    {
        public int Id { get; set; }
        public int IdEmpleado { get; set; }
        public int DiasSolicitados { get; set; } 
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public string Estado { get; set; } = "Pendiente";
        public DateTime? FechaCambioEstado { get; set; }
        public string MotivoRechazo { get; set; } = string.Empty;
        public string? ModificadoPor { get; set; }
    }

}
