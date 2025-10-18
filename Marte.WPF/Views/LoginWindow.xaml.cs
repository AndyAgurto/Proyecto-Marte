using System.Windows;
using Marte.Application.Interfaces;
using Marte.Application.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Marte.WPF.Views
{
    public partial class LoginWindow : Window
    {
        private readonly IAuthService _authService;
        private readonly ISessionService _sessionService;

        public LoginWindow(IAuthService authService, ISessionService sessionService)
        {
            InitializeComponent();
            _authService = authService;
            _sessionService = sessionService;
        }

        private async void OnLoginClick(object sender, RoutedEventArgs e)
        {
            string usuario = txtUsuario.Text;
            string password = txtPassword.Password;

            // Validación simple
            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(password))
            {
                MostrarError("Por favor, complete todos los campos");
                return;
            }

            try
            {
                // Intentar autenticar
                var loginRequest = new LoginRequest
                {
                    NombreUsuario = usuario,
                    Contraseña = password
                };

                var usuarioAutenticado = await _authService.LoginAsync(loginRequest);

                if (usuarioAutenticado != null)
                {
                    // Iniciar sesión
                    await _sessionService.SignInAsync(usuarioAutenticado);

                    // Abrir ventana principal
                    var mainWindow = ((App)System.Windows.Application.Current).ServiceProvider.GetRequiredService<MainWindow>();
                    mainWindow.Show();

                    // Cerrar ventana de login
                    this.Close();
                }
                else
                {
                    MostrarError("Usuario o contraseña incorrectos, o usuario deshabilitado");
                }
            }
            catch (Exception ex)
            {
                MostrarError($"Error al iniciar sesión: {ex.Message}");
            }
        }

        private void MostrarError(string mensaje)
        {
            txtError.Text = mensaje;
            txtError.Visibility = Visibility.Visible;
        }
    }
}
