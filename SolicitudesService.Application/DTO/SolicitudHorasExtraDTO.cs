using System;

namespace SolicitudesService.Application.DTO
{
    public class SolicitudHorasExtraDTO
    {
        public int IdSolicitudHorasExtra { get; set; } 
        public int IdEmpleado { get; set; } 
        public int CantidadHoras { get; set; } 
        public DateOnly FechaSolicitud { get; set; } 
        public bool EstaAprobada { get; set; } 
    }
}
