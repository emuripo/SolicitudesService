using Microsoft.AspNetCore.Mvc;
using SolicitudesService.Application.DTO;
using SolicitudesService.Application.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SolicitudesService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolicitudDocumentoController : ControllerBase
    {
        private readonly ISolicitudDocumentoService _solicitudDocumentoService;

        public SolicitudDocumentoController(ISolicitudDocumentoService solicitudDocumentoService)
        {
            _solicitudDocumentoService = solicitudDocumentoService;
        }

        // POST: api/SolicitudDocumento
        [HttpPost]
        public async Task<ActionResult<SolicitudDocumentoDTO>> CreateSolicitudDocumento([FromBody] SolicitudDocumentoDTO solicitudDTO)
        {
            var solicitud = await _solicitudDocumentoService.CreateSolicitud(solicitudDTO);
            if (solicitud == null)
            {
                return BadRequest("No se pudo crear la solicitud de documento.");
            }

            return CreatedAtAction(nameof(GetSolicitudDocumento), new { id = solicitud.IdSolicitudDocumento }, solicitud);
        }

        // PUT: api/SolicitudDocumento/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSolicitudDocumento(int id, [FromBody] SolicitudDocumentoDTO solicitudDTO)
        {
            var result = await _solicitudDocumentoService.UpdateSolicitud(id, solicitudDTO);
            if (!result)
            {
                return NotFound("No se encontró la solicitud de documento para actualizar.");
            }
            return NoContent();
        }

        // GET: api/SolicitudDocumento/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SolicitudDocumentoDTO>> GetSolicitudDocumento(int id)
        {
            var solicitud = await _solicitudDocumentoService.GetSolicitudById(id);
            if (solicitud == null)
            {
                return NotFound("No se encontró la solicitud de documento.");
            }
            return Ok(solicitud);
        }

        // DELETE: api/SolicitudDocumento/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSolicitudDocumento(int id)
        {
            var result = await _solicitudDocumentoService.DeleteSolicitud(id);
            if (!result)
            {
                return NotFound("No se encontró la solicitud de documento para eliminar.");
            }
            return NoContent();
        }

        // GET: api/SolicitudDocumento
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SolicitudDocumentoDTO>>> GetAllSolicitudesDocumentos()
        {
            var solicitudes = await _solicitudDocumentoService.GetAllSolicitudes();
            return Ok(solicitudes);
        }
    }
}
