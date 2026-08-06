using Api.Puestos.Entities;
using Api.Puestos.Repository;

namespace Api.Puestos.Services
{
    public class PuestoService : IPuestoService
    {
        private readonly PuestoRepository _puestoRepository;

        public PuestoService(PuestoRepository puestoRepository)
        {
            _puestoRepository = puestoRepository;
        }

        public async Task<IEnumerable<Puesto>> GetActivosAsync()
        {
            return await _puestoRepository.GetActivosAsync();
        }
    }
}