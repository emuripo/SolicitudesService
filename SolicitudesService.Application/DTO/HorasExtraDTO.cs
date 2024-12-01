using SolicitudesService.Application.DTO;

public class HorasExtraDTO
{
    public int HorasExtrasTrabajadasHoy { get; set; }
    public int HorasExtrasTrabajadasSemana { get; set; }
    public int HorasExtrasTrabajadasMes { get; set; }
    public List<SolicitudHorasExtraDTO> SolicitudesAprobadas { get; set; } = new List<SolicitudHorasExtraDTO>();
}
