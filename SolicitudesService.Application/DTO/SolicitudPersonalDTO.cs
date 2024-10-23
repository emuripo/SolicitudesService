using System;

namespace SolicitudesService.Application.DTO
{
    public class SolicitudPersonalDTO
    {
        public int IdSolicitudPersonal { get; set; }
        public int IdEmpleado { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public DateTime FechaSolicitud { get; set; }
        public bool EstaAprobada { get; set; }
        public DateTime? FechaAprobacion { get; set; }
    }
}
