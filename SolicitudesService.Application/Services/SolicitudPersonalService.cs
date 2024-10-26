using SolicitudesService.Application.DTO;
using SolicitudesService.Application.Interfaces;
using SolicitudesService.Infrastructure.Data;
using SolicitudesService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<SolicitudPersonalDTO> CreateSolicitud(SolicitudPersonalDTO solicitudDTO)
        {
            var solicitud = new SolicitudPersonal
            {
                IdEmpleado = solicitudDTO.IdEmpleado,
                Descripcion = solicitudDTO.Descripcion,
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

        public async Task<bool> UpdateSolicitud(int id, SolicitudPersonalDTO solicitudDTO)
        {
            var solicitud = await _context.SolicitudesPersonales.FindAsync(id);

            if (solicitud == null)
            {
                _logger.LogWarning("Solicitud personal con ID {id} no fue encontrada. Verifica que el ID proporcionado sea correcto.", id);
                return false; // Aquí debes devolver false si no se encontró la solicitud
            }

            solicitud.Descripcion = solicitudDTO.Descripcion;
            solicitud.FechaSolicitud = solicitudDTO.FechaSolicitud;
            solicitud.EstaAprobada = solicitudDTO.EstaAprobada;
            solicitud.FechaAprobacion = solicitudDTO.FechaAprobacion;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Solicitud personal con ID {IdSolicitudPersonal} actualizada exitosamente.", solicitud.IdSolicitudPersonal);

            return true; // Aquí devuelves true si la solicitud fue actualizada correctamente
        }


        public async Task<bool> DeleteSolicitud(int id)
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

        public async Task<SolicitudPersonalDTO?> GetSolicitudById(int id)
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
                Descripcion = solicitud.Descripcion,
                FechaSolicitud = solicitud.FechaSolicitud,
                EstaAprobada = solicitud.EstaAprobada,
                FechaAprobacion = solicitud.FechaAprobacion
            };
        }

        public async Task<IEnumerable<SolicitudPersonalDTO>> GetAllSolicitudes()
        {
            var solicitudes = await _context.SolicitudesPersonales.ToListAsync();

            _logger.LogInformation("Se obtuvieron {Count} solicitudes personales.", solicitudes.Count);

            return solicitudes.Select(s => new SolicitudPersonalDTO
            {
                IdSolicitudPersonal = s.IdSolicitudPersonal,
                IdEmpleado = s.IdEmpleado,
                Descripcion = s.Descripcion,
                FechaSolicitud = s.FechaSolicitud,
                EstaAprobada = s.EstaAprobada,
                FechaAprobacion = s.FechaAprobacion
            }).ToList();
        }
    }
}
