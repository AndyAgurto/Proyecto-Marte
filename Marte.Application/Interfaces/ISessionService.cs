using Marte.Domain.Entities;

namespace Marte.Application.Interfaces
{
    public interface ISessionService
    {
        Usuario? CurrentUser { get; }
        bool IsAuthenticated { get; }
        Task SignInAsync(Usuario user);
        Task SignOutAsync();
        event EventHandler<Usuario?>? UserChanged;
    }
}