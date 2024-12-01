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
    public class SolicitudVacacionesService : ISolicitudVacacionesService
    {
        private readonly SolicitudesServiceDbContext _context;
        private readonly ILogger<SolicitudVacacionesService> _logger;
        private readonly HttpClient _httpClient;

        public SolicitudVacacionesService(SolicitudesServiceDbContext context, ILogger<SolicitudVacacionesService> logger, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("FuncionarioService");
        }

        public async Task<SolicitudVacacionesDTO> CrearSolicitudAsync(SolicitudVacacionesDTO solicitudDTO)
        {
            var solicitud = new SolicitudVacaciones
            {
                IdEmpleado = solicitudDTO.IdEmpleado,
                DiasSolicitados = solicitudDTO.DiasSolicitados,
                FechaInicio = solicitudDTO.FechaInicio,
                FechaFin = solicitudDTO.FechaFin,
                FechaSolicitud = DateTime.Now,
                Estado = "Pendiente",
                CreadoPor = solicitudDTO.ModificadoPor ?? "UsuarioDesconocido"
            };

            _context.SolicitudesVacaciones.Add(solicitud);
            await _context.SaveChangesAsync();

            solicitudDTO.Id = solicitud.Id;
            return solicitudDTO;
        }

        public async Task<SolicitudVacacionesDTO?> ObtenerSolicitudPorIdAsync(int id)
        {
            var solicitud = await _context.SolicitudesVacaciones.FindAsync(id);
            return solicitud == null ? null : MapToDTO(solicitud);
        }

        public async Task<IEnumerable<SolicitudVacacionesDTO>> ObtenerSolicitudesPorEmpleadoAsync(int idEmpleado)
        {
            var solicitudes = await _context.SolicitudesVacaciones
                .Where(s => s.IdEmpleado == idEmpleado)
                .ToListAsync();

            return solicitudes.Select(MapToDTO);
        }

        public async Task<bool> ActualizarSolicitudAsync(SolicitudVacacionesDTO solicitudDTO)
        {
            var solicitud = await _context.SolicitudesVacaciones.FindAsync(solicitudDTO.Id);
            if (solicitud == null || solicitud.Estado != "Pendiente")
            {
                return false;
            }

            solicitud.DiasSolicitados = solicitudDTO.DiasSolicitados;
            solicitud.FechaInicio = solicitudDTO.FechaInicio;
            solicitud.FechaFin = solicitudDTO.FechaFin;
            solicitud.FechaModificacion = DateTime.Now;
            solicitud.ModificadoPor = solicitudDTO.ModificadoPor;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarSolicitudAsync(int id)
        {
            var solicitud = await _context.SolicitudesVacaciones.FindAsync(id);
            if (solicitud == null || solicitud.Estado != "Pendiente")
            {
                return false;
            }

            _context.SolicitudesVacaciones.Remove(solicitud);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AprobarSolicitudAsync(int id)
        {
            var solicitud = await _context.SolicitudesVacaciones.FindAsync(id);
            if (solicitud == null || solicitud.Estado != "Pendiente")
            {
                return false;
            }

            solicitud.Estado = "Aprobada";
            solicitud.FechaCambioEstado = DateTime.Now;

            // Obtener el saldo de vacaciones actual del empleado
            var saldoResponse = await _httpClient.GetAsync($"/api/Empleado/{solicitud.IdEmpleado}/vacaciones/saldo");
            if (!saldoResponse.IsSuccessStatusCode)
            {
                _logger.LogError("Error al consultar el saldo de vacaciones del empleado.");
                throw new Exception("No se pudo obtener el saldo de vacaciones.");
            }

            var saldoVacaciones = await saldoResponse.Content.ReadFromJsonAsync<VacacionesDTO>();
            if (saldoVacaciones == null)
            {
                throw new Exception("No se pudo obtener el saldo de vacaciones del empleado.");
            }

            // Calcular el nuevo saldo de vacaciones
            var nuevosDiasDisponibles = saldoVacaciones.DiasDisponibles - solicitud.DiasSolicitados;
            var nuevosDiasGozados = saldoVacaciones.DiasGozados + solicitud.DiasSolicitados;

            if (nuevosDiasDisponibles < 0)
            {
                return false; // Validación para que no exceda los días disponibles
            }

            // Actualizar los días disponibles y gozados a través del endpoint de actualización
            var updateResponse = await _httpClient.PutAsJsonAsync($"/api/Empleado/{solicitud.IdEmpleado}/vacaciones/actualizar", new
            {
                DiasDisponibles = nuevosDiasDisponibles,
                DiasGozados = nuevosDiasGozados
            });

            if (!updateResponse.IsSuccessStatusCode)
            {
                _logger.LogError("Error al actualizar el saldo de vacaciones en FuncionarioService.");
                throw new Exception("No se pudo actualizar el saldo de vacaciones.");
            }

            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> RechazarSolicitudAsync(int id, string motivoRechazo)
        {
            var solicitud = await _context.SolicitudesVacaciones.FindAsync(id);
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

        public async Task<IEnumerable<SolicitudVacacionesDTO>> ObtenerTodasSolicitudesAsync()
        {
            var solicitudes = await _context.SolicitudesVacaciones.ToListAsync();
            return solicitudes.Select(MapToDTO);
        }

        public async Task<object> ObtenerResumenVacacionesAsync(int idEmpleado)
        {
            var saldoResponse = await _httpClient.GetAsync($"/api/Empleado/{idEmpleado}/vacaciones/saldo");
            if (!saldoResponse.IsSuccessStatusCode)
                throw new Exception("Error al consultar el saldo de vacaciones del empleado.");

            var saldoVacaciones = await saldoResponse.Content.ReadFromJsonAsync<VacacionesDTO>();
            if (saldoVacaciones == null)
                throw new Exception("No se pudo obtener el saldo de vacaciones del empleado.");

            var solicitudes = await _context.SolicitudesVacaciones
                .Where(s => s.IdEmpleado == idEmpleado)
                .Select(s => new SolicitudVacacionesDTO
                {
                    Id = s.Id,
                    IdEmpleado = s.IdEmpleado,
                    DiasSolicitados = s.DiasSolicitados,
                    FechaInicio = s.FechaInicio,
                    FechaFin = s.FechaFin,
                    FechaSolicitud = s.FechaSolicitud,
                    Estado = s.Estado,
                    FechaCambioEstado = s.FechaCambioEstado,
                    MotivoRechazo = s.MotivoRechazo,
                    ModificadoPor = s.ModificadoPor
                }).ToListAsync();

            return new
            {
                saldoVacaciones.DiasDisponibles,
                saldoVacaciones.DiasGozados,
                solicitudes
            };
        }

        private SolicitudVacacionesDTO MapToDTO(SolicitudVacaciones solicitud)
        {
            return new SolicitudVacacionesDTO
            {
                Id = solicitud.Id,
                IdEmpleado = solicitud.IdEmpleado,
                DiasSolicitados = solicitud.DiasSolicitados,
                FechaInicio = solicitud.FechaInicio,
                FechaFin = solicitud.FechaFin,
                FechaSolicitud = solicitud.FechaSolicitud,
                Estado = solicitud.Estado,
                FechaCambioEstado = solicitud.FechaCambioEstado,
                MotivoRechazo = solicitud.MotivoRechazo,
                ModificadoPor = solicitud.ModificadoPor
            };
        }
    }
}
