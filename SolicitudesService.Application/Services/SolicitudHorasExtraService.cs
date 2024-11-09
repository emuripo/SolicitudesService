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
    public class SolicitudHorasExtraService : ISolicitudHorasExtraService
    {
        private readonly SolicitudesServiceDbContext _context;
        private readonly ILogger<SolicitudHorasExtraService> _logger;

        public SolicitudHorasExtraService(SolicitudesServiceDbContext context, ILogger<SolicitudHorasExtraService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<SolicitudHorasExtraDTO> CrearSolicitudAsync(SolicitudHorasExtraDTO solicitudDTO)
        {
            var solicitud = new SolicitudHorasExtra
            {
                IdEmpleado = solicitudDTO.IdEmpleado,
                CantidadHoras = solicitudDTO.CantidadHoras,
                FechaSolicitud = DateTime.Now,
                FechaTrabajo = solicitudDTO.FechaTrabajo,
                Estado = "Pendiente",
                CreadoPor = solicitudDTO.ModificadoPor ?? "UsuarioDesconocido"
            };

            _context.SolicitudesHorasExtra.Add(solicitud);
            await _context.SaveChangesAsync();

            solicitudDTO.Id = solicitud.Id;
            return solicitudDTO;
        }

        public async Task<SolicitudHorasExtraDTO?> ObtenerSolicitudPorIdAsync(int id)
        {
            var solicitud = await _context.SolicitudesHorasExtra.FindAsync(id);
            return solicitud == null ? null : MapToDTO(solicitud);
        }

        public async Task<IEnumerable<SolicitudHorasExtraDTO>> ObtenerSolicitudesPorEmpleadoAsync(int idEmpleado)
        {
            var solicitudes = await _context.SolicitudesHorasExtra
                .Where(s => s.IdEmpleado == idEmpleado)
                .ToListAsync();

            return solicitudes.Select(MapToDTO);
        }

        public async Task<bool> ActualizarSolicitudAsync(SolicitudHorasExtraDTO solicitudDTO)
        {
            var solicitud = await _context.SolicitudesHorasExtra.FindAsync(solicitudDTO.Id);
            if (solicitud == null || solicitud.Estado != "Pendiente")
            {
                return false;
            }

            solicitud.CantidadHoras = solicitudDTO.CantidadHoras;
            solicitud.FechaTrabajo = solicitudDTO.FechaTrabajo;
            solicitud.FechaModificacion = DateTime.Now;
            solicitud.ModificadoPor = solicitudDTO.ModificadoPor;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarSolicitudAsync(int id)
        {
            var solicitud = await _context.SolicitudesHorasExtra.FindAsync(id);
            if (solicitud == null || solicitud.Estado != "Pendiente")
            {
                return false;
            }

            _context.SolicitudesHorasExtra.Remove(solicitud);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AprobarSolicitudAsync(int id)
        {
            var solicitud = await _context.SolicitudesHorasExtra.FindAsync(id);
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
            var solicitud = await _context.SolicitudesHorasExtra.FindAsync(id);
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

        public async Task<IEnumerable<SolicitudHorasExtraDTO>> ObtenerTodasSolicitudesAsync()
        {
            var solicitudes = await _context.SolicitudesHorasExtra.ToListAsync();
            return solicitudes.Select(MapToDTO);
        }

        // Método de mapeo para convertir la entidad a DTO
        private SolicitudHorasExtraDTO MapToDTO(SolicitudHorasExtra solicitud)
        {
            return new SolicitudHorasExtraDTO
            {
                Id = solicitud.Id,
                IdEmpleado = solicitud.IdEmpleado,
                CantidadHoras = solicitud.CantidadHoras,
                FechaSolicitud = solicitud.FechaSolicitud,
                FechaTrabajo = solicitud.FechaTrabajo,
                Estado = solicitud.Estado,
                FechaCambioEstado = solicitud.FechaCambioEstado,
                MotivoRechazo = solicitud.MotivoRechazo,
                ModificadoPor = solicitud.ModificadoPor
            };
        }
    }
}
