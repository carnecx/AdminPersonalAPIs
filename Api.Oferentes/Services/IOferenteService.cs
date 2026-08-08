using Api.Oferentes.Entities;

namespace Api.Oferentes.Services
{
    public interface IOferenteService
    {
        Task<IEnumerable<Oferente>> GetAllAsync();
        Task<OferenteDetalle?> GetDetalleByIdentificacionAsync(string identificacion);
        Task<IEnumerable<OferenteResumen>> GetOferentesPorPuestoAsync(string codigoPuesto);
        Task<string> CrearPostulacionAsync(OferentePostulacion datos, string? rutaCurriculum);
    }
}