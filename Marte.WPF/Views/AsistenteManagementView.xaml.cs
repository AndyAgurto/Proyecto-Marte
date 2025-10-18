using System.Windows;
using Marte.WPF.ViewModels;

namespace Marte.WPF.Views
{
    public partial class AsistenteManagementView : Window
    {
        public AsistenteManagementView(AsistenteManagementViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
