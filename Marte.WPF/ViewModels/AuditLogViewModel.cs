using Marte.Application.Interfaces;
using Marte.Application.ViewModels;
using Marte.Domain.Entities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Marte.WPF.ViewModels
{
    public class AuditLogViewModel : INotifyPropertyChanged
    {
        private readonly IAuditLogService _auditLogService;
        private DateTime? _fechaInicio;
        private DateTime? _fechaFin;
        private string _usuarioFiltro = string.Empty;
        private ObservableCollection<AuditLog> _logs = new();
        private AuditLog? _selectedLog;
        private bool _isBusy;
        private string _totalRegistros = "0";

        public AuditLogViewModel(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
            
            // Establecer fechas por defecto (último mes)
            FechaFin = DateTime.Now;
            FechaInicio = DateTime.Now.AddMonths(-1);

            BuscarCommand = new RelayCommand(async () => await BuscarAsync());
            LimpiarFiltrosCommand = new RelayCommand(LimpiarFiltros);
            ExportarCommand = new RelayCommand(async () => await ExportarAsync(), () => Logs.Any());
            CargarTodosCommand = new RelayCommand(async () => await CargarTodosAsync());

            // Cargar datos iniciales
            _ = BuscarAsync();
        }

        public DateTime? FechaInicio
        {
            get => _fechaInicio;
            set
            {
                _fechaInicio = value;
                OnPropertyChanged();
            }
        }

        public DateTime? FechaFin
        {
            get => _fechaFin;
            set
            {
                _fechaFin = value;
                OnPropertyChanged();
            }
        }

        public string UsuarioFiltro
        {
            get => _usuarioFiltro;
            set
            {
                _usuarioFiltro = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<AuditLog> Logs
        {
            get => _logs;
            set
            {
                _logs = value;
                OnPropertyChanged();
                UpdateTotalRegistros();
            }
        }

        public AuditLog? SelectedLog
        {
            get => _selectedLog;
            set
            {
                _selectedLog = value;
                OnPropertyChanged();
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        public string TotalRegistros
        {
            get => _totalRegistros;
            set
            {
                _totalRegistros = value;
                OnPropertyChanged();
            }
        }

        public ICommand BuscarCommand { get; }
        public ICommand LimpiarFiltrosCommand { get; }
        public ICommand ExportarCommand { get; }
        public ICommand CargarTodosCommand { get; }

        private async Task BuscarAsync()
        {
            try
            {
                IsBusy = true;

                var resultado = await _auditLogService.GetByFiltrosAsync(
                    FechaInicio,
                    FechaFin,
                    string.IsNullOrWhiteSpace(UsuarioFiltro) ? null : UsuarioFiltro
                );

                Logs = new ObservableCollection<AuditLog>(resultado);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar registros: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void LimpiarFiltros()
        {
            FechaInicio = DateTime.Now.AddMonths(-1);
            FechaFin = DateTime.Now;
            UsuarioFiltro = string.Empty;
        }

        private async Task CargarTodosAsync()
        {
            try
            {
                IsBusy = true;

                var resultado = await _auditLogService.GetAllAsync();
                Logs = new ObservableCollection<AuditLog>(resultado);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar todos los registros: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ExportarAsync()
        {
            try
            {
                if (!Logs.Any())
                {
                    MessageBox.Show("No hay registros para exportar.", "Información",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Archivos de texto|*.txt",
                    FileName = $"AuditoriaMarte_{DateTime.Now:yyyyMMdd_HHmmss}.txt",
                    DefaultExt = ".txt"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    IsBusy = true;
                    await _auditLogService.ExportarATxtAsync(Logs, saveDialog.FileName);
                    
                    MessageBox.Show($"Bitácora exportada exitosamente:\n{saveDialog.FileName}", 
                        "Exportación Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void UpdateTotalRegistros()
        {
            TotalRegistros = $"{Logs.Count} registro(s)";
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
