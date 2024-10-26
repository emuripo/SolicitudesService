using System;
using System.ComponentModel.DataAnnotations;

namespace SolicitudesService.Core.Entities
{
    public class SolicitudPersonal
    {
        [Key]
        public int IdSolicitudPersonal { get; set; }
        public int IdEmpleado { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public DateTime FechaSolicitud { get; set; }
        public bool EstaAprobada { get; set; }
        public DateTime? FechaAprobacion { get; set; }
    }
}
