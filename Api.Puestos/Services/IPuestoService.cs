using Api.Puestos.Entities;

namespace Api.Puestos.Services
{
    public interface IPuestoService
    {
        Task<IEnumerable<Puesto>> GetActivosAsync();
    }
}