using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SolicitudesService.Application.DTO;
using SolicitudesService.Interfaces;
using FuncionarioService.Interfaces; // Integración con FuncionarioService

namespace SolicitudesServiceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SolicitudVacacionesController : ControllerBase
    {
        private readonly ISolicitudVacacionesService _solicitudVacacionesService;
        private readonly IFuncionarioService _funcionarioService; // Integración con FuncionarioService

        public SolicitudVacacionesController(ISolicitudVacacionesService solicitudVacacionesService, IFuncionarioService funcionarioService)
        {
            _solicitudVacacionesService = solicitudVacacionesService;
            _funcionarioService = funcionarioService;
        }

        // POST: api/SolicitudVacaciones
        [HttpPost]
        public async Task<IActionResult> CrearSolicitud([FromBody] SolicitudVacacionesDTO solicitudDTO)
        {
            if (solicitudDTO == null)
                return BadRequest("La solicitud no puede estar vacía.");

            // Verificar antigüedad del empleado
            var empleado = await _funcionarioService.ObtenerEmpleadoPorIdAsync(solicitudDTO.IdEmpleado);
            if (empleado == null)
                return NotFound("Empleado no encontrado.");

            var antiguedad = DateTime.Now - empleado.FechaContratacion;
            if (antiguedad.TotalDays < 180)
                return BadRequest("El empleado no tiene la antigüedad mínima de 6 meses para solicitar vacaciones.");

            // Verificar saldo de vacaciones
            var saldoVacaciones = await _funcionarioService.ObtenerSaldoVacacionesPorEmpleadoAsync(solicitudDTO.IdEmpleado);
            if (saldoVacaciones.DiasDisponibles < solicitudDTO.DiasSolicitados)
                return BadRequest("Saldo insuficiente.");

            // Validaciones de formato de datos
            if (solicitudDTO.DiasSolicitados <= 0)
                return BadRequest("La cantidad de días debe ser mayor a cero.");

            if (solicitudDTO.FechaInicio == default || solicitudDTO.FechaFin == default)
                return BadRequest("Debe especificar fechas válidas para las vacaciones.");

            if (solicitudDTO.FechaFin <= solicitudDTO.FechaInicio)
                return BadRequest("La fecha de fin debe ser posterior a la fecha de inicio.");

            if ((solicitudDTO.FechaFin - solicitudDTO.FechaInicio).Days + 1 != solicitudDTO.DiasSolicitados)
                return BadRequest("La cantidad de días no coincide con el período seleccionado.");

            var result = await _solicitudVacacionesService.CrearSolicitudAsync(solicitudDTO);
            return CreatedAtAction(nameof(ObtenerSolicitudPorId), new { id = result.Id }, result);
        }

        // PUT: api/SolicitudVacaciones/aprobar/{id}
        [HttpPut("aprobar/{id}")]
        public async Task<IActionResult> AprobarSolicitud(int id)
        {
            var solicitud = await _solicitudVacacionesService.ObtenerSolicitudPorIdAsync(id);
            if (solicitud == null || solicitud.Estado != "Pendiente")
                return NotFound("Solicitud no encontrada o ya procesada.");

            // Reconfirmar saldo antes de aprobar
            var saldoVacaciones = await _funcionarioService.ObtenerSaldoVacacionesPorEmpleadoAsync(solicitud.IdEmpleado);
            if (saldoVacaciones.DiasDisponibles < solicitud.DiasSolicitados)
                return BadRequest("Saldo insuficiente para aprobar la solicitud.");

            // Actualizar el saldo y días gozados en FuncionarioService
            var actualizacionExitosa = await _funcionarioService.RestarDiasDeVacacionesAsync(solicitud.IdEmpleado, solicitud.DiasSolicitados);
            if (!actualizacionExitosa)
                return StatusCode(500, "Error al actualizar el saldo de vacaciones.");

            // Aprobar la solicitud
            var aprobado = await _solicitudVacacionesService.AprobarSolicitudAsync(id);
            if (!aprobado)
                return StatusCode(500, "No se pudo aprobar la solicitud.");

            return Ok("Solicitud aprobada y saldo actualizado.");
        }

        // PUT: api/SolicitudVacaciones/rechazar/{id}
        [HttpPut("rechazar/{id}")]
        public async Task<IActionResult> RechazarSolicitud(int id, [FromQuery] string motivoRechazo)
        {
            if (string.IsNullOrWhiteSpace(motivoRechazo))
                return BadRequest("Debe proporcionar un motivo de rechazo.");

            var solicitud = await _solicitudVacacionesService.ObtenerSolicitudPorIdAsync(id);
            if (solicitud == null || solicitud.Estado != "Pendiente")
                return NotFound("Solicitud no encontrada o ya procesada.");

            var rechazado = await _solicitudVacacionesService.RechazarSolicitudAsync(id, motivoRechazo);
            if (!rechazado)
                return StatusCode(500, "No se pudo rechazar la solicitud.");

            return Ok("Solicitud rechazada.");
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
    }
}
