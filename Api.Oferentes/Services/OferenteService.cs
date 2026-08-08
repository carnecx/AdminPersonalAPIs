using Api.Oferentes.Entities;
using Api.Oferentes.Repository;

namespace Api.Oferentes.Services
{
    public class OferenteService : IOferenteService
    {
        private readonly OferenteRepository _oferenteRepository;

        public OferenteService(OferenteRepository oferenteRepository)
        {
            _oferenteRepository = oferenteRepository;
        }

        public async Task<IEnumerable<Oferente>> GetAllAsync()
        {
            return await _oferenteRepository.GetAllAsync();
        }

        public async Task<OferenteDetalle?> GetDetalleByIdentificacionAsync(string identificacion)
        {
            return await _oferenteRepository.GetDetalleByIdentificacionAsync(identificacion);
        }

        public async Task<IEnumerable<OferenteResumen>> GetOferentesPorPuestoAsync(string codigoPuesto)
        {
            return await _oferenteRepository.GetOferentesPorPuestoAsync(codigoPuesto);
        }

        public async Task<string> CrearPostulacionAsync(OferentePostulacion datos, string? rutaCurriculum)
        {
            return await _oferenteRepository.CrearPostulacionAsync(datos, rutaCurriculum);
        }
    }
}