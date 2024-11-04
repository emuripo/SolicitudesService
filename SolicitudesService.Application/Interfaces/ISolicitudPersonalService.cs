using SolicitudesService.Application.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SolicitudesService.Application.Interfaces
{
    public interface ISolicitudPersonalService
    {
        Task<IEnumerable<SolicitudPersonalDTO>> GetAllSolicitudes();
        Task<SolicitudPersonalDTO> GetSolicitudById(int id);
        Task<SolicitudPersonalDTO> CreateSolicitud(SolicitudPersonalDTO solicitud);
        Task<bool> UpdateSolicitud(int id, SolicitudPersonalDTO solicitud);
        Task<bool> DeleteSolicitud(int id);
        Task<IEnumerable<SolicitudPersonalDTO>> GetSolicitudesByEmpleado(int idEmpleado);
    }
}
