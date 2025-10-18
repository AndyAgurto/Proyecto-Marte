using Marte.Domain.Entities;

namespace Marte.Infrastructure.Interfaces
{
    public interface IRolRepository
    {
        Task<IEnumerable<Rol>> GetAllAsync();
        Task<Rol?> GetByIdAsync(Guid id);
        Task<Rol?> GetByNombreAsync(string nombre);
    }
}
