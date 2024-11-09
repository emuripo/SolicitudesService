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
    public class SolicitudPersonalService : ISolicitudPersonalService
    {
        private readonly SolicitudesServiceDbContext _context;
        private readonly ILogger<SolicitudPersonalService> _logger;

        public SolicitudPersonalService(SolicitudesServiceDbContext context, ILogger<SolicitudPersonalService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<SolicitudPersonalDTO> CrearSolicitudAsync(SolicitudPersonalDTO solicitudDTO)
        {
            var solicitud = new SolicitudPersonal
            {
                IdEmpleado = solicitudDTO.IdEmpleado,
                Motivo = solicitudDTO.Motivo,
                FechaSolicitud = DateTime.Now,
                Estado = "Pendiente",
                CreadoPor = solicitudDTO.ModificadoPor ?? "UsuarioDesconocido"
            };

            _context.SolicitudesPersonales.Add(solicitud);
            await _context.SaveChangesAsync();

            solicitudDTO.Id = solicitud.Id;
            return solicitudDTO;
        }

        public async Task<SolicitudPersonalDTO?> ObtenerSolicitudPorIdAsync(int id)
        {
            var solicitud = await _context.SolicitudesPersonales.FindAsync(id);
            return solicitud == null ? null : MapToDTO(solicitud);
        }

        public async Task<IEnumerable<SolicitudPersonalDTO>> ObtenerSolicitudesPorEmpleadoAsync(int idEmpleado)
        {
            var solicitudes = await _context.SolicitudesPersonales
                .Where(s => s.IdEmpleado == idEmpleado)
                .ToListAsync();

            return solicitudes.Select(MapToDTO);
        }

        public async Task<bool> ActualizarSolicitudAsync(SolicitudPersonalDTO solicitudDTO)
        {
            var solicitud = await _context.SolicitudesPersonales.FindAsync(solicitudDTO.Id);
            if (solicitud == null || solicitud.Estado != "Pendiente")
            {
                return false;
            }

            solicitud.Motivo = solicitudDTO.Motivo;
            solicitud.FechaModificacion = DateTime.Now;
            solicitud.ModificadoPor = solicitudDTO.ModificadoPor;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarSolicitudAsync(int id)
        {
            var solicitud = await _context.SolicitudesPersonales.FindAsync(id);
            if (solicitud == null || solicitud.Estado != "Pendiente")
            {
                return false;
            }

            _context.SolicitudesPersonales.Remove(solicitud);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AprobarSolicitudAsync(int id)
        {
            var solicitud = await _context.SolicitudesPersonales.FindAsync(id);
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
            var solicitud = await _context.SolicitudesPersonales.FindAsync(id);
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

        public async Task<IEnumerable<SolicitudPersonalDTO>> ObtenerTodasSolicitudesAsync()
        {
            var solicitudes = await _context.SolicitudesPersonales.ToListAsync();
            return solicitudes.Select(MapToDTO);
        }

        // Método de mapeo para convertir la entidad a DTO
        private SolicitudPersonalDTO MapToDTO(SolicitudPersonal solicitud)
        {
            return new SolicitudPersonalDTO
            {
                Id = solicitud.Id,
                IdEmpleado = solicitud.IdEmpleado,
                Motivo = solicitud.Motivo,
                FechaSolicitud = solicitud.FechaSolicitud,
                Estado = solicitud.Estado,
                FechaCambioEstado = solicitud.FechaCambioEstado,
                MotivoRechazo = solicitud.MotivoRechazo,
                ModificadoPor = solicitud.ModificadoPor
            };
        }
    }
}
