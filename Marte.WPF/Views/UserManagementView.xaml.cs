using System.Windows;
using System.Windows.Controls;
using Marte.Application.Interfaces;
using Marte.Infrastructure.Interfaces;
using Marte.WPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Marte.WPF.Views
{
    public partial class UserManagementView : Window
    {
        private UserManagementViewModel _viewModel;

        public UserManagementView(string currentUser)
        {
            InitializeComponent();

            // Obtener servicios desde el DI container
            var userManagementService = ((App)System.Windows.Application.Current).ServiceProvider.GetRequiredService<IUserManagementService>();
            var rolRepository = ((App)System.Windows.Application.Current).ServiceProvider.GetRequiredService<IRolRepository>();

            _viewModel = new UserManagementViewModel(userManagementService, rolRepository, currentUser);
            DataContext = _viewModel;

            // Conectar eventos de PasswordBox
            txtContraseña.PasswordChanged += TxtContraseña_PasswordChanged;
            txtConfirmarContraseña.PasswordChanged += TxtConfirmarContraseña_PasswordChanged;
        }

        private void TxtContraseña_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is UserManagementViewModel viewModel)
            {
                viewModel.Contraseña = txtContraseña.Password;
            }
        }

        private void TxtConfirmarContraseña_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is UserManagementViewModel viewModel)
            {
                viewModel.ConfirmarContraseña = txtConfirmarContraseña.Password;
            }
        }
    }
}
