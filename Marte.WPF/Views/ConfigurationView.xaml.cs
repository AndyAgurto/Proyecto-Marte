using System.Windows;
using Marte.Application.Interfaces;
using Marte.WPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Marte.WPF.Views
{
    public partial class ConfigurationView : Window
    {
        public ConfigurationView()
        {
            InitializeComponent();
            
            var serviceProvider = ((App)System.Windows.Application.Current).ServiceProvider;
            var configuracionService = serviceProvider.GetRequiredService<IConfiguracionService>();
            
            DataContext = new ConfigurationViewModel(configuracionService);
        }
    }
}
