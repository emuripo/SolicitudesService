using SolicitudesService.Application.DTO;
using SolicitudesService.Application.Interfaces;
using SolicitudesService.Infrastructure.Data;
using SolicitudesService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SolicitudesService.Application.Services
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

        public async Task<SolicitudPersonalDTO> CreateSolicitudPersonalAsync(SolicitudPersonalDTO solicitudDTO)
        {
            var solicitud = new SolicitudPersonal
            {
                IdEmpleado = solicitudDTO.IdEmpleado,
                Motivo = solicitudDTO.Motivo,
                FechaSolicitud = solicitudDTO.FechaSolicitud,
                EstaAprobada = solicitudDTO.EstaAprobada,
                FechaAprobacion = solicitudDTO.FechaAprobacion
            };

            _context.SolicitudesPersonales.Add(solicitud);
            await _context.SaveChangesAsync();

            solicitudDTO.IdSolicitudPersonal = solicitud.IdSolicitudPersonal;

            _logger.LogInformation("Solicitud personal creada exitosamente con ID {IdSolicitudPersonal}", solicitud.IdSolicitudPersonal);

            return solicitudDTO;
        }

        public async Task<SolicitudPersonalDTO> UpdateSolicitudPersonalAsync(int id, SolicitudPersonalDTO solicitudDTO)
        {
            var solicitud = await _context.SolicitudesPersonales.FindAsync(id);

            if (solicitud == null)
            {
                _logger.LogWarning("Solicitud personal con ID {id} no fue encontrada. Verifica que el ID proporcionado sea correcto.", id);
                return null;
            }

            solicitud.Motivo = solicitudDTO.Motivo;
            solicitud.FechaSolicitud = solicitudDTO.FechaSolicitud;
            solicitud.EstaAprobada = solicitudDTO.EstaAprobada;
            solicitud.FechaAprobacion = solicitudDTO.FechaAprobacion;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Solicitud personal con ID {IdSolicitudPersonal} actualizada exitosamente.", solicitud.IdSolicitudPersonal);

            return solicitudDTO;
        }

        public async Task<bool> DeleteSolicitudPersonalAsync(int id)
        {
            var solicitud = await _context.SolicitudesPersonales.FindAsync(id);

            if (solicitud == null)
            {
                _logger.LogWarning("Solicitud personal con ID {id} no fue encontrada. Verifica que el ID proporcionado sea correcto.", id);
                return false;
            }

            _context.SolicitudesPersonales.Remove(solicitud);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Solicitud personal con ID {IdSolicitudPersonal} eliminada exitosamente.", solicitud.IdSolicitudPersonal);

            return true;
        }

        public async Task<SolicitudPersonalDTO?> GetSolicitudPersonalByIdAsync(int id)
        {
            var solicitud = await _context.SolicitudesPersonales.FindAsync(id);

            if (solicitud == null)
            {
                _logger.LogWarning("Solicitud personal con ID {id} no fue encontrada. Verifica que el ID proporcionado sea correcto.", id);
                return null;
            }

            return new SolicitudPersonalDTO
            {
                IdSolicitudPersonal = solicitud.IdSolicitudPersonal,
                IdEmpleado = solicitud.IdEmpleado,
                Motivo = solicitud.Motivo,
                FechaSolicitud = solicitud.FechaSolicitud,
                EstaAprobada = solicitud.EstaAprobada,
                FechaAprobacion = solicitud.FechaAprobacion
            };
        }

        public async Task<IEnumerable<SolicitudPersonalDTO>> GetAllSolicitudesPersonalesAsync()
        {
            var solicitudes = await _context.SolicitudesPersonales.ToListAsync();

            _logger.LogInformation("Se obtuvieron {Count} solicitudes personales.", solicitudes.Count);

            var solicitudesDTO = solicitudes.Select(s => new SolicitudPersonalDTO
            {
                IdSolicitudPersonal = s.IdSolicitudPersonal,
                IdEmpleado = s.IdEmpleado,
                Motivo = s.Motivo,
                FechaSolicitud = s.FechaSolicitud,
                EstaAprobada = s.EstaAprobada,
                FechaAprobacion = s.FechaAprobacion
            }).ToList();

            return solicitudesDTO;
        }

        public Task<IEnumerable<SolicitudPersonalDTO>> GetAllSolicitudes()
        {
            throw new NotImplementedException();
        }

        public Task<SolicitudPersonalDTO> GetSolicitudById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<SolicitudPersonalDTO> CreateSolicitud(SolicitudPersonalDTO solicitud)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateSolicitud(int id, SolicitudPersonalDTO solicitud)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteSolicitud(int id)
        {
            throw new NotImplementedException();
        }
    }
}
