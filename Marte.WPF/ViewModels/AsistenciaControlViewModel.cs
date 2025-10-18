using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Marte.Application.Interfaces;
using Marte.Application.ViewModels;
using Marte.Domain.Entities;

namespace Marte.WPF.ViewModels
{
    public class AsistenciaControlViewModel : ViewModelBase
    {
        private readonly IAsistenciaService _asistenciaService;
        private readonly string _currentUser;
        private readonly DispatcherTimer _refreshTimer;

        private ObservableCollection<Asistencia> _asistenciasPresentes;
        private ObservableCollection<Asistencia> _historialDia;
        private Asistencia? _selectedAsistenciaPresente;
        private string _dniBusqueda = string.Empty;
        private string _observacionSalida = string.Empty;
        private int _totalPresentes;

        public AsistenciaControlViewModel(
            IAsistenciaService asistenciaService,
            string currentUser)
        {
            _asistenciaService = asistenciaService;
            _currentUser = currentUser;

            _asistenciasPresentes = new ObservableCollection<Asistencia>();
            _historialDia = new ObservableCollection<Asistencia>();

            // Comandos
            RegistrarIngresoCommand = new RelayCommand(() => ExecuteRegistrarIngreso(null), () => CanRegistrarIngreso(null));
            RegistrarSalidaCommand = new RelayCommand(() => ExecuteRegistrarSalida(null), () => CanRegistrarSalida(null));
            AplicarCierreCommand = new RelayCommand(() => ExecuteAplicarCierre(null));
            ActualizarCommand = new RelayCommand(() => ExecuteActualizar(null));

            // Timer para actualización automática cada 30 segundos
            _refreshTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(30)
            };
            _refreshTimer.Tick += async (s, e) => await LoadAsistenciasPresentes();
            _refreshTimer.Start();

            _ = LoadData();
        }

        #region Properties

        public ObservableCollection<Asistencia> AsistenciasPresentes
        {
            get => _asistenciasPresentes;
            set => SetProperty(ref _asistenciasPresentes, value);
        }

        public ObservableCollection<Asistencia> HistorialDia
        {
            get => _historialDia;
            set => SetProperty(ref _historialDia, value);
        }

        public Asistencia? SelectedAsistenciaPresente
        {
            get => _selectedAsistenciaPresente;
            set
            {
                SetProperty(ref _selectedAsistenciaPresente, value);
                OnPropertyChanged(nameof(CanSalir));
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            }
        }

        public string DNIBusqueda
        {
            get => _dniBusqueda;
            set
            {
                SetProperty(ref _dniBusqueda, value);
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            }
        }

        public string ObservacionSalida
        {
            get => _observacionSalida;
            set => SetProperty(ref _observacionSalida, value);
        }

        public int TotalPresentes
        {
            get => _totalPresentes;
            set => SetProperty(ref _totalPresentes, value);
        }

        public bool CanSalir => SelectedAsistenciaPresente != null;

        #endregion

        #region Commands

        public ICommand RegistrarIngresoCommand { get; }
        public ICommand RegistrarSalidaCommand { get; }
        public ICommand AplicarCierreCommand { get; }
        public ICommand ActualizarCommand { get; }

        #endregion

        #region Command Handlers

        private bool CanRegistrarIngreso(object? parameter)
        {
            return !string.IsNullOrWhiteSpace(DNIBusqueda);
        }

        private async void ExecuteRegistrarIngreso(object? parameter)
        {
            try
            {
                var result = await _asistenciaService.RegistrarIngresoAsync(DNIBusqueda.Trim(), _currentUser);

                if (result.Success)
                {
                    MessageBox.Show(result.Message, "Ingreso Registrado", MessageBoxButton.OK, MessageBoxImage.Information);
                    DNIBusqueda = string.Empty;
                    await LoadAsistenciasPresentes();
                    await LoadHistorialDia();
                }
                else
                {
                    MessageBox.Show(result.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al registrar ingreso: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanRegistrarSalida(object? parameter)
        {
            return CanSalir;
        }

        private async void ExecuteRegistrarSalida(object? parameter)
        {
            if (SelectedAsistenciaPresente == null) return;

            try
            {
                var asistente = SelectedAsistenciaPresente.Asistente;
                var result = MessageBox.Show(
                    $"¿Registrar salida para {asistente.Nombres} {asistente.Apellidos}?\n\nIngreso: {SelectedAsistenciaPresente.HoraIngreso:HH:mm}",
                    "Confirmar Salida",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    var salidaResult = await _asistenciaService.RegistrarSalidaAsync(
                        SelectedAsistenciaPresente.Id, 
                        _currentUser,
                        string.IsNullOrWhiteSpace(ObservacionSalida) ? null : ObservacionSalida);

                    if (salidaResult.Success)
                    {
                        MessageBox.Show(salidaResult.Message, "Salida Registrada", MessageBoxButton.OK, MessageBoxImage.Information);
                        ObservacionSalida = string.Empty;
                        SelectedAsistenciaPresente = null;
                        await LoadAsistenciasPresentes();
                        await LoadHistorialDia();
                    }
                    else
                    {
                        MessageBox.Show(salidaResult.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al registrar salida: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ExecuteAplicarCierre(object? parameter)
        {
            try
            {
                if (TotalPresentes == 0)
                {
                    MessageBox.Show("No hay asistencias abiertas para aplicar cierre automático.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var result = MessageBox.Show(
                    $"¿Aplicar cierre automático a {TotalPresentes} asistencia(s) abierta(s)?\n\nSe registrará la hora de cierre configurada en el sistema.",
                    "Confirmar Cierre Automático",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    var cierreResult = await _asistenciaService.AplicarCierreAutomaticoAsync(_currentUser);

                    if (cierreResult.Success)
                    {
                        MessageBox.Show(cierreResult.Message, "Cierre Aplicado", MessageBoxButton.OK, MessageBoxImage.Information);
                        await LoadAsistenciasPresentes();
                        await LoadHistorialDia();
                    }
                    else
                    {
                        MessageBox.Show(cierreResult.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al aplicar cierre: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ExecuteActualizar(object? parameter)
        {
            await LoadAsistenciasPresentes();
            await LoadHistorialDia();
        }

        #endregion

        #region Helper Methods

        private async Task LoadData()
        {
            await LoadAsistenciasPresentes();
            await LoadHistorialDia();
        }

        private async Task LoadAsistenciasPresentes()
        {
            try
            {
                var presentes = await _asistenciaService.GetAsistenciasPresentesAsync();
                
                AsistenciasPresentes.Clear();
                foreach (var asistencia in presentes)
                {
                    AsistenciasPresentes.Add(asistencia);
                }

                TotalPresentes = AsistenciasPresentes.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar asistencias presentes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadHistorialDia()
        {
            try
            {
                // Siempre cargar el historial de hoy
                var historial = await _asistenciaService.GetAsistenciasByFechaAsync(DateTime.Today);
                
                HistorialDia.Clear();
                foreach (var asistencia in historial)
                {
                    HistorialDia.Add(asistencia);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar historial: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        public void Dispose()
        {
            _refreshTimer?.Stop();
        }
    }
}
