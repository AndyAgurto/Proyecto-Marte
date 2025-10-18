using System.Windows;
using Marte.WPF.ViewModels;

namespace Marte.WPF.Views
{
    public partial class ReportesView : Window
    {
        public ReportesView(ReportesViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
