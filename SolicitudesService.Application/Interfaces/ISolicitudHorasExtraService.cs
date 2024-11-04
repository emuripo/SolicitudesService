using System.Threading.Tasks;
using System.Collections.Generic;
using SolicitudesService.Application.DTO;

namespace SolicitudesService.Application.Interfaces
{
    public interface ISolicitudHorasExtraService
    {
        Task<SolicitudHorasExtraDTO> CreateSolicitudHorasExtraAsync(SolicitudHorasExtraDTO solicitudDTO);
        Task<SolicitudHorasExtraDTO> UpdateSolicitudHorasExtraAsync(int id, SolicitudHorasExtraDTO solicitudDTO);
        Task<bool> DeleteSolicitudHorasExtraAsync(int id);
        Task<SolicitudHorasExtraDTO?> GetSolicitudHorasExtraByIdAsync(int id);
        Task<IEnumerable<SolicitudHorasExtraDTO>> GetAllSolicitudesHorasExtraAsync();
        Task<IEnumerable<SolicitudHorasExtraDTO>> GetSolicitudesHorasExtraByEmpleadoAsync(int idEmpleado);
    }
}
