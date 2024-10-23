using System;
using System.ComponentModel.DataAnnotations;

namespace SolicitudesService.Core.Entities
{
    public class SolicitudDocumentos
    {
        [Key]
        public int IdSolicitudDocumentos { get; set; }
        public int IdEmpleado { get; set; }
        public string TipoDocumento { get; set; } = string.Empty;
        public DateTime FechaSolicitud { get; set; }
        public bool EstaAprobada { get; set; }
        public DateTime? FechaAprobacion { get; set; }
    }
}
