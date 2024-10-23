using System;
using System.ComponentModel.DataAnnotations;

namespace SolicitudesService.Core.Entities
{
    public class SolicitudVacaciones
    {
        [Key]
        public int IdSolicitudVacaciones { get; set; }
        public int IdEmpleado { get; set; }
        public int DiasSolicitados { get; set; } // Este campo ya es correcto
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public bool EstaAprobada { get; set; }
        public DateTime? FechaAprobacion { get; set; } // También mantenemos la fecha de aprobación
    }
}
