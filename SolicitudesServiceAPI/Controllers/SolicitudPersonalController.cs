using Microsoft.AspNetCore.Mvc;
using SolicitudesService.Application.DTO;
using SolicitudesService.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using SolicitudesService.Application.Services;

namespace SolicitudesService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolicitudPersonalController : ControllerBase
    {
        private readonly ISolicitudPersonalService _solicitudPersonalService;
        private readonly ILogger<SolicitudPersonalController> _logger;

        public SolicitudPersonalController(ISolicitudPersonalService solicitudPersonalService, ILogger<SolicitudPersonalController> logger)
        {
            _solicitudPersonalService = solicitudPersonalService;
            _logger = logger;
        }

        // GET: api/SolicitudPersonal
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SolicitudPersonalDTO>>> GetAllSolicitudes()
        {
            var solicitudes = await _solicitudPersonalService.GetAllSolicitudes();
            _logger.LogInformation("Se obtuvieron {Count} solicitudes personales.", solicitudes.Count());
            return Ok(solicitudes);
        }

        // GET: api/SolicitudPersonal/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SolicitudPersonalDTO>> GetSolicitudById(int id)
        {
            var solicitud = await _solicitudPersonalService.GetSolicitudById(id);

            if (solicitud == null)
            {
                _logger.LogWarning("Solicitud personal con ID {id} no fue encontrada.", id);
                return NotFound();
            }

            return Ok(solicitud);
        }

        // POST: api/SolicitudPersonal
        [HttpPost]
        public async Task<ActionResult<SolicitudPersonalDTO>> CreateSolicitud([FromBody] SolicitudPersonalDTO solicitudDTO)
        {
            var nuevaSolicitud = await _solicitudPersonalService.CreateSolicitud(solicitudDTO);
            _logger.LogInformation("Solicitud personal creada exitosamente con ID {IdSolicitudPersonal}", nuevaSolicitud.IdSolicitudPersonal);
            return CreatedAtAction(nameof(GetSolicitudById), new { id = nuevaSolicitud.IdSolicitudPersonal }, nuevaSolicitud);
        }

        // PUT: api/SolicitudPersonal/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSolicitud(int id, [FromBody] SolicitudPersonalDTO solicitudDTO)
        {
            var result = await _solicitudPersonalService.UpdateSolicitud(id, solicitudDTO);

            if (!result)
            {
                _logger.LogWarning("Solicitud personal con ID {id} no fue encontrada para actualizar.", id);
                return NotFound();
            }

            _logger.LogInformation("Solicitud personal con ID {IdSolicitudPersonal} actualizada exitosamente.", id);
            return NoContent();
        }

        // DELETE: api/SolicitudPersonal/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSolicitud(int id)
        {
            var result = await _solicitudPersonalService.DeleteSolicitud(id);

            if (!result)
            {
                _logger.LogWarning("Solicitud personal con ID {id} no fue encontrada para eliminar.", id);
                return NotFound();
            }

            _logger.LogInformation("Solicitud personal con ID {IdSolicitudPersonal} eliminada exitosamente.", id);
            return NoContent();
        }
    }
}
