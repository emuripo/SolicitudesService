using System;

namespace SolicitudesService.Application.DTO
{
    public class SolicitudVacacionesDTO
    {
        public int IdSolicitudVacaciones { get; set; }

        public int IdEmpleado { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        public DateTime FechaSolicitud { get; set; }

        public bool EstaAprobada { get; set; }

        public DateTime? FechaAprobacion { get; set; }
    }
}
