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
    public class SolicitudDocumentoService : ISolicitudDocumentoService
    {
        private readonly SolicitudesServiceDbContext _context;
        private readonly ILogger<SolicitudDocumentoService> _logger;

        public SolicitudDocumentoService(SolicitudesServiceDbContext context, ILogger<SolicitudDocumentoService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<SolicitudDocumentoDTO> CrearSolicitudAsync(SolicitudDocumentoDTO solicitudDTO)
        {
            var solicitud = new SolicitudDocumentos
            {
                IdEmpleado = solicitudDTO.IdEmpleado,
                TipoDocumento = solicitudDTO.TipoDocumento,
                Descripcion = solicitudDTO.Descripcion,
                FechaSolicitud = DateTime.Now,
                Estado = "Pendiente",
                CreadoPor = solicitudDTO.ModificadoPor ?? "UsuarioDesconocido"
            };

            _context.SolicitudDocumentos.Add(solicitud);
            await _context.SaveChangesAsync();

            solicitudDTO.Id = solicitud.Id;
            return solicitudDTO;
        }

        public async Task<SolicitudDocumentoDTO?> ObtenerSolicitudPorIdAsync(int id)
        {
            var solicitud = await _context.SolicitudDocumentos.FindAsync(id);
            return solicitud == null ? null : MapToDTO(solicitud);
        }

        public async Task<IEnumerable<SolicitudDocumentoDTO>> ObtenerSolicitudesPorEmpleadoAsync(int idEmpleado)
        {
            var solicitudes = await _context.SolicitudDocumentos
                .Where(s => s.IdEmpleado == idEmpleado)
                .ToListAsync();

            return solicitudes.Select(MapToDTO);
        }

        public async Task<bool> ActualizarSolicitudAsync(SolicitudDocumentoDTO solicitudDTO)
        {
            var solicitud = await _context.SolicitudDocumentos.FindAsync(solicitudDTO.Id);
            if (solicitud == null || solicitud.Estado != "Pendiente")
            {
                return false;
            }

            solicitud.TipoDocumento = solicitudDTO.TipoDocumento;
            solicitud.Descripcion = solicitudDTO.Descripcion;
            solicitud.FechaModificacion = DateTime.Now;
            solicitud.ModificadoPor = solicitudDTO.ModificadoPor;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarSolicitudAsync(int id)
        {
            var solicitud = await _context.SolicitudDocumentos.FindAsync(id);
            if (solicitud == null || solicitud.Estado != "Pendiente")
            {
                return false;
            }

            _context.SolicitudDocumentos.Remove(solicitud);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AprobarSolicitudAsync(int id)
        {
            var solicitud = await _context.SolicitudDocumentos.FindAsync(id);
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
            var solicitud = await _context.SolicitudDocumentos.FindAsync(id);
            if (solicitud == null || solicitud.Estado != "Pendiente")
            {
                return false;
            }

            solicitud.Estado = "Rechazada";
            solicitud.FechaCambioEstado = DateTime.Now;
            solicitud.MotivoRechazo = motivoRechazo; // Guardar el motivo de rechazo

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<SolicitudDocumentoDTO>> ObtenerTodasSolicitudesAsync()
        {
            var solicitudes = await _context.SolicitudDocumentos.ToListAsync();
            return solicitudes.Select(MapToDTO);
        }

        // Método de mapeo para convertir la entidad a DTO
        private SolicitudDocumentoDTO MapToDTO(SolicitudDocumentos solicitud)
        {
            return new SolicitudDocumentoDTO
            {
                Id = solicitud.Id,
                IdEmpleado = solicitud.IdEmpleado,
                TipoDocumento = solicitud.TipoDocumento,
                Descripcion = solicitud.Descripcion,
                FechaSolicitud = solicitud.FechaSolicitud,
                Estado = solicitud.Estado,
                FechaCambioEstado = solicitud.FechaCambioEstado,
                MotivoRechazo = solicitud.MotivoRechazo, 
                ModificadoPor = solicitud.ModificadoPor
            };
        }
    }
}
