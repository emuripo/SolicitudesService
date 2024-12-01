using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using SolicitudesService.Application.DTO;
using SolicitudesService.Interfaces;

namespace SolicitudesServiceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SolicitudVacacionesController : ControllerBase
    {
        private readonly ISolicitudVacacionesService _solicitudVacacionesService;
        private readonly HttpClient _httpClient;

        public SolicitudVacacionesController(ISolicitudVacacionesService solicitudVacacionesService, IHttpClientFactory httpClientFactory)
        {
            _solicitudVacacionesService = solicitudVacacionesService;
            _httpClient = httpClientFactory.CreateClient("FuncionarioService");
        }

        // POST: api/SolicitudVacaciones
        [HttpPost]
        public async Task<IActionResult> CrearSolicitud([FromBody] SolicitudVacacionesDTO solicitudDTO)
        {
            if (solicitudDTO == null || solicitudDTO.DiasSolicitados <= 0)
                return BadRequest("La solicitud no puede estar vacía y debe tener un número positivo de días solicitados.");

            var empleadoResponse = await _httpClient.GetAsync($"/api/Empleado/{solicitudDTO.IdEmpleado}");
            if (!empleadoResponse.IsSuccessStatusCode)
                return NotFound("Empleado no encontrado.");

            var empleado = await empleadoResponse.Content.ReadFromJsonAsync<EmpleadoDTO>();
            if (empleado == null)
                return NotFound("No se pudo obtener la información del empleado.");

            var antiguedad = DateTime.Now - empleado.FechaContratacion;
            if (antiguedad.TotalDays < 180)
                return BadRequest("El empleado no tiene la antigüedad mínima de 6 meses para solicitar vacaciones.");

            var saldoResponse = await _httpClient.GetAsync($"/api/Empleado/{solicitudDTO.IdEmpleado}/vacaciones/saldo");
            if (!saldoResponse.IsSuccessStatusCode)
                return StatusCode(500, "Error al consultar el saldo de vacaciones.");

            var saldoVacaciones = await saldoResponse.Content.ReadFromJsonAsync<VacacionesDTO>();
            if (saldoVacaciones == null)
                return StatusCode(500, "No se pudo obtener el saldo de vacaciones del empleado.");

            if (saldoVacaciones.DiasDisponibles < solicitudDTO.DiasSolicitados || saldoVacaciones.DiasDisponibles > 15)
                return BadRequest("Saldo insuficiente o supera el límite de 15 días disponibles.");

            var result = await _solicitudVacacionesService.CrearSolicitudAsync(solicitudDTO);
            return CreatedAtAction(nameof(ObtenerSolicitudPorId), new { id = result.Id }, result);
        }

        // GET: api/SolicitudVacaciones/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerSolicitudPorId(int id)
        {
            if (id <= 0)
                return BadRequest("El ID de solicitud debe ser un número positivo.");

            var solicitud = await _solicitudVacacionesService.ObtenerSolicitudPorIdAsync(id);
            if (solicitud == null)
                return NotFound("Solicitud no encontrada.");

            return Ok(solicitud);
        }

        // GET: api/SolicitudVacaciones/empleado/{idEmpleado}
        [HttpGet("empleado/{idEmpleado}")]
        public async Task<IActionResult> ObtenerSolicitudesPorEmpleado(int idEmpleado)
        {
            if (idEmpleado <= 0)
                return BadRequest("El ID del empleado debe ser un número positivo.");

            var solicitudes = await _solicitudVacacionesService.ObtenerSolicitudesPorEmpleadoAsync(idEmpleado);
            if (solicitudes == null || !solicitudes.Any())
                return NotFound("No se encontraron solicitudes para el empleado especificado.");

            return Ok(solicitudes);
        }

        // GET: api/SolicitudVacaciones/empleado/{idEmpleado}/vacaciones
        [HttpGet("empleado/{idEmpleado}/vacaciones")]
        public async Task<IActionResult> ObtenerResumenVacaciones(int idEmpleado)
        {
            if (idEmpleado <= 0)
                return BadRequest("El ID del empleado debe ser un número positivo.");

            try
            {
                var resumenVacaciones = await _solicitudVacacionesService.ObtenerResumenVacacionesAsync(idEmpleado);
                return Ok(resumenVacaciones);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PUT: api/SolicitudVacaciones/aprobar/{id}
        [HttpPut("aprobar/{id}")]
        public async Task<IActionResult> AprobarSolicitud(int id)
        {
            var solicitud = await _solicitudVacacionesService.ObtenerSolicitudPorIdAsync(id);
            if (solicitud == null || solicitud.Estado != "Pendiente")
                return NotFound("Solicitud no encontrada o ya procesada.");

            var saldoResponse = await _httpClient.GetAsync($"/api/Empleado/{solicitud.IdEmpleado}/vacaciones/saldo");
            if (!saldoResponse.IsSuccessStatusCode)
                return StatusCode(500, "Error al consultar el saldo de vacaciones.");

            var saldoVacaciones = await saldoResponse.Content.ReadFromJsonAsync<VacacionesDTO>();
            if (saldoVacaciones == null)
                return StatusCode(500, "No se pudo obtener el saldo de vacaciones del empleado.");

            if (saldoVacaciones.DiasDisponibles < solicitud.DiasSolicitados || saldoVacaciones.DiasDisponibles > 15)
                return BadRequest("Saldo insuficiente para aprobar la solicitud o excede el límite de 15 días.");

            var aprobado = await _solicitudVacacionesService.AprobarSolicitudAsync(id);
            return aprobado ? Ok("Solicitud aprobada y saldo actualizado.") : StatusCode(500, "No se pudo aprobar la solicitud.");
        }

        // PUT: api/SolicitudVacaciones/rechazar/{id}
        [HttpPut("rechazar/{id}")]
        public async Task<IActionResult> RechazarSolicitud(int id, [FromQuery] string motivoRechazo)
        {
            var solicitud = await _solicitudVacacionesService.ObtenerSolicitudPorIdAsync(id);
            if (solicitud == null || solicitud.Estado != "Pendiente")
                return NotFound("Solicitud no encontrada o ya procesada.");

            var rejected = await _solicitudVacacionesService.RechazarSolicitudAsync(id, motivoRechazo);
            return rejected ? Ok("Solicitud rechazada.") : StatusCode(500, "No se pudo rechazar la solicitud.");
        }

        // DELETE: api/SolicitudVacaciones/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarSolicitud(int id)
        {
            if (id <= 0)
                return BadRequest("El ID de la solicitud debe ser un número positivo.");

            var solicitud = await _solicitudVacacionesService.ObtenerSolicitudPorIdAsync(id);
            if (solicitud == null || solicitud.Estado != "Pendiente")
                return NotFound("Solicitud no encontrada o no está en estado 'Pendiente'.");

            var deleted = await _solicitudVacacionesService.EliminarSolicitudAsync(id);
            return deleted ? NoContent() : StatusCode(500, "No se pudo eliminar la solicitud.");
        }

        // GET: api/SolicitudVacaciones/todas
        [HttpGet("todas")]
        public async Task<IActionResult> ObtenerTodasSolicitudes()
        {
            var solicitudes = await _solicitudVacacionesService.ObtenerTodasSolicitudesAsync();
            return Ok(solicitudes);
        }

        // PUT: api/SolicitudVacaciones
        [HttpPut]
        public async Task<IActionResult> ActualizarSolicitud([FromBody] SolicitudVacacionesDTO solicitudDTO)
        {
            if (solicitudDTO == null || solicitudDTO.Id <= 0)
                return BadRequest("Datos de solicitud no válidos.");

            var actualizado = await _solicitudVacacionesService.ActualizarSolicitudAsync(solicitudDTO);
            if (!actualizado)
                return NotFound("No se encontró la solicitud o ya fue procesada.");

            return Ok("Solicitud actualizada correctamente.");
        }
    }
}
