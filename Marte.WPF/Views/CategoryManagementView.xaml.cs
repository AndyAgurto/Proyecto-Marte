using System.Windows;
using Marte.Application.Interfaces;
using Marte.WPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Marte.WPF.Views
{
    public partial class CategoryManagementView : Window
    {
        public CategoryManagementView()
        {
            InitializeComponent();
            
            var serviceProvider = ((App)System.Windows.Application.Current).ServiceProvider;
            var categoriaService = serviceProvider.GetRequiredService<ICategoriaService>();
            
            DataContext = new CategoryManagementViewModel(categoriaService);
        }
    }
}
