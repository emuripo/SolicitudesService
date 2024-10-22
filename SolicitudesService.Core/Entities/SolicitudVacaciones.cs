using System;
using System.ComponentModel.DataAnnotations;

namespace SolicitudesService.Core.Entities
{
    public class SolicitudVacaciones
    {
        [Key]
        public int IdSolicitudVacaciones { get; set; }
        public int IdEmpleado { get; set; }
        public int DiasSolicitados { get; set; }
        public DateOnly FechaInicio { get; set; }
        public DateOnly FechaFin { get; set; }
        public DateOnly FechaSolicitud { get; set; }
        public bool EstaAprobada { get; set; }
        public DateOnly? FechaAprobacion { get; set; }
    }
}
