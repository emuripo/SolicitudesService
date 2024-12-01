using System.Collections.Generic;
using System.Threading.Tasks;
using SolicitudesService.Application.DTO;

namespace SolicitudesService.Interfaces
{
    public interface ISolicitudVacacionesService
    {
        Task<SolicitudVacacionesDTO> CrearSolicitudAsync(SolicitudVacacionesDTO solicitudDTO);
        Task<SolicitudVacacionesDTO?> ObtenerSolicitudPorIdAsync(int id);
        Task<IEnumerable<SolicitudVacacionesDTO>> ObtenerSolicitudesPorEmpleadoAsync(int idEmpleado);
        Task<bool> ActualizarSolicitudAsync(SolicitudVacacionesDTO solicitudDTO);
        Task<bool> EliminarSolicitudAsync(int id);
        Task<bool> AprobarSolicitudAsync(int id);
        Task<bool> RechazarSolicitudAsync(int id, string motivoRechazo);
        Task<IEnumerable<SolicitudVacacionesDTO>> ObtenerTodasSolicitudesAsync();
        Task<object> ObtenerResumenVacacionesAsync(int idEmpleado);
    }
}
