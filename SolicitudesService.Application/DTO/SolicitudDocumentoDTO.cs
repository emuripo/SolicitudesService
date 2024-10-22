using System;

namespace SolicitudesService.Application.DTO
{
    public class SolicitudDocumentoDTO
    {
        public int IdSolicitudDocumento { get; set; } 
        public int IdEmpleado { get; set; } 
        public string TipoDocumento { get; set; } = string.Empty;
        public DateOnly FechaSolicitud { get; set; } 
        public bool EstaAprobada { get; set; } 
    }
}
