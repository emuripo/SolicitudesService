using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SolicitudesService.Application.DTO;

namespace SolicitudesService.Interfaces
{
    public interface ISolicitudDocumentoService
    {
        Task<SolicitudDocumentoDTO> CrearSolicitudAsync(SolicitudDocumentoDTO solicitudDTO);
        Task<SolicitudDocumentoDTO?> ObtenerSolicitudPorIdAsync(int id);
        Task<IEnumerable<SolicitudDocumentoDTO>> ObtenerSolicitudesPorEmpleadoAsync(int idEmpleado);
        Task<bool> ActualizarSolicitudAsync(SolicitudDocumentoDTO solicitudDTO);
        Task<bool> EliminarSolicitudAsync(int id);
        Task<bool> AprobarSolicitudAsync(int id);
        Task<bool> RechazarSolicitudAsync(int id, string motivoRechazo);
        Task<IEnumerable<SolicitudDocumentoDTO>> ObtenerTodasSolicitudesAsync();
    }
}
