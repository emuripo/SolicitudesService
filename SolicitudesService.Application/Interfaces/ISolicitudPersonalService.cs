using System.Collections.Generic;
using System.Threading.Tasks;
using SolicitudesService.Application.DTO;

namespace SolicitudesService.Interfaces
{
    public interface ISolicitudPersonalService
    {
        Task<SolicitudPersonalDTO> CrearSolicitudAsync(SolicitudPersonalDTO solicitudDTO);
        Task<SolicitudPersonalDTO?> ObtenerSolicitudPorIdAsync(int id);
        Task<IEnumerable<SolicitudPersonalDTO>> ObtenerSolicitudesPorEmpleadoAsync(int idEmpleado);
        Task<bool> ActualizarSolicitudAsync(SolicitudPersonalDTO solicitudDTO);
        Task<bool> EliminarSolicitudAsync(int id);
        Task<bool> AprobarSolicitudAsync(int id);
        Task<bool> RechazarSolicitudAsync(int id, string motivoRechazo);
        Task<IEnumerable<SolicitudPersonalDTO>> ObtenerTodasSolicitudesAsync();
    }
}
