using SolicitudesService.Application.DTO;
using SolicitudesService.Application.Interfaces;
using SolicitudesService.Infrastructure.Data;
using SolicitudesService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SolicitudesService.Application.Services
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

        public async Task<IEnumerable<SolicitudDocumentoDTO>> GetAllSolicitudes()
        {
            var solicitudes = await _context.SolicitudesDocumentos.ToListAsync();

            _logger.LogInformation("Se obtuvieron {Count} solicitudes de documentos.", solicitudes.Count);

            return solicitudes.Select(s => new SolicitudDocumentoDTO
            {
                IdSolicitudDocumento = s.IdSolicitudDocumento,
                IdEmpleado = s.IdEmpleado,
                Descripcion = s.Descripcion,
                FechaSolicitud = s.FechaSolicitud,
                EstaAprobada = s.EstaAprobada
            }).ToList();
        }

        public async Task<SolicitudDocumentoDTO> GetSolicitudById(int id)
        {
            var solicitud = await _context.SolicitudesDocumentos.FindAsync(id);

            if (solicitud == null)
            {
                _logger.LogWarning("Solicitud de documento con ID {Id} no fue encontrada.", id);
                return null;
            }

            _logger.LogInformation("Solicitud de documento con ID {Id} fue obtenida exitosamente.", id);

            return new SolicitudDocumentoDTO
            {
                IdSolicitudDocumento = solicitud.IdSolicitudDocumento,
                IdEmpleado = solicitud.IdEmpleado,
                Descripcion = solicitud.Descripcion,
                FechaSolicitud = solicitud.FechaSolicitud,
                EstaAprobada = solicitud.EstaAprobada
            };
        }

        public async Task<SolicitudDocumentoDTO> CreateSolicitud(SolicitudDocumentoDTO solicitudDTO)
        {
            var solicitud = new SolicitudDocumentos
            {
                IdEmpleado = solicitudDTO.IdEmpleado,
                Descripcion = solicitudDTO.Descripcion,
                FechaSolicitud = solicitudDTO.FechaSolicitud,
                EstaAprobada = solicitudDTO.EstaAprobada
            };

            _context.SolicitudesDocumentos.Add(solicitud);
            await _context.SaveChangesAsync();

            solicitudDTO.IdSolicitudDocumento = solicitud.IdSolicitudDocumento;

            _logger.LogInformation("Solicitud de documento creada exitosamente con ID {Id}.", solicitud.IdSolicitudDocumento);

            return solicitudDTO;
        }

        public async Task<bool> UpdateSolicitud(int id, SolicitudDocumentoDTO solicitudDTO)
        {
            var solicitud = await _context.SolicitudesDocumentos.FindAsync(id);

            if (solicitud == null)
            {
                _logger.LogWarning("Solicitud de documento con ID {Id} no fue encontrada.", id);
                return false;
            }

            solicitud.IdEmpleado = solicitudDTO.IdEmpleado;
            solicitud.Descripcion = solicitudDTO.Descripcion;
            solicitud.FechaSolicitud = solicitudDTO.FechaSolicitud;
            solicitud.EstaAprobada = solicitudDTO.EstaAprobada;

            _context.SolicitudesDocumentos.Update(solicitud);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Solicitud de documento con ID {Id} actualizada exitosamente.", id);

            return true;
        }

        public async Task<bool> DeleteSolicitud(int id)
        {
            var solicitud = await _context.SolicitudesDocumentos.FindAsync(id);

            if (solicitud == null)
            {
                _logger.LogWarning("Solicitud de documento con ID {Id} no fue encontrada.", id);
                return false;
            }

            _context.SolicitudesDocumentos.Remove(solicitud);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Solicitud de documento con ID {Id} eliminada exitosamente.", id);

            return true;
        }
    }
}
