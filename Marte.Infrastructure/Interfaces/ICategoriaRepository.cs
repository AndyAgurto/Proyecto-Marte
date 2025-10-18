using Marte.Domain.Entities;

namespace Marte.Infrastructure.Interfaces
{
    public interface ICategoriaRepository
    {
        Task<List<Categoria>> GetAllAsync();
        Task<Categoria?> GetByIdAsync(Guid id);
        Task<Categoria> CreateAsync(Categoria categoria);
        Task<Categoria> UpdateAsync(Categoria categoria);
        Task DeleteAsync(Guid id);
        Task<bool> ExisteNombreAsync(string nombre, Guid? excludeId = null);
    }
}
