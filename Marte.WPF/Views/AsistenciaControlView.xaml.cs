using System.Windows;
using Marte.WPF.ViewModels;

namespace Marte.WPF.Views
{
    public partial class AsistenciaControlView : Window
    {
        public AsistenciaControlView(AsistenciaControlViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
