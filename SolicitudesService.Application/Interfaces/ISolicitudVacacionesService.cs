using SolicitudesService.Application.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SolicitudesService.Application.Interfaces
{
    public interface ISolicitudVacacionesService
    {
        Task<IEnumerable<SolicitudVacacionesDTO>> GetAllSolicitudes();
        Task<SolicitudVacacionesDTO> GetSolicitudById(int id);
        Task<SolicitudVacacionesDTO> CreateSolicitud(SolicitudVacacionesDTO solicitud);
        Task<bool> UpdateSolicitud(int id, SolicitudVacacionesDTO solicitud);
        Task<bool> DeleteSolicitud(int id);
    }
}
