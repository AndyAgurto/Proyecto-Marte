using Marte.Domain.Entities;

namespace Marte.Application.Interfaces
{
    public interface ICategoriaService
    {
        Task<List<Categoria>> GetAllCategoriasAsync();
        Task<Categoria?> GetCategoriaByIdAsync(Guid id);
        Task<Categoria> CreateCategoriaAsync(string nombre);
        Task<Categoria> UpdateCategoriaAsync(Guid id, string nombre);
        Task DeleteCategoriaAsync(Guid id);
        Task<bool> ExisteNombreCategoriaAsync(string nombre, Guid? excludeId = null);
    }
}
