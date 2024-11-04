using SolicitudesService.Application.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SolicitudesService.Application.Interfaces
{
    public interface ISolicitudDocumentoService
    {
        Task<IEnumerable<SolicitudDocumentoDTO>> GetAllSolicitudes();
        Task<SolicitudDocumentoDTO> GetSolicitudById(int id);
        Task<SolicitudDocumentoDTO> CreateSolicitud(SolicitudDocumentoDTO solicitud);
        Task<bool> UpdateSolicitud(int id, SolicitudDocumentoDTO solicitud);
        Task<bool> DeleteSolicitud(int id);
        Task<IEnumerable<SolicitudDocumentoDTO>> GetSolicitudesByEmpleado(int idEmpleado);
    }
}
