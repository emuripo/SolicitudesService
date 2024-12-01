using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using SolicitudesService.Application.DTO;
using SolicitudesService.Core.Entities;
using SolicitudesService.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using SolicitudesService.Infrastructure.Data;

namespace SolicitudesService.Services
{
    public class SolicitudHorasExtraService : ISolicitudHorasExtraService
    {
        private readonly SolicitudesServiceDbContext _context;
        private readonly ILogger<SolicitudHorasExtraService> _logger;
        private readonly HttpClient _httpClient;

        public SolicitudHorasExtraService(SolicitudesServiceDbContext context, ILogger<SolicitudHorasExtraService> logger, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("FuncionarioService");
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

        public async Task<HorasExtraDTO> ObtenerSaldoHorasExtraAsync(int idEmpleado)
        {
            // Obtener todas las solicitudes aprobadas del empleado.
            var solicitudesAprobadas = await _context.SolicitudesHorasExtra
                .Where(s => s.IdEmpleado == idEmpleado && s.Estado == "Aprobada")
                .ToListAsync();

            // Calcular horas trabajadas hoy, esta semana y este mes.
            var hoy = DateTime.Now.Date;
            var inicioSemana = hoy.AddDays(-(int)hoy.DayOfWeek); // Semana comienza el domingo.
            var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);

            var horasHoy = solicitudesAprobadas.Where(s => s.FechaTrabajo.Date == hoy).Sum(s => s.CantidadHoras);
            var horasSemana = solicitudesAprobadas.Where(s => s.FechaTrabajo.Date >= inicioSemana).Sum(s => s.CantidadHoras);
            var horasMes = solicitudesAprobadas.Where(s => s.FechaTrabajo.Date >= inicioMes).Sum(s => s.CantidadHoras);

            // Crear y devolver el objeto HorasExtraDTO con los datos acumulados.
            return new HorasExtraDTO
            {
                HorasExtrasTrabajadasHoy = horasHoy,
                HorasExtrasTrabajadasSemana = horasSemana,
                HorasExtrasTrabajadasMes = horasMes,
                SolicitudesAprobadas = solicitudesAprobadas.Select(MapToDTO).ToList()
            };
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
                _logger.LogWarning($"No se pudo aprobar la solicitud con ID {id}. Solicitud no encontrada o no está en estado 'Pendiente'.");
                return false;
            }

            solicitud.Estado = "Aprobada";
            solicitud.FechaCambioEstado = DateTime.Now;

            // Consultar saldo actual de horas extra
            var horasResponse = await _httpClient.GetAsync($"/api/Empleado/{solicitud.IdEmpleado}/horasextra/saldo");
            if (!horasResponse.IsSuccessStatusCode)
            {
                _logger.LogError($"Error al consultar las horas extra del empleado {solicitud.IdEmpleado}. Estado: {horasResponse.StatusCode}");
                throw new Exception("No se pudo obtener las horas extra del empleado.");
            }

            var horasExtraActuales = await horasResponse.Content.ReadFromJsonAsync<HorasExtraDTO>();
            if (horasExtraActuales == null)
            {
                throw new Exception("Datos de horas extra no encontrados.");
            }

            var nuevasHorasHoy = horasExtraActuales.HorasExtrasTrabajadasHoy + solicitud.CantidadHoras;
            var nuevasHorasSemana = horasExtraActuales.HorasExtrasTrabajadasSemana + solicitud.CantidadHoras;
            var nuevasHorasMes = horasExtraActuales.HorasExtrasTrabajadasMes + solicitud.CantidadHoras;

            // Validar límites
            if (nuevasHorasHoy > 4 || nuevasHorasSemana > 24 || nuevasHorasMes > 96)
            {
                _logger.LogWarning($"Solicitud {id} excede los límites permitidos de horas extra.");
                return false;
            }

            // Actualizar en FuncionarioService
            var updateResponse = await _httpClient.PutAsJsonAsync($"/api/Empleado/{solicitud.IdEmpleado}/horasextra/actualizar", new
            {
                HorasExtrasTrabajadasHoy = nuevasHorasHoy,
                HorasExtrasTrabajadasSemana = nuevasHorasSemana,
                HorasExtrasTrabajadasMes = nuevasHorasMes
            });

            if (!updateResponse.IsSuccessStatusCode)
            {
                _logger.LogError($"Error al actualizar las horas extra del empleado {solicitud.IdEmpleado}. Estado: {updateResponse.StatusCode}");
                throw new Exception("No se pudo actualizar las horas extra del empleado.");
            }

            // Guardar cambios locales después de una actualización exitosa en FuncionarioService
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Solicitud {id} aprobada exitosamente para el empleado {solicitud.IdEmpleado}.");

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
