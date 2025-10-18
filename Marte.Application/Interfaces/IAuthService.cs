using Marte.Application.Models;
using Marte.Domain.Entities;

namespace Marte.Application.Interfaces
{
    public interface IAuthService
    {
        Task<Usuario?> LoginAsync(LoginRequest request);
        Usuario? Login(LoginRequest request);
        Task LogoutAsync(string usuario);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }
}