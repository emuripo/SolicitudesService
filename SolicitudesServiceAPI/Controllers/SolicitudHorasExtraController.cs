using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using SolicitudesService.Application.DTO;
using SolicitudesService.Interfaces;

namespace SolicitudesServiceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SolicitudHorasExtraController : ControllerBase
    {
        private readonly ISolicitudHorasExtraService _solicitudHorasExtraService;

        public SolicitudHorasExtraController(ISolicitudHorasExtraService solicitudHorasExtraService)
        {
            _solicitudHorasExtraService = solicitudHorasExtraService;
        }

        // POST: api/SolicitudHorasExtra
        [HttpPost]
        public async Task<IActionResult> CrearSolicitud([FromBody] SolicitudHorasExtraDTO solicitudDTO)
        {
            if (solicitudDTO == null)
                return BadRequest("La solicitud no puede estar vacía.");

            if (solicitudDTO.IdEmpleado <= 0)
                return BadRequest("Debe especificar un ID de empleado válido.");

            if (solicitudDTO.CantidadHoras <= 0)
                return BadRequest("La cantidad de horas debe ser mayor a cero.");

            if (solicitudDTO.CantidadHoras > 12)
                return BadRequest("La cantidad de horas extra no puede exceder las 12 horas en un solo día.");

            if (solicitudDTO.FechaTrabajo == default)
                return BadRequest("Debe especificar una fecha válida para las horas extra trabajadas.");

            if (solicitudDTO.FechaTrabajo.Date > DateTime.Now.Date)
                return BadRequest("La fecha de trabajo no puede ser en el futuro.");

            var result = await _solicitudHorasExtraService.CrearSolicitudAsync(solicitudDTO);
            return CreatedAtAction(nameof(ObtenerSolicitudPorId), new { id = result.Id }, result);
        }

        // GET: api/SolicitudHorasExtra/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerSolicitudPorId(int id)
        {
            if (id <= 0)
                return BadRequest("El ID de solicitud debe ser un número positivo.");

            var solicitud = await _solicitudHorasExtraService.ObtenerSolicitudPorIdAsync(id);
            if (solicitud == null)
                return NotFound("Solicitud no encontrada.");

            return Ok(solicitud);
        }

        // GET: api/SolicitudHorasExtra/empleado/{idEmpleado}
        [HttpGet("empleado/{idEmpleado}")]
        public async Task<IActionResult> ObtenerSolicitudesPorEmpleado(int idEmpleado)
        {
            if (idEmpleado <= 0)
                return BadRequest("El ID del empleado debe ser un número positivo.");

            var solicitudes = await _solicitudHorasExtraService.ObtenerSolicitudesPorEmpleadoAsync(idEmpleado);
            if (solicitudes == null || !solicitudes.Any())
                return NotFound("No se encontraron solicitudes para el empleado especificado.");

            return Ok(solicitudes);
        }

        // PUT: api/SolicitudHorasExtra
        [HttpPut]
        public async Task<IActionResult> ActualizarSolicitud([FromBody] SolicitudHorasExtraDTO solicitudDTO)
        {
            if (solicitudDTO == null)
                return BadRequest("La solicitud no puede estar vacía.");

            if (solicitudDTO.Id <= 0)
                return BadRequest("El ID de la solicitud debe ser un número positivo.");

            if (solicitudDTO.CantidadHoras <= 0)
                return BadRequest("La cantidad de horas debe ser mayor a cero.");

            if (solicitudDTO.CantidadHoras > 12)
                return BadRequest("La cantidad de horas extra no puede exceder las 12 horas en un solo día.");

            if (solicitudDTO.FechaTrabajo == default)
                return BadRequest("Debe especificar una fecha válida para las horas extra trabajadas.");

            if (solicitudDTO.FechaTrabajo.Date > DateTime.Now.Date)
                return BadRequest("La fecha de trabajo no puede ser en el futuro.");

            var updated = await _solicitudHorasExtraService.ActualizarSolicitudAsync(solicitudDTO);
            if (!updated)
                return NotFound("No se pudo actualizar la solicitud. Puede que no esté en estado 'Pendiente' o no exista.");

            return NoContent();
        }

        // DELETE: api/SolicitudHorasExtra/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarSolicitud(int id)
        {
            if (id <= 0)
                return BadRequest("El ID de la solicitud debe ser un número positivo.");

            var deleted = await _solicitudHorasExtraService.EliminarSolicitudAsync(id);
            if (!deleted)
                return NotFound("No se pudo eliminar la solicitud. Puede que no esté en estado 'Pendiente' o no exista.");

            return NoContent();
        }

        // PUT: api/SolicitudHorasExtra/aprobar/{id}
        [HttpPut("aprobar/{id}")]
        public async Task<IActionResult> AprobarSolicitud(int id)
        {
            if (id <= 0)
                return BadRequest("El ID de la solicitud debe ser un número positivo.");

            var approved = await _solicitudHorasExtraService.AprobarSolicitudAsync(id);
            if (!approved)
                return NotFound("No se pudo aprobar la solicitud. Puede que no esté en estado 'Pendiente' o no exista.");

            return NoContent();
        }

        // PUT: api/SolicitudHorasExtra/rechazar/{id}
        [HttpPut("rechazar/{id}")]
        public async Task<IActionResult> RechazarSolicitud(int id, [FromQuery] string motivoRechazo)
        {
            if (id <= 0)
                return BadRequest("El ID de la solicitud debe ser un número positivo.");

            if (string.IsNullOrWhiteSpace(motivoRechazo))
                return BadRequest("Debe proporcionar un motivo de rechazo.");

            var rejected = await _solicitudHorasExtraService.RechazarSolicitudAsync(id, motivoRechazo);
            if (!rejected)
                return NotFound("No se pudo rechazar la solicitud. Puede que no esté en estado 'Pendiente' o no exista.");

            return NoContent();
        }

        // GET: api/SolicitudHorasExtra/todas
        [HttpGet("todas")]
        public async Task<IActionResult> ObtenerTodasSolicitudes()
        {
            var solicitudes = await _solicitudHorasExtraService.ObtenerTodasSolicitudesAsync();
            if (solicitudes == null || !solicitudes.Any())
                return NotFound("No se encontraron solicitudes.");

            return Ok(solicitudes);
        }
    }
}
