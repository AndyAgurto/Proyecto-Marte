using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Marte.Application.Interfaces;
using Marte.Application.ViewModels;

namespace Marte.WPF.ViewModels
{
    public class ConfigurationViewModel : INotifyPropertyChanged
    {
        private readonly IConfiguracionService _configuracionService;
        private string _nombreFilial = "N. A. Primavera";
        private int _horaCierreHora = 22;
        private int _horaCierreMinuto = 0;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ConfigurationViewModel(IConfiguracionService configuracionService)
        {
            _configuracionService = configuracionService;

            SaveConfigCommand = new RelayCommand(async () => await SaveConfigurationAsync(), CanSaveConfiguration);
            OpenUserManagementCommand = new RelayCommand(OpenUserManagement);
            OpenCategoryManagementCommand = new RelayCommand(OpenCategoryManagement);

            _ = LoadConfigurationAsync();
        }

        public string NombreFilial
        {
            get => _nombreFilial;
            set
            {
                _nombreFilial = value;
                OnPropertyChanged();
            }
        }

        public int HoraCierreHora
        {
            get => _horaCierreHora;
            set
            {
                if (value >= 0 && value <= 23)
                {
                    _horaCierreHora = value;
                    OnPropertyChanged();
                }
            }
        }

        public int HoraCierreMinuto
        {
            get => _horaCierreMinuto;
            set
            {
                if (value >= 0 && value <= 59)
                {
                    _horaCierreMinuto = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand SaveConfigCommand { get; }
        public ICommand OpenUserManagementCommand { get; }
        public ICommand OpenCategoryManagementCommand { get; }

        private async Task LoadConfigurationAsync()
        {
            try
            {
                var config = await _configuracionService.GetConfiguracionAsync();
                if (config != null)
                {
                    NombreFilial = config.NombreFilial;
                    HoraCierreHora = config.HoraCierre.Hours;
                    HoraCierreMinuto = config.HoraCierre.Minutes;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar configuración: {ex.Message}", 
                              "Error", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Error);
            }
        }

        private async Task SaveConfigurationAsync()
        {
            try
            {
                var horaCierre = new TimeSpan(HoraCierreHora, HoraCierreMinuto, 0);
                await _configuracionService.UpdateHoraCierreAsync(horaCierre, NombreFilial);

                MessageBox.Show("Configuración guardada exitosamente.", 
                              "Éxito", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar configuración: {ex.Message}", 
                              "Error", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Error);
            }
        }

        private bool CanSaveConfiguration()
        {
            return !string.IsNullOrWhiteSpace(NombreFilial);
        }

        private void OpenUserManagement()
        {
            var userManagementView = new Views.UserManagementView("Administrador");
            userManagementView.ShowDialog();
        }

        private void OpenCategoryManagement()
        {
            var categoryManagementView = new Views.CategoryManagementView();
            categoryManagementView.ShowDialog();
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
