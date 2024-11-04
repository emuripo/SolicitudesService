using Microsoft.AspNetCore.Mvc;
using SolicitudesService.Application.DTO;
using SolicitudesService.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SolicitudesService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolicitudVacacionesController : ControllerBase
    {
        private readonly ISolicitudVacacionesService _solicitudVacacionesService;
        private readonly ILogger<SolicitudVacacionesController> _logger;

        public SolicitudVacacionesController(ISolicitudVacacionesService solicitudVacacionesService, ILogger<SolicitudVacacionesController> logger)
        {
            _solicitudVacacionesService = solicitudVacacionesService;
            _logger = logger;
        }

        // GET: api/SolicitudVacaciones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SolicitudVacacionesDTO>>> GetAllSolicitudes()
        {
            var solicitudes = (await _solicitudVacacionesService.GetAllSolicitudes()).ToList();
            _logger.LogInformation("Se obtuvieron {Count} solicitudes de vacaciones.", solicitudes.Count);
            return Ok(solicitudes);
        }

        // GET: api/SolicitudVacaciones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SolicitudVacacionesDTO>> GetSolicitudVacacionesById(int id)
        {
            var solicitud = await _solicitudVacacionesService.GetSolicitudById(id);

            if (solicitud == null)
            {
                _logger.LogWarning("No se encontró la solicitud de vacaciones con ID {Id}.", id);
                return NotFound($"No se encontró la solicitud de vacaciones con ID {id}.");
            }

            _logger.LogInformation("Se obtuvo exitosamente la solicitud de vacaciones con ID {Id}.", id);
            return Ok(solicitud);
        }

        // POST: api/SolicitudVacaciones
        [HttpPost]
        public async Task<ActionResult<SolicitudVacacionesDTO>> CreateSolicitud([FromBody] SolicitudVacacionesDTO solicitudDTO)
        {
            try
            {
                var solicitudCreada = await _solicitudVacacionesService.CreateSolicitud(solicitudDTO);
                return CreatedAtAction(nameof(GetSolicitudVacacionesById), new { id = solicitudCreada.IdSolicitudVacaciones }, solicitudCreada);
            }
            catch (System.Exception ex)
            {
                _logger.LogError("Error inesperado al crear la solicitud: {Message}", ex.Message);
                return StatusCode(500, "Ocurrió un error inesperado al crear la solicitud.");
            }
        }

        // PUT: api/SolicitudVacaciones/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSolicitud(int id, [FromBody] SolicitudVacacionesDTO solicitudDTO)
        {
            try
            {
                var resultado = await _solicitudVacacionesService.UpdateSolicitud(id, solicitudDTO);
                if (!resultado)
                {
                    _logger.LogWarning("No se pudo actualizar la solicitud de vacaciones con ID {Id}.", id);
                    return NotFound($"No se encontró la solicitud de vacaciones con ID {id}.");
                }

                _logger.LogInformation("Solicitud de vacaciones con ID {Id} actualizada exitosamente.", id);
                return NoContent();
            }
            catch (System.Exception ex)
            {
                _logger.LogError("Error inesperado al actualizar la solicitud: {Message}", ex.Message);
                return StatusCode(500, "Ocurrió un error inesperado al actualizar la solicitud.");
            }
        }

        // DELETE: api/SolicitudVacaciones/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSolicitud(int id)
        {
            var resultado = await _solicitudVacacionesService.DeleteSolicitud(id);

            if (!resultado)
            {
                _logger.LogWarning("No se encontró la solicitud de vacaciones con ID {Id}.", id);
                return NotFound($"No se encontró la solicitud de vacaciones con ID {id}.");
            }

            _logger.LogInformation("Solicitud de vacaciones con ID {Id} eliminada exitosamente.", id);
            return NoContent();
        }
    }
}
