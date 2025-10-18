using Marte.Domain.Entities;

namespace Marte.Infrastructure.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetByUsernameAsync(string username);
        Task<Usuario?> GetByIdAsync(Guid id);
        Task<IEnumerable<Usuario>> GetAllAsync();
        Task AddAsync(Usuario usuario);
        Task UpdateAsync(Usuario usuario);
        Task DeleteAsync(Guid id);
        Task SaveChangesAsync();
        Task<bool> ExistsAsync(string username);
    }
}
