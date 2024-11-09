using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SolicitudesService.Application.DTO;

namespace SolicitudesService.Interfaces
{
    public interface ISolicitudHorasExtraService
    {
        Task<SolicitudHorasExtraDTO> CrearSolicitudAsync(SolicitudHorasExtraDTO solicitudDTO);
        Task<SolicitudHorasExtraDTO?> ObtenerSolicitudPorIdAsync(int id);
        Task<IEnumerable<SolicitudHorasExtraDTO>> ObtenerSolicitudesPorEmpleadoAsync(int idEmpleado);
        Task<bool> ActualizarSolicitudAsync(SolicitudHorasExtraDTO solicitudDTO);
        Task<bool> EliminarSolicitudAsync(int id);
        Task<bool> AprobarSolicitudAsync(int id);
        Task<bool> RechazarSolicitudAsync(int id, string motivoRechazo);
        Task<IEnumerable<SolicitudHorasExtraDTO>> ObtenerTodasSolicitudesAsync();
    }
}
