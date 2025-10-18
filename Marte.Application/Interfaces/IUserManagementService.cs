using Marte.Domain.Entities;

namespace Marte.Application.Interfaces
{
    public interface IUserManagementService
    {
        Task<IEnumerable<Usuario>> GetAllUsersAsync();
        Task<Usuario?> GetUserByIdAsync(Guid id);
        Task<Usuario> CreateUserAsync(string nombreUsuario, string contraseña, string nombreCompleto, Guid rolId, string currentUser);
        Task UpdateUserAsync(Guid id, string nombreCompleto, Guid rolId, string currentUser);
        Task ChangePasswordAsync(Guid id, string nuevaContraseña, string currentUser);
        Task DisableUserAsync(Guid id, string currentUser);
        Task EnableUserAsync(Guid id, string currentUser);
        Task<bool> UserExistsAsync(string nombreUsuario);
    }
}
