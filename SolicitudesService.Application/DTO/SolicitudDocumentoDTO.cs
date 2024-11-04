using System;

namespace SolicitudesService.Application.DTO
{
    public class SolicitudDocumentoDTO
    {
        public int IdSolicitudDocumento { get; set; }
        public int IdEmpleado { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public DateTime FechaSolicitud { get; set; }
        public bool EstaAprobada { get; set; }
        public DateTime? FechaAprobacion { get; set; } 
    }
}
