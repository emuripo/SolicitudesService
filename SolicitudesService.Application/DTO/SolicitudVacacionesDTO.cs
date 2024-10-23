using System;

namespace SolicitudesService.Application.DTO
{
    public class SolicitudVacacionesDTO
    {
        public int IdSolicitudVacaciones { get; set; }

        public int IdEmpleado { get; set; }

        public int DiasSolicitados { get; set; } 

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        public DateTime FechaSolicitud { get; set; }

        public bool EstaAprobada { get; set; }

        public DateTime? FechaAprobacion { get; set; }

        public int DiasGozados
        {
            get
            {
                return (FechaFin - FechaInicio).Days + 1; 
            }
        }

        public int DiasRestantes { get; set; } 
    }
}
