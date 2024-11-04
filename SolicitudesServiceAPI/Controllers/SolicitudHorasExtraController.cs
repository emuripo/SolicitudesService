using Microsoft.AspNetCore.Mvc;
using SolicitudesService.Application.DTO;
using SolicitudesService.Application.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SolicitudesService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolicitudHorasExtraController : ControllerBase
    {
        private readonly ISolicitudHorasExtraService _solicitudHorasExtraService;

        public SolicitudHorasExtraController(ISolicitudHorasExtraService solicitudHorasExtraService)
        {
            _solicitudHorasExtraService = solicitudHorasExtraService;
        }

        // POST: api/SolicitudHorasExtra
        [HttpPost]
        public async Task<ActionResult<SolicitudHorasExtraDTO>> CreateSolicitudHorasExtra([FromBody] SolicitudHorasExtraDTO solicitudDTO)
        {
            var solicitud = await _solicitudHorasExtraService.CreateSolicitudHorasExtraAsync(solicitudDTO);
            if (solicitud == null)
            {
                return BadRequest("La solicitud de horas extra no es válida. Verifica que no se exceda el límite permitido de horas.");
            }

            return CreatedAtAction(nameof(GetSolicitudHorasExtra), new { id = solicitud.IdSolicitudHorasExtra }, solicitud);
        }

        // PUT: api/SolicitudHorasExtra/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSolicitudHorasExtra(int id, [FromBody] SolicitudHorasExtraDTO solicitudDTO)
        {
            var result = await _solicitudHorasExtraService.UpdateSolicitudHorasExtraAsync(id, solicitudDTO);
            if (result == null)
            {
                return NotFound("No se encontró la solicitud de horas extra para actualizar.");
            }

            return NoContent();
        }

        // GET: api/SolicitudHorasExtra/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SolicitudHorasExtraDTO>> GetSolicitudHorasExtra(int id)
        {
            var solicitud = await _solicitudHorasExtraService.GetSolicitudHorasExtraByIdAsync(id);
            if (solicitud == null)
            {
                return NotFound("No se encontró la solicitud de horas extra.");
            }

            return Ok(solicitud);
        }

        // DELETE: api/SolicitudHorasExtra/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSolicitudHorasExtra(int id)
        {
            var result = await _solicitudHorasExtraService.DeleteSolicitudHorasExtraAsync(id);
            if (!result)
            {
                return NotFound("No se encontró la solicitud de horas extra para eliminar.");
            }

            return NoContent();
        }

        // GET: api/SolicitudHorasExtra
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SolicitudHorasExtraDTO>>> GetAllSolicitudesHorasExtra()
        {
            var solicitudes = await _solicitudHorasExtraService.GetAllSolicitudesHorasExtraAsync();
            return Ok(solicitudes);
        }

        // GET: api/SolicitudHorasExtra/empleado/{idEmpleado}
        [HttpGet("empleado/{idEmpleado}")]
        public async Task<ActionResult<IEnumerable<SolicitudHorasExtraDTO>>> GetSolicitudesHorasExtraByEmpleado(int idEmpleado)
        {
            var solicitudes = await _solicitudHorasExtraService.GetSolicitudesHorasExtraByEmpleadoAsync(idEmpleado);
            if (solicitudes == null || !solicitudes.Any())
            {
                return NotFound("No se encontraron solicitudes de horas extra para el empleado especificado.");
            }
            return Ok(solicitudes);
        }
    }
}
