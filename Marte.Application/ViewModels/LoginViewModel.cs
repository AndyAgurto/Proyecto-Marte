using System.Windows.Input;
using Marte.Application.Interfaces;
using Marte.Application.Models;

namespace Marte.Application.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        private readonly ISessionService _sessionService;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _isLoading = false;

        public LoginViewModel(IAuthService authService, ISessionService sessionService)
        {
            _authService = authService;
            _sessionService = sessionService;
            LoginCommand = new RelayCommand(async () => await LoginAsync(), () => CanLogin());
        }

        public string Username
        {
            get => _username;
            set
            {
                if (SetProperty(ref _username, value))
                {
                    System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (SetProperty(ref _password, value))
                {
                    System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (SetProperty(ref _isLoading, value))
                {
                    System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public ICommand LoginCommand { get; }

        public event EventHandler<bool>? LoginCompleted;

        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(Username) &&
                   !string.IsNullOrWhiteSpace(Password) &&
                   !IsLoading;
        }

        private async Task LoginAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                var request = new LoginRequest
                {
                    NombreUsuario = Username,
                    Contraseña = Password
                };

                var user = await _authService.LoginAsync(request);
                if (user == null)
                {
                    ErrorMessage = "Usuario o contraseña incorrectos";
                    LoginCompleted?.Invoke(this, false);
                    return;
                }

                await _sessionService.SignInAsync(user);
                LoginCompleted?.Invoke(this, true);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al intentar iniciar sesión: {ex.Message}";
                LoginCompleted?.Invoke(this, false);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}