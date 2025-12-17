using System.Windows;

namespace Marte.WPF.Views
{
    public partial class RegistroTemporalDialog : Window
    {
        public string Nombres { get; private set; } = string.Empty;
        public string Apellidos { get; private set; } = string.Empty;

        public RegistroTemporalDialog()
        {
            InitializeComponent();
            txtNombres.Focus();
        }

        private void BtnRegistrar_Click(object sender, RoutedEventArgs e)
        {
            txtError.Text = string.Empty;

            var nombres = txtNombres.Text.Trim();
            var apellidos = txtApellidos.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombres))
            {
                txtError.Text = "⚠️ El nombre es obligatorio.";
                txtNombres.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(apellidos))
            {
                txtError.Text = "⚠️ Los apellidos son obligatorios.";
                txtApellidos.Focus();
                return;
            }

            Nombres = nombres;
            Apellidos = apellidos;
            DialogResult = true;
            Close();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
