using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using SolicitudesService.Application.DTO;
using SolicitudesService.Interfaces;

namespace SolicitudesServiceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SolicitudPersonalController : ControllerBase
    {
        private readonly ISolicitudPersonalService _solicitudPersonalService;

        public SolicitudPersonalController(ISolicitudPersonalService solicitudPersonalService)
        {
            _solicitudPersonalService = solicitudPersonalService;
        }

        // POST: api/SolicitudPersonal
        [HttpPost]
        public async Task<IActionResult> CrearSolicitud([FromBody] SolicitudPersonalDTO solicitudDTO)
        {
            if (solicitudDTO == null)
                return BadRequest("La solicitud no puede estar vacía.");

            if (solicitudDTO.IdEmpleado <= 0)
                return BadRequest("Debe especificar un ID de empleado válido.");

            if (string.IsNullOrWhiteSpace(solicitudDTO.Motivo))
                return BadRequest("El motivo de la solicitud es obligatorio.");

            var result = await _solicitudPersonalService.CrearSolicitudAsync(solicitudDTO);
            return CreatedAtAction(nameof(ObtenerSolicitudPorId), new { id = result.Id }, result);
        }

        // GET: api/SolicitudPersonal/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerSolicitudPorId(int id)
        {
            if (id <= 0)
                return BadRequest("El ID de solicitud debe ser un número positivo.");

            var solicitud = await _solicitudPersonalService.ObtenerSolicitudPorIdAsync(id);
            if (solicitud == null)
                return NotFound("Solicitud no encontrada.");

            return Ok(solicitud);
        }

        // GET: api/SolicitudPersonal/empleado/{idEmpleado}
        [HttpGet("empleado/{idEmpleado}")]
        public async Task<IActionResult> ObtenerSolicitudesPorEmpleado(int idEmpleado)
        {
            if (idEmpleado <= 0)
                return BadRequest("El ID del empleado debe ser un número positivo.");

            var solicitudes = await _solicitudPersonalService.ObtenerSolicitudesPorEmpleadoAsync(idEmpleado);
            if (solicitudes == null || !solicitudes.Any())
                return NotFound("No se encontraron solicitudes para el empleado especificado.");

            return Ok(solicitudes);
        }

        // PUT: api/SolicitudPersonal
        [HttpPut]
        public async Task<IActionResult> ActualizarSolicitud([FromBody] SolicitudPersonalDTO solicitudDTO)
        {
            if (solicitudDTO == null)
                return BadRequest("La solicitud no puede estar vacía.");

            if (solicitudDTO.Id <= 0)
                return BadRequest("El ID de la solicitud debe ser un número positivo.");

            if (string.IsNullOrWhiteSpace(solicitudDTO.Motivo))
                return BadRequest("El motivo de la solicitud es obligatorio.");

            var updated = await _solicitudPersonalService.ActualizarSolicitudAsync(solicitudDTO);
            if (!updated)
                return NotFound("No se pudo actualizar la solicitud. Puede que no esté en estado 'Pendiente' o no exista.");

            return NoContent();
        }

        // DELETE: api/SolicitudPersonal/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarSolicitud(int id)
        {
            if (id <= 0)
                return BadRequest("El ID de la solicitud debe ser un número positivo.");

            var deleted = await _solicitudPersonalService.EliminarSolicitudAsync(id);
            if (!deleted)
                return NotFound("No se pudo eliminar la solicitud. Puede que no esté en estado 'Pendiente' o no exista.");

            return NoContent();
        }

        // PUT: api/SolicitudPersonal/aprobar/{id}
        [HttpPut("aprobar/{id}")]
        public async Task<IActionResult> AprobarSolicitud(int id)
        {
            if (id <= 0)
                return BadRequest("El ID de la solicitud debe ser un número positivo.");

            var approved = await _solicitudPersonalService.AprobarSolicitudAsync(id);
            if (!approved)
                return NotFound("No se pudo aprobar la solicitud. Puede que no esté en estado 'Pendiente' o no exista.");

            return NoContent();
        }

        // PUT: api/SolicitudPersonal/rechazar/{id}
        [HttpPut("rechazar/{id}")]
        public async Task<IActionResult> RechazarSolicitud(int id, [FromQuery] string motivoRechazo)
        {
            if (id <= 0)
                return BadRequest("El ID de la solicitud debe ser un número positivo.");

            if (string.IsNullOrWhiteSpace(motivoRechazo))
                return BadRequest("Debe proporcionar un motivo de rechazo.");

            var rejected = await _solicitudPersonalService.RechazarSolicitudAsync(id, motivoRechazo);
            if (!rejected)
                return NotFound("No se pudo rechazar la solicitud. Puede que no esté en estado 'Pendiente' o no exista.");

            return NoContent();
        }

        // GET: api/SolicitudPersonal/todas
        [HttpGet("todas")]
        public async Task<IActionResult> ObtenerTodasSolicitudes()
        {
            var solicitudes = await _solicitudPersonalService.ObtenerTodasSolicitudesAsync();
            if (solicitudes == null || !solicitudes.Any())
                return NotFound("No se encontraron solicitudes.");

            return Ok(solicitudes);
        }
    }
}
