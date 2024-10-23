using SolicitudesService.Application.DTO;
using SolicitudesService.Application.Interfaces;
using SolicitudesService.Infrastructure.Data;
using SolicitudesService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SolicitudesService.Application.Services
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

        public async Task<IEnumerable<SolicitudVacacionesDTO>> GetAllSolicitudes()
        {
            var solicitudes = await _context.SolicitudesVacaciones.ToListAsync();

            _logger.LogInformation("Se obtuvieron {Count} solicitudes de vacaciones.", solicitudes.Count);

            return solicitudes.Select(s => new SolicitudVacacionesDTO
            {
                IdSolicitudVacaciones = s.IdSolicitudVacaciones,
                IdEmpleado = s.IdEmpleado,
                DiasSolicitados = (s.FechaFin - s.FechaInicio).Days + 1,
                FechaInicio = s.FechaInicio,
                FechaFin = s.FechaFin,
                FechaSolicitud = s.FechaSolicitud,
                EstaAprobada = s.EstaAprobada,
                FechaAprobacion = s.FechaAprobacion
            }).ToList();
        }

        public async Task<SolicitudVacacionesDTO> GetSolicitudById(int id)
        {
            var solicitud = await _context.SolicitudesVacaciones.FindAsync(id);

            if (solicitud == null)
            {
                _logger.LogWarning("Solicitud de vacaciones con ID {Id} no fue encontrada.", id);
                return null;
            }

            _logger.LogInformation("Solicitud de vacaciones con ID {Id} fue obtenida exitosamente.", id);

            return new SolicitudVacacionesDTO
            {
                IdSolicitudVacaciones = solicitud.IdSolicitudVacaciones,
                IdEmpleado = solicitud.IdEmpleado,
                DiasSolicitados = (solicitud.FechaFin - solicitud.FechaInicio).Days + 1,
                FechaInicio = solicitud.FechaInicio,
                FechaFin = solicitud.FechaFin,
                FechaSolicitud = solicitud.FechaSolicitud,
                EstaAprobada = solicitud.EstaAprobada,
                FechaAprobacion = solicitud.FechaAprobacion
            };
        }

        public async Task<SolicitudVacacionesDTO> CreateSolicitud(SolicitudVacacionesDTO solicitudDTO)
        {
            var solicitud = new SolicitudVacaciones
            {
                IdEmpleado = solicitudDTO.IdEmpleado,
                FechaInicio = solicitudDTO.FechaInicio,
                FechaFin = solicitudDTO.FechaFin,
                FechaSolicitud = solicitudDTO.FechaSolicitud,
                EstaAprobada = solicitudDTO.EstaAprobada,
                FechaAprobacion = solicitudDTO.FechaAprobacion
            };

            _context.SolicitudesVacaciones.Add(solicitud);
            await _context.SaveChangesAsync();

            solicitudDTO.IdSolicitudVacaciones = solicitud.IdSolicitudVacaciones;

            _logger.LogInformation("Solicitud de vacaciones creada exitosamente con ID {Id}.", solicitud.IdSolicitudVacaciones);

            return solicitudDTO;
        }

        public async Task<bool> UpdateSolicitud(int id, SolicitudVacacionesDTO solicitudDTO)
        {
            var solicitud = await _context.SolicitudesVacaciones.FindAsync(id);

            if (solicitud == null)
            {
                _logger.LogWarning("Solicitud de vacaciones con ID {Id} no fue encontrada.", id);
                return false;
            }

            solicitud.IdEmpleado = solicitudDTO.IdEmpleado;
            solicitud.FechaInicio = solicitudDTO.FechaInicio;
            solicitud.FechaFin = solicitudDTO.FechaFin;
            solicitud.FechaSolicitud = solicitudDTO.FechaSolicitud;
            solicitud.EstaAprobada = solicitudDTO.EstaAprobada;
            solicitud.FechaAprobacion = solicitudDTO.FechaAprobacion;

            _context.SolicitudesVacaciones.Update(solicitud);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Solicitud de vacaciones con ID {Id} actualizada exitosamente.", id);

            return true;
        }

        public async Task<bool> DeleteSolicitud(int id)
        {
            var solicitud = await _context.SolicitudesVacaciones.FindAsync(id);

            if (solicitud == null)
            {
                _logger.LogWarning("Solicitud de vacaciones con ID {Id} no fue encontrada.", id);
                return false;
            }

            _context.SolicitudesVacaciones.Remove(solicitud);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Solicitud de vacaciones con ID {Id} eliminada exitosamente.", id);

            return true;
        }
    }
}
