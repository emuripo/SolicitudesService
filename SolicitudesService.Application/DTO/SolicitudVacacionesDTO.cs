using System;

namespace SolicitudesService.Application.DTO
{
    public class SolicitudVacacionesDTO
    {
        public int IdSolicitudVacaciones { get; set; } 
        public int IdEmpleado { get; set; } 
        public DateOnly FechaInicio { get; set; } 
        public DateOnly FechaFin { get; set; } 
        public DateOnly FechaSolicitud { get; set; } 
        public bool EstaAprobada { get; set; } 
    }
}
