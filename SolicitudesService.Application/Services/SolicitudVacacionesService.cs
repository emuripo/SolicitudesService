using SolicitudesService.Application.DTO;
using SolicitudesService.Application.Interfaces;
using SolicitudesService.Infrastructure.Data;
using SolicitudesService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolicitudesService.Application.Services
{
    public class SolicitudVacacionesService : ISolicitudVacacionesService
    {
        private readonly SolicitudesServiceDbContext _context;
        private readonly ILogger<SolicitudVacacionesService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public SolicitudVacacionesService(SolicitudesServiceDbContext context, ILogger<SolicitudVacacionesService> logger, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
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
            if (!await ValidarSolicitudVacaciones(solicitudDTO))
            {
                _logger.LogWarning("La solicitud de vacaciones no es válida. El empleado no tiene suficientes días disponibles.");
                throw new EmployeeValidationException("No tienes suficientes días de vacaciones disponibles.");
            }

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

            if (!await ValidarSolicitudVacaciones(solicitudDTO))
            {
                _logger.LogWarning("La actualización de la solicitud de vacaciones no es válida. El empleado no tiene suficientes días disponibles.");
                throw new EmployeeValidationException("No tienes suficientes días de vacaciones disponibles.");
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

        // Validación de la solicitud de vacaciones
        private async Task<bool> ValidarSolicitudVacaciones(SolicitudVacacionesDTO solicitudDTO)
        {
            var diasSolicitados = (solicitudDTO.FechaFin - solicitudDTO.FechaInicio).Days + 1;

            // Llamar a FuncionarioService para obtener la fecha de contratación del empleado
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync($"http://localhost:8085/api/Empleado/{solicitudDTO.IdEmpleado}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("No se pudo obtener la información del empleado con ID {IdEmpleado}.", solicitudDTO.IdEmpleado);
                throw new EmployeeNotFoundException($"Error al obtener información del empleado con ID {solicitudDTO.IdEmpleado}.");
            }

            var empleadoJson = await response.Content.ReadAsStringAsync();
            var empleado = JsonSerializer.Deserialize<dynamic>(empleadoJson);

            // Obtener la fecha de contratación del empleado
            DateTime fechaContratacion = empleado?.infoContratoFuncionario?.fechaContratacion;

            if (fechaContratacion == null)
            {
                _logger.LogError("La fecha de contratación no se pudo obtener para el empleado con ID {IdEmpleado}.", solicitudDTO.IdEmpleado);
                throw new EmployeeValidationException($"Error al obtener la fecha de contratación del empleado con ID {solicitudDTO.IdEmpleado}.");
            }

            // Calcular los días acumulados
            var diasAcumulados = CalcularDiasVacaciones(fechaContratacion);
            var diasTomados = await ObtenerDiasTomados(solicitudDTO.IdEmpleado);

            if (diasAcumulados - diasTomados < diasSolicitados)
            {
                _logger.LogWarning("El empleado con ID {IdEmpleado} no tiene suficientes días disponibles.", solicitudDTO.IdEmpleado);
                return false;
            }

            return true;
        }

        private int CalcularDiasVacaciones(DateTime fechaContratacion)
        {
            var mesesTrabajados = (DateTime.Now.Year - fechaContratacion.Year) * 12 + DateTime.Now.Month - fechaContratacion.Month;
            return mesesTrabajados; // 1 día de vacaciones por cada mes trabajado
        }

        private async Task<int> ObtenerDiasTomados(int idEmpleado)
        {
            var solicitudesAprobadas = await _context.SolicitudesVacaciones
                .Where(s => s.IdEmpleado == idEmpleado && s.EstaAprobada)
                .ToListAsync();

            return solicitudesAprobadas.Sum(s => (s.FechaFin - s.FechaInicio).Days + 1);
        }
    }

    // Excepciones personalizadas
    public class EmployeeNotFoundException : Exception
    {
        public EmployeeNotFoundException(string message) : base(message) { }
    }

    public class EmployeeValidationException : Exception
    {
        public EmployeeValidationException(string message) : base(message) { }
    }
}
