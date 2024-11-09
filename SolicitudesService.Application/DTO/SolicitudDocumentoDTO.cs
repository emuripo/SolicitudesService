namespace SolicitudesService.Application.DTO
{
    public class SolicitudDocumentoDTO
    {
        public int Id { get; set; }
        public int IdEmpleado { get; set; }
        public string TipoDocumento { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public DateTime FechaSolicitud { get; set; }
        public string Estado { get; set; } = "Pendiente";
        public DateTime? FechaCambioEstado { get; set; }
        public string MotivoRechazo { get; set; } = string.Empty;
        public string? ModificadoPor { get; set; } 
    }
}
