using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SolicitudesService.Core.Entities;
using SolicitudesService.Application.DTO;
using SolicitudesService.Infrastructure.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SolicitudesService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolicitudesAPIController : ControllerBase
    {
        private readonly SolicitudesServiceDbContext _context;

        public SolicitudesAPIController(SolicitudesServiceDbContext context)
        {
            _context = context;
        }

        // POST: api/SolicitudesHorasExtra
        [HttpPost("HorasExtra")]
        public async Task<ActionResult<SolicitudHorasExtraDTO>> CreateSolicitudHorasExtra([FromBody] SolicitudHorasExtraDTO solicitudDTO)
        {
            if (!await ValidarSolicitudHorasExtra(solicitudDTO))
            {
                return BadRequest("La solicitud de horas extra no es válida. Verifica que no se exceda el límite permitido de horas.");
            }

            var solicitud = new SolicitudHorasExtra
            {
                IdEmpleado = solicitudDTO.IdEmpleado,
                CantidadHoras = solicitudDTO.CantidadHoras,
                FechaSolicitud = DateTime.Now,
                EstaAprobada = false
            };

            _context.SolicitudesHorasExtra.Add(solicitud);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSolicitudHorasExtra), new { id = solicitud.IdSolicitudHorasExtra }, solicitudDTO);
        }

        // PUT: api/SolicitudesHorasExtra/5
        [HttpPut("HorasExtra/{id}")]
        public async Task<IActionResult> UpdateSolicitudHorasExtra(int id, [FromBody] SolicitudHorasExtraDTO solicitudDTO)
        {
            var solicitud = await _context.SolicitudesHorasExtra.FindAsync(id);
            if (solicitud == null)
            {
                return NotFound();
            }

            if (!await ValidarSolicitudHorasExtra(solicitudDTO))
            {
                return BadRequest("La solicitud de horas extra no es válida. Verifica que no se exceda el límite permitido de horas.");
            }

            solicitud.CantidadHoras = solicitudDTO.CantidadHoras;
            solicitud.EstaAprobada = solicitudDTO.EstaAprobada;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Validar la solicitud de horas extra
        private async Task<bool> ValidarSolicitudHorasExtra(SolicitudHorasExtraDTO solicitudDTO)
        {
            // Asumimos que la jornada ordinaria es de 8 horas para este ejemplo
            int jornadaOrdinaria = 8;

            // Verificar que la suma de la jornada ordinaria más las horas extra no exceda 12 horas
            if (solicitudDTO.CantidadHoras + jornadaOrdinaria > 12)
            {
                return false; // Excede el límite de 12 horas
            }

            return true;
        }

        // GET: api/SolicitudesHorasExtra/5
        [HttpGet("HorasExtra/{id}")]
        public async Task<ActionResult<SolicitudHorasExtraDTO>> GetSolicitudHorasExtra(int id)
        {
            var solicitud = await _context.SolicitudesHorasExtra.FindAsync(id);

            if (solicitud == null)
            {
                return NotFound();
            }

            var solicitudDTO = new SolicitudHorasExtraDTO
            {
                IdSolicitudHorasExtra = solicitud.IdSolicitudHorasExtra,
                IdEmpleado = solicitud.IdEmpleado,
                CantidadHoras = solicitud.CantidadHoras,
                FechaSolicitud = solicitud.FechaSolicitud,
                EstaAprobada = solicitud.EstaAprobada
            };

            return Ok(solicitudDTO);
        }

        // DELETE: api/SolicitudesHorasExtra/5
        [HttpDelete("HorasExtra/{id}")]
        public async Task<IActionResult> DeleteSolicitudHorasExtra(int id)
        {
            var solicitud = await _context.SolicitudesHorasExtra.FindAsync(id);
            if (solicitud == null)
            {
                return NotFound();
            }

            _context.SolicitudesHorasExtra.Remove(solicitud);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
