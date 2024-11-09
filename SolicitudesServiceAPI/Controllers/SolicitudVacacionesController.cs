using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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

        public SolicitudVacacionesController(ISolicitudVacacionesService solicitudVacacionesService)
        {
            _solicitudVacacionesService = solicitudVacacionesService;
        }

        // POST: api/SolicitudVacaciones
        [HttpPost]
        public async Task<IActionResult> CrearSolicitud([FromBody] SolicitudVacacionesDTO solicitudDTO)
        {
            if (solicitudDTO == null)
                return BadRequest("La solicitud no puede estar vacía.");

            // Validación de datos obligatorios
            if (solicitudDTO.IdEmpleado <= 0)
                return BadRequest("Debe especificar un ID de empleado válido.");

            if (solicitudDTO.DiasSolicitados <= 0)
                return BadRequest("La cantidad de días debe ser mayor a cero.");

            if (solicitudDTO.FechaInicio == default || solicitudDTO.FechaFin == default)
                return BadRequest("Debe especificar fechas válidas para las vacaciones.");

            if (solicitudDTO.FechaFin <= solicitudDTO.FechaInicio)
                return BadRequest("La fecha de fin debe ser posterior a la fecha de inicio.");

            if ((solicitudDTO.FechaFin - solicitudDTO.FechaInicio).Days + 1 != solicitudDTO.DiasSolicitados)
                return BadRequest("La cantidad de días no coincide con el período seleccionado.");

            if (solicitudDTO.FechaInicio < DateTime.Now.Date)
                return BadRequest("La fecha de inicio de las vacaciones no puede ser en el pasado.");

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

        // PUT: api/SolicitudVacaciones
        [HttpPut]
        public async Task<IActionResult> ActualizarSolicitud([FromBody] SolicitudVacacionesDTO solicitudDTO)
        {
            if (solicitudDTO == null)
                return BadRequest("La solicitud no puede estar vacía.");

            if (solicitudDTO.Id <= 0)
                return BadRequest("El ID de la solicitud debe ser un número positivo.");

            if (solicitudDTO.DiasSolicitados <= 0)
                return BadRequest("La cantidad de días debe ser mayor a cero.");

            if (solicitudDTO.FechaInicio == default || solicitudDTO.FechaFin == default)
                return BadRequest("Debe especificar fechas válidas para las vacaciones.");

            if (solicitudDTO.FechaFin <= solicitudDTO.FechaInicio)
                return BadRequest("La fecha de fin debe ser posterior a la fecha de inicio.");

            if ((solicitudDTO.FechaFin - solicitudDTO.FechaInicio).Days + 1 != solicitudDTO.DiasSolicitados)
                return BadRequest("La cantidad de días no coincide con el período seleccionado.");

            var updated = await _solicitudVacacionesService.ActualizarSolicitudAsync(solicitudDTO);
            if (!updated)
                return NotFound("No se pudo actualizar la solicitud. Puede que no esté en estado 'Pendiente' o no exista.");

            return NoContent();
        }

        // DELETE: api/SolicitudVacaciones/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarSolicitud(int id)
        {
            if (id <= 0)
                return BadRequest("El ID de la solicitud debe ser un número positivo.");

            var deleted = await _solicitudVacacionesService.EliminarSolicitudAsync(id);
            if (!deleted)
                return NotFound("No se pudo eliminar la solicitud. Puede que no esté en estado 'Pendiente' o no exista.");

            return NoContent();
        }

        // PUT: api/SolicitudVacaciones/aprobar/{id}
        [HttpPut("aprobar/{id}")]
        public async Task<IActionResult> AprobarSolicitud(int id)
        {
            if (id <= 0)
                return BadRequest("El ID de la solicitud debe ser un número positivo.");

            var approved = await _solicitudVacacionesService.AprobarSolicitudAsync(id);
            if (!approved)
                return NotFound("No se pudo aprobar la solicitud. Puede que no esté en estado 'Pendiente' o no exista.");

            return NoContent();
        }

        // PUT: api/SolicitudVacaciones/rechazar/{id}
        [HttpPut("rechazar/{id}")]
        public async Task<IActionResult> RechazarSolicitud(int id, [FromQuery] string motivoRechazo)
        {
            if (id <= 0)
                return BadRequest("El ID de la solicitud debe ser un número positivo.");

            if (string.IsNullOrWhiteSpace(motivoRechazo))
                return BadRequest("Debe proporcionar un motivo de rechazo.");

            var rejected = await _solicitudVacacionesService.RechazarSolicitudAsync(id, motivoRechazo);
            if (!rejected)
                return NotFound("No se pudo rechazar la solicitud. Puede que no esté en estado 'Pendiente' o no exista.");

            return NoContent();
        }

        // GET: api/SolicitudVacaciones/todas
        [HttpGet("todas")]
        public async Task<IActionResult> ObtenerTodasSolicitudes()
        {
            var solicitudes = await _solicitudVacacionesService.ObtenerTodasSolicitudesAsync();
            if (solicitudes == null || !solicitudes.Any())
                return NotFound("No se encontraron solicitudes.");

            return Ok(solicitudes);
        }
    }
}
