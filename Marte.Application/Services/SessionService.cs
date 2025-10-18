using Marte.Application.Interfaces;
using Marte.Domain.Entities;

namespace Marte.Application.Services
{
    public class SessionService : ISessionService
    {
        public Usuario? CurrentUser { get; private set; }
        public bool IsAuthenticated => CurrentUser != null;

        public event EventHandler<Usuario?>? UserChanged;

        public Task SignInAsync(Usuario user)
        {
            CurrentUser = user;
            UserChanged?.Invoke(this, user);
            return Task.CompletedTask;
        }

        public Task SignOutAsync()
        {
            CurrentUser = null;
            UserChanged?.Invoke(this, null);
            return Task.CompletedTask;
        }
    }
}