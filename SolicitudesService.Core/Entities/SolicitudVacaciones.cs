using System;
using System.ComponentModel.DataAnnotations;

namespace SolicitudesService.Core.Entities
{
    public class SolicitudVacaciones
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdEmpleado { get; set; }

        [Required]
        public int DiasSolicitados { get; set; } 

        [Required]
        public DateTime FechaInicio { get; set; }

        [Required]
        public DateTime FechaFin { get; set; }

        [Required]
        public DateTime FechaSolicitud { get; set; }

        [Required]
        [MaxLength(50)]
        public string Estado { get; set; } = "Pendiente";

        public DateTime? FechaCambioEstado { get; set; }

        [MaxLength(250)]
        public string MotivoRechazo { get; set; } = string.Empty;

        [MaxLength(100)]
        public string CreadoPor { get; set; } = string.Empty;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [MaxLength(100)]
        public string? ModificadoPor { get; set; }

        public DateTime? FechaModificacion { get; set; }
    }

}
