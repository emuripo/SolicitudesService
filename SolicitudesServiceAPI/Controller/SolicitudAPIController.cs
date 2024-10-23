using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SolicitudesService.Core.Entities;
using SolicitudesService.Application.DTO;
using SolicitudesService.Infrastructure.Data;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SolicitudesService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolicitudesAPIController : ControllerBase
    {
        private readonly SolicitudesServiceDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public SolicitudesAPIController(SolicitudesServiceDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        // POST: api/Solicitudes
        [HttpPost]
        public async Task<ActionResult<SolicitudVacacionesDTO>> CreateSolicitud([FromBody] SolicitudVacacionesDTO solicitudDTO)
        {
            if (!await ValidarSolicitud(solicitudDTO))
            {
                return BadRequest("La solicitud no es válida. Verifica los días disponibles o si las fechas se superponen.");
            }

            var solicitud = new SolicitudVacaciones
            {
                IdEmpleado = solicitudDTO.IdEmpleado,
                FechaInicio = solicitudDTO.FechaInicio,
                FechaFin = solicitudDTO.FechaFin,
                FechaSolicitud = DateTime.Now, // Se registra la fecha actual
                EstaAprobada = false // Inicialmente no aprobada
            };

            _context.SolicitudesVacaciones.Add(solicitud);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSolicitud), new { id = solicitud.IdSolicitudVacaciones }, solicitudDTO);
        }

        // PUT: api/Solicitudes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSolicitud(int id, [FromBody] SolicitudVacacionesDTO solicitudDTO)
        {
            var solicitud = await _context.SolicitudesVacaciones.FindAsync(id);
            if (solicitud == null)
            {
                return NotFound();
            }

            if (!await ValidarSolicitud(solicitudDTO))
            {
                return BadRequest("La solicitud no es válida. Verifica los días disponibles o si las fechas se superponen.");
            }

            // Actualizar propiedades
            solicitud.FechaInicio = solicitudDTO.FechaInicio;
            solicitud.FechaFin = solicitudDTO.FechaFin;
            solicitud.EstaAprobada = solicitudDTO.EstaAprobada;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Solicitudes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSolicitud(int id)
        {
            var solicitud = await _context.SolicitudesVacaciones.FindAsync(id);
            if (solicitud == null)
            {
                return NotFound();
            }

            _context.SolicitudesVacaciones.Remove(solicitud);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Solicitudes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SolicitudVacacionesDTO>> GetSolicitud(int id)
        {
            var solicitud = await _context.SolicitudesVacaciones.FindAsync(id);

            if (solicitud == null)
            {
                return NotFound();
            }

            var solicitudDTO = new SolicitudVacacionesDTO
            {
                IdSolicitudVacaciones = solicitud.IdSolicitudVacaciones,
                IdEmpleado = solicitud.IdEmpleado,
                FechaInicio = solicitud.FechaInicio,
                FechaFin = solicitud.FechaFin,
                FechaSolicitud = solicitud.FechaSolicitud,
                EstaAprobada = solicitud.EstaAprobada
            };

            return Ok(solicitudDTO);
        }

        // Validar la solicitud
        private async Task<bool> ValidarSolicitud(SolicitudVacacionesDTO solicitudDTO)
        {
            // Validar que las fechas de vacaciones no se superpongan
            if (await SolicitudSeSuperpone(solicitudDTO.IdEmpleado, solicitudDTO.FechaInicio, solicitudDTO.FechaFin))
            {
                return false; // Las fechas se superponen
            }

            // Verificar que el empleado tenga días de vacaciones disponibles
            if (!await EmpleadoTieneDiasDisponibles(solicitudDTO.IdEmpleado, solicitudDTO.FechaInicio, solicitudDTO.FechaFin))
            {
                return false; // No hay suficientes días disponibles
            }

            return true; // Validación exitosa
        }

        // Método para comprobar si la solicitud se superpone
        private async Task<bool> SolicitudSeSuperpone(int idEmpleado, DateTime fechaInicio, DateTime fechaFin)
        {
            var solicitudes = await _context.SolicitudesVacaciones
                .Where(s => s.IdEmpleado == idEmpleado && s.FechaFin >= fechaInicio && s.FechaInicio <= fechaFin)
                .ToListAsync();

            return solicitudes.Count > 0; // Retorna true si hay superposición
        }

        // Método para verificar si el empleado tiene días disponibles
        private async Task<bool> EmpleadoTieneDiasDisponibles(int idEmpleado, DateTime fechaInicio, DateTime fechaFin)
        {
            // Obtén el número de días solicitados
            int diasSolicitados = (fechaFin - fechaInicio).Days + 1;

            // Llamar a FuncionarioService para obtener los días acumulados y tomados
            var httpClient = _httpClientFactory.CreateClient("FuncionarioService");
            var response = await httpClient.GetAsync($"/{idEmpleado}");

            if (!response.IsSuccessStatusCode)
            {
                return false; // Error al obtener datos del empleado
            }

            var empleadoJson = await response.Content.ReadAsStringAsync();

            // Deserializar solo los datos que necesitamos
            var empleado = JsonSerializer.Deserialize<dynamic>(empleadoJson);

            // Suponiendo que "FechaContratacion" viene en el JSON
            DateTime fechaContratacion = empleado?.FechaContratacion;

            if (fechaContratacion == null)
            {
                return false; // Error, no se pudo obtener la fecha de contratación
            }

            var diasAcumulados = CalcularDiasAcumulados(fechaContratacion);
            var diasTomados = await ObtenerDiasTomados(idEmpleado);

            // Verifica si los días solicitados no exceden los días disponibles
            return (diasAcumulados - diasTomados) >= diasSolicitados;
        }

        // Método para calcular los días acumulados en base a la fecha de ingreso
        private int CalcularDiasAcumulados(DateTime fechaIngreso)
        {
            var mesesTrabajados = (DateTime.Now.Year - fechaIngreso.Year) * 12 + DateTime.Now.Month - fechaIngreso.Month;
            return mesesTrabajados; // 1 día por mes trabajado
        }

        // Método para obtener los días de vacaciones ya tomados
        private async Task<int> ObtenerDiasTomados(int idEmpleado)
        {
            var solicitudesAprobadas = await _context.SolicitudesVacaciones
                .Where(s => s.IdEmpleado == idEmpleado && s.EstaAprobada)
                .ToListAsync();

            return solicitudesAprobadas.Sum(s => (s.FechaFin - s.FechaInicio).Days + 1);
        }
    }
}
