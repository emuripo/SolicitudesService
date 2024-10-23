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
    public class SolicitudHorasExtraService : ISolicitudHorasExtraService
    {
        private readonly SolicitudesServiceDbContext _context;
        private readonly ILogger<SolicitudHorasExtraService> _logger;

        public SolicitudHorasExtraService(SolicitudesServiceDbContext context, ILogger<SolicitudHorasExtraService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<SolicitudHorasExtraDTO> CreateSolicitudHorasExtraAsync(SolicitudHorasExtraDTO solicitudDTO)
        {
            // Validar la solicitud
            if (!await ValidarSolicitudHorasExtra(solicitudDTO))
            {
                _logger.LogWarning("Solicitud no válida: el empleado {IdEmpleado} excedió el límite de horas extra permitidas.", solicitudDTO.IdEmpleado);
                return null;
            }

            var solicitud = new SolicitudHorasExtra
            {
                IdEmpleado = solicitudDTO.IdEmpleado,
                CantidadHoras = solicitudDTO.CantidadHoras,
                FechaSolicitud = solicitudDTO.FechaSolicitud,
                EstaAprobada = solicitudDTO.EstaAprobada
            };

            _context.SolicitudesHorasExtra.Add(solicitud);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Solicitud de horas extra creada exitosamente con ID {IdSolicitudHorasExtra}", solicitud.IdSolicitudHorasExtra);

            solicitudDTO.IdSolicitudHorasExtra = solicitud.IdSolicitudHorasExtra;

            return solicitudDTO;
        }

        public async Task<SolicitudHorasExtraDTO> UpdateSolicitudHorasExtraAsync(int id, SolicitudHorasExtraDTO solicitudDTO)
        {
            var solicitud = await _context.SolicitudesHorasExtra.FindAsync(id);

            if (solicitud == null)
            {
                _logger.LogWarning("Solicitud con ID {id} no fue encontrada. Verifica que el ID proporcionado sea correcto.", id);
                return null;
            }

            // Validar la solicitud
            if (!await ValidarSolicitudHorasExtra(solicitudDTO))
            {
                _logger.LogWarning("Solicitud no válida: el empleado {IdEmpleado} excedió el límite de horas extra permitidas.", solicitudDTO.IdEmpleado);
                return null;
            }

            solicitud.CantidadHoras = solicitudDTO.CantidadHoras;
            solicitud.FechaSolicitud = solicitudDTO.FechaSolicitud;
            solicitud.EstaAprobada = solicitudDTO.EstaAprobada;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Solicitud de horas extra con ID {IdSolicitudHorasExtra} actualizada exitosamente.", solicitud.IdSolicitudHorasExtra);

            return solicitudDTO;
        }

        public async Task<bool> DeleteSolicitudHorasExtraAsync(int id)
        {
            var solicitud = await _context.SolicitudesHorasExtra.FindAsync(id);

            if (solicitud == null)
            {
                _logger.LogWarning("Solicitud con ID {id} no fue encontrada. Verifica que el ID proporcionado sea correcto.", id);
                return false;
            }

            _context.SolicitudesHorasExtra.Remove(solicitud);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Solicitud de horas extra con ID {IdSolicitudHorasExtra} eliminada exitosamente.", solicitud.IdSolicitudHorasExtra);

            return true;
        }

        public async Task<SolicitudHorasExtraDTO?> GetSolicitudHorasExtraByIdAsync(int id)
        {
            var solicitud = await _context.SolicitudesHorasExtra.FindAsync(id);

            if (solicitud == null)
            {
                _logger.LogWarning("Solicitud con ID {id} no fue encontrada. Verifica que el ID proporcionado sea correcto.", id);
                return null;
            }

            return new SolicitudHorasExtraDTO
            {
                IdSolicitudHorasExtra = solicitud.IdSolicitudHorasExtra,
                IdEmpleado = solicitud.IdEmpleado,
                CantidadHoras = solicitud.CantidadHoras,
                FechaSolicitud = solicitud.FechaSolicitud,
                EstaAprobada = solicitud.EstaAprobada
            };
        }

        public async Task<IEnumerable<SolicitudHorasExtraDTO>> GetAllSolicitudesHorasExtraAsync()
        {
            var solicitudes = await _context.SolicitudesHorasExtra.ToListAsync();

            _logger.LogInformation("Se obtuvieron {Count} solicitudes de horas extra.", solicitudes.Count);

            var solicitudesDTO = solicitudes.Select(s => new SolicitudHorasExtraDTO
            {
                IdSolicitudHorasExtra = s.IdSolicitudHorasExtra,
                IdEmpleado = s.IdEmpleado,
                CantidadHoras = s.CantidadHoras,
                FechaSolicitud = s.FechaSolicitud,
                EstaAprobada = s.EstaAprobada
            }).ToList();

            return solicitudesDTO;
        }
        private async Task<bool> ValidarSolicitudHorasExtra(SolicitudHorasExtraDTO solicitudDTO)
        {
            int jornadaOrdinaria = 8;

            if (solicitudDTO.CantidadHoras + jornadaOrdinaria > 12)
            {
                _logger.LogWarning("La solicitud de horas extra con ID {IdEmpleado} excede el límite de horas permitido.", solicitudDTO.IdEmpleado);
                return false;
            }

            return true;
        }
    }
}
