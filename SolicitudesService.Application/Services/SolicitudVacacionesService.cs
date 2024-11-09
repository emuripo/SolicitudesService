using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SolicitudesService.Application.DTO;
using SolicitudesService.Core.Entities;
using SolicitudesService.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using SolicitudesService.Infrastructure.Data;

namespace SolicitudesService.Services
{
    public class SolicitudVacacionesService : ISolicitudVacacionesService
    {
        private readonly SolicitudesServiceDbContext _context;
        private readonly ILogger<SolicitudVacacionesService> _logger;

        public SolicitudVacacionesService(SolicitudesServiceDbContext context, ILogger<SolicitudVacacionesService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<SolicitudVacacionesDTO> CrearSolicitudAsync(SolicitudVacacionesDTO solicitudDTO)
        {
            var solicitud = new SolicitudVacaciones
            {
                IdEmpleado = solicitudDTO.IdEmpleado,
                DiasSolicitados = solicitudDTO.DiasSolicitados, // Ajustado de CantidadDias
                FechaInicio = solicitudDTO.FechaInicio,
                FechaFin = solicitudDTO.FechaFin,
                FechaSolicitud = DateTime.Now,
                Estado = "Pendiente",
                CreadoPor = solicitudDTO.ModificadoPor ?? "UsuarioDesconocido"
            };

            _context.SolicitudesVacaciones.Add(solicitud);
            await _context.SaveChangesAsync();

            solicitudDTO.Id = solicitud.Id;
            return solicitudDTO;
        }

        public async Task<SolicitudVacacionesDTO?> ObtenerSolicitudPorIdAsync(int id)
        {
            var solicitud = await _context.SolicitudesVacaciones.FindAsync(id);
            return solicitud == null ? null : MapToDTO(solicitud);
        }

        public async Task<IEnumerable<SolicitudVacacionesDTO>> ObtenerSolicitudesPorEmpleadoAsync(int idEmpleado)
        {
            var solicitudes = await _context.SolicitudesVacaciones
                .Where(s => s.IdEmpleado == idEmpleado)
                .ToListAsync();

            return solicitudes.Select(MapToDTO);
        }

        public async Task<bool> ActualizarSolicitudAsync(SolicitudVacacionesDTO solicitudDTO)
        {
            var solicitud = await _context.SolicitudesVacaciones.FindAsync(solicitudDTO.Id);
            if (solicitud == null || solicitud.Estado != "Pendiente")
            {
                return false;
            }

            solicitud.DiasSolicitados = solicitudDTO.DiasSolicitados; // Ajustado de CantidadDias
            solicitud.FechaInicio = solicitudDTO.FechaInicio;
            solicitud.FechaFin = solicitudDTO.FechaFin;
            solicitud.FechaModificacion = DateTime.Now;
            solicitud.ModificadoPor = solicitudDTO.ModificadoPor;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarSolicitudAsync(int id)
        {
            var solicitud = await _context.SolicitudesVacaciones.FindAsync(id);
            if (solicitud == null || solicitud.Estado != "Pendiente")
            {
                return false;
            }

            _context.SolicitudesVacaciones.Remove(solicitud);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AprobarSolicitudAsync(int id)
        {
            var solicitud = await _context.SolicitudesVacaciones.FindAsync(id);
            if (solicitud == null || solicitud.Estado != "Pendiente")
            {
                return false;
            }

            solicitud.Estado = "Aprobada";
            solicitud.FechaCambioEstado = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RechazarSolicitudAsync(int id, string motivoRechazo)
        {
            var solicitud = await _context.SolicitudesVacaciones.FindAsync(id);
            if (solicitud == null || solicitud.Estado != "Pendiente")
            {
                return false;
            }

            solicitud.Estado = "Rechazada";
            solicitud.FechaCambioEstado = DateTime.Now;
            solicitud.MotivoRechazo = motivoRechazo;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<SolicitudVacacionesDTO>> ObtenerTodasSolicitudesAsync()
        {
            var solicitudes = await _context.SolicitudesVacaciones.ToListAsync();
            return solicitudes.Select(MapToDTO);
        }

        // Método de mapeo para convertir la entidad a DTO
        private SolicitudVacacionesDTO MapToDTO(SolicitudVacaciones solicitud)
        {
            return new SolicitudVacacionesDTO
            {
                Id = solicitud.Id,
                IdEmpleado = solicitud.IdEmpleado,
                DiasSolicitados = solicitud.DiasSolicitados, // Ajustado de CantidadDias
                FechaInicio = solicitud.FechaInicio,
                FechaFin = solicitud.FechaFin,
                FechaSolicitud = solicitud.FechaSolicitud,
                Estado = solicitud.Estado,
                FechaCambioEstado = solicitud.FechaCambioEstado,
                MotivoRechazo = solicitud.MotivoRechazo,
                ModificadoPor = solicitud.ModificadoPor
            };
        }
    }
}
