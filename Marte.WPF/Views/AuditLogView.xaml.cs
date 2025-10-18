using Marte.WPF.ViewModels;
using System.Windows;

namespace Marte.WPF.Views
{
    public partial class AuditLogView : Window
    {
        public AuditLogView(AuditLogViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
