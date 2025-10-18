using System.Windows;

namespace Marte.WPF.Views
{
    public partial class ChangePasswordDialog : Window
    {
        public string NewPassword { get; private set; } = string.Empty;

        public ChangePasswordDialog(string username)
        {
            InitializeComponent();
            txtUsuario.Text = username;
        }

        private void OnAceptarClick(object sender, RoutedEventArgs e)
        {
            txtError.Visibility = Visibility.Collapsed;

            var nuevaContraseña = txtNuevaContraseña.Password;
            var confirmarContraseña = txtConfirmarContraseña.Password;

            // Validaciones
            if (string.IsNullOrWhiteSpace(nuevaContraseña))
            {
                txtError.Text = "⚠️ La contraseña no puede estar vacía.";
                txtError.Visibility = Visibility.Visible;
                return;
            }

            if (nuevaContraseña.Length < 6)
            {
                txtError.Text = "⚠️ La contraseña debe tener al menos 6 caracteres.";
                txtError.Visibility = Visibility.Visible;
                return;
            }

            if (nuevaContraseña != confirmarContraseña)
            {
                txtError.Text = "⚠️ Las contraseñas no coinciden.";
                txtError.Visibility = Visibility.Visible;
                return;
            }

            // Todo OK
            NewPassword = nuevaContraseña;
            DialogResult = true;
            Close();
        }

        private void OnCancelarClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
