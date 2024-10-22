using System;
using System.ComponentModel.DataAnnotations;

namespace SolicitudesService.Core.Entities
{
    public class SolicitudHorasExtra
    {
        [Key]
        public int IdSolicitudHorasExtra { get; set; }
        public int IdEmpleado { get; set; }
        public int CantidadHoras { get; set; }
        public decimal MontoPorHora { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public bool EstaAprobada { get; set; }
        public DateTime? FechaAprobacion { get; set; }
    }
}
