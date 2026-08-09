using Api.Puestos.Entities;

namespace Api.Puestos.Services
{
    public interface IPuestoService
    {
        Task<IEnumerable<Puesto>> GetActivosAsync();
        Task<IEnumerable<Puesto>> GetDisponiblesAsync();
    }
}