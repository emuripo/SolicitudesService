using System;

namespace SolicitudesService.Application.DTO
{
    public class SolicitudVacacionesDTO
    {
        public int IdSolicitudVacaciones { get; set; }
        public int IdEmpleado { get; set; }
        public int DiasSolicitados { get; set; } // Nuevo campo para manejar los días solicitados
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public bool EstaAprobada { get; set; }
        public DateTime? FechaAprobacion { get; set; } // Este campo puede ser útil para saber cuándo fue aprobada la solicitud
    }
}

