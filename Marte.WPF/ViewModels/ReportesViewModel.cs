using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Marte.Application.Interfaces;
using Marte.Application.ViewModels;
using Marte.Domain.Entities;
using Marte.Domain.DTOs;

namespace Marte.WPF.ViewModels
{
    public class ReportesViewModel : ViewModelBase
    {
        private readonly IReporteService _reporteService;
        private readonly string _currentUser;

        // Propiedades para filtros
        private DateTime _fechaInicio = DateTime.Today.AddMonths(-1);
        private DateTime _fechaFin = DateTime.Today;
        private string _dniBusqueda = string.Empty;
        private string _nombresBusqueda = string.Empty;
        private string _apellidosBusqueda = string.Empty;
        private TimeSpan _horaPuntualidad = new TimeSpan(19, 30, 0);
        private int _topConstantes = 10;

        // Collections para reportes básicos
        private ObservableCollection<ReporteTotalDiario> _totalesDiarios;
        private ObservableCollection<ReportePorCategoria> _totalesPorCategoria;
        private ObservableCollection<ReportePorGrupo> _totalesPorGrupo;
        private ObservableCollection<ReporteHistorialIndividual> _historialIndividual;

        // Collections para paneles analíticos
        private ObservableCollection<ReportePuntualidad> _reportePuntualidad;
        private ObservableCollection<ReporteHastaCierre> _reporteHastaCierre;
        private ObservableCollection<ReporteAsistenciaGrupoDetalle> _reportePorGrupoDetalle;
        private ObservableCollection<ReporteTopConstantes> _topAsistentesConstantes;
        private ReporteDiaMayorAsistencia? _diaMayorAsistencia;

        // Estado UI
        private bool _isBusy;
        private string _statusMessage = string.Empty;

        public ReportesViewModel(IReporteService reporteService, string currentUser)
        {
            _reporteService = reporteService;
            _currentUser = currentUser;

            _totalesDiarios = new ObservableCollection<ReporteTotalDiario>();
            _totalesPorCategoria = new ObservableCollection<ReportePorCategoria>();
            _totalesPorGrupo = new ObservableCollection<ReportePorGrupo>();
            _historialIndividual = new ObservableCollection<ReporteHistorialIndividual>();
            _reportePuntualidad = new ObservableCollection<ReportePuntualidad>();
            _reporteHastaCierre = new ObservableCollection<ReporteHastaCierre>();
            _reportePorGrupoDetalle = new ObservableCollection<ReporteAsistenciaGrupoDetalle>();
            _topAsistentesConstantes = new ObservableCollection<ReporteTopConstantes>();

            // Comandos de consulta
            ConsultarTotalesDiariosCommand = new RelayCommand(() => ExecuteConsultarTotalesDiarios(null));
            ConsultarPorCategoriaCommand = new RelayCommand(() => ExecuteConsultarPorCategoria(null));
            ConsultarPorGrupoCommand = new RelayCommand(() => ExecuteConsultarPorGrupo(null));
            ConsultarHistorialPorDNICommand = new RelayCommand(() => ExecuteConsultarHistorialPorDNI(null), () => !string.IsNullOrWhiteSpace(DNIBusqueda));
            ConsultarHistorialPorNombreCommand = new RelayCommand(() => ExecuteConsultarHistorialPorNombre(null), () => CanConsultarPorNombre(null));

            // Comandos de paneles analíticos
            ConsultarPuntualidadCommand = new RelayCommand(() => ExecuteConsultarPuntualidad(null));
            ConsultarHastaCierreCommand = new RelayCommand(() => ExecuteConsultarHastaCierre(null));
            ConsultarPorGrupoDetalleCommand = new RelayCommand(() => ExecuteConsultarPorGrupoDetalle(null));
            ConsultarDiaMayorAsistenciaCommand = new RelayCommand(() => ExecuteConsultarDiaMayorAsistencia(null));
            ConsultarTopConstantesCommand = new RelayCommand(() => ExecuteConsultarTopConstantes(null));

            // Comandos de exportación
            ExportarTotalesDiariosExcelCommand = new RelayCommand(() => ExecuteExportarTotalesDiariosExcel(null));
            ExportarTotalesDiariosPDFCommand = new RelayCommand(() => ExecuteExportarTotalesDiariosPDF(null));
            ExportarPorCategoriaExcelCommand = new RelayCommand(() => ExecuteExportarPorCategoriaExcel(null));
            ExportarPorCategoriaPDFCommand = new RelayCommand(() => ExecuteExportarPorCategoriaPDF(null));
            ExportarHistorialIndividualExcelCommand = new RelayCommand(() => ExecuteExportarHistorialExcel(null), () => !string.IsNullOrWhiteSpace(DNIBusqueda));
            ExportarHistorialIndividualPDFCommand = new RelayCommand(() => ExecuteExportarHistorialPDF(null), () => !string.IsNullOrWhiteSpace(DNIBusqueda));
            
            ExportarPuntualidadExcelCommand = new RelayCommand(() => ExecuteExportarPuntualidadExcel(null));
            ExportarHastaCierreExcelCommand = new RelayCommand(() => ExecuteExportarHastaCierreExcel(null));
            ExportarPorGrupoDetalleExcelCommand = new RelayCommand(() => ExecuteExportarPorGrupoDetalleExcel(null));
            ExportarTopConstantesExcelCommand = new RelayCommand(() => ExecuteExportarTopConstantesExcel(null));
        }

        #region Properties

        public DateTime FechaInicio
        {
            get => _fechaInicio;
            set => SetProperty(ref _fechaInicio, value);
        }

        public DateTime FechaFin
        {
            get => _fechaFin;
            set => SetProperty(ref _fechaFin, value);
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

        public string NombresBusqueda
        {
            get => _nombresBusqueda;
            set
            {
                SetProperty(ref _nombresBusqueda, value);
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            }
        }

        public string ApellidosBusqueda
        {
            get => _apellidosBusqueda;
            set
            {
                SetProperty(ref _apellidosBusqueda, value);
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            }
        }

        public TimeSpan HoraPuntualidad
        {
            get => _horaPuntualidad;
            set => SetProperty(ref _horaPuntualidad, value);
        }

        public int TopConstantes
        {
            get => _topConstantes;
            set => SetProperty(ref _topConstantes, value);
        }

        public ObservableCollection<ReporteTotalDiario> TotalesDiarios
        {
            get => _totalesDiarios;
            set => SetProperty(ref _totalesDiarios, value);
        }

        public ObservableCollection<ReportePorCategoria> TotalesPorCategoria
        {
            get => _totalesPorCategoria;
            set => SetProperty(ref _totalesPorCategoria, value);
        }

        public ObservableCollection<ReportePorGrupo> TotalesPorGrupo
        {
            get => _totalesPorGrupo;
            set => SetProperty(ref _totalesPorGrupo, value);
        }

        public ObservableCollection<ReporteHistorialIndividual> HistorialIndividual
        {
            get => _historialIndividual;
            set => SetProperty(ref _historialIndividual, value);
        }

        public ObservableCollection<ReportePuntualidad> ReportePuntualidad
        {
            get => _reportePuntualidad;
            set => SetProperty(ref _reportePuntualidad, value);
        }

        public ObservableCollection<ReporteHastaCierre> ReporteHastaCierre
        {
            get => _reporteHastaCierre;
            set => SetProperty(ref _reporteHastaCierre, value);
        }

        public ObservableCollection<ReporteAsistenciaGrupoDetalle> ReportePorGrupoDetalle
        {
            get => _reportePorGrupoDetalle;
            set => SetProperty(ref _reportePorGrupoDetalle, value);
        }

        public ObservableCollection<ReporteTopConstantes> TopAsistentesConstantes
        {
            get => _topAsistentesConstantes;
            set => SetProperty(ref _topAsistentesConstantes, value);
        }

        public ReporteDiaMayorAsistencia? DiaMayorAsistencia
        {
            get => _diaMayorAsistencia;
            set => SetProperty(ref _diaMayorAsistencia, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        #endregion

        #region Commands

        public ICommand ConsultarTotalesDiariosCommand { get; }
        public ICommand ConsultarPorCategoriaCommand { get; }
        public ICommand ConsultarPorGrupoCommand { get; }
        public ICommand ConsultarHistorialPorDNICommand { get; }
        public ICommand ConsultarHistorialPorNombreCommand { get; }

        public ICommand ConsultarPuntualidadCommand { get; }
        public ICommand ConsultarHastaCierreCommand { get; }
        public ICommand ConsultarPorGrupoDetalleCommand { get; }
        public ICommand ConsultarDiaMayorAsistenciaCommand { get; }
        public ICommand ConsultarTopConstantesCommand { get; }

        public ICommand ExportarTotalesDiariosExcelCommand { get; }
        public ICommand ExportarTotalesDiariosPDFCommand { get; }
        public ICommand ExportarPorCategoriaExcelCommand { get; }
        public ICommand ExportarPorCategoriaPDFCommand { get; }
        public ICommand ExportarHistorialIndividualExcelCommand { get; }
        public ICommand ExportarHistorialIndividualPDFCommand { get; }
        public ICommand ExportarPuntualidadExcelCommand { get; }
        public ICommand ExportarHastaCierreExcelCommand { get; }
        public ICommand ExportarPorGrupoDetalleExcelCommand { get; }
        public ICommand ExportarTopConstantesExcelCommand { get; }

        #endregion

        #region Command Handlers - Consultas Básicas

        private async void ExecuteConsultarTotalesDiarios(object? parameter)
        {
            try
            {
                IsBusy = true;
                StatusMessage = "Consultando totales diarios...";

                var datos = await _reporteService.GetTotalesDiariosAsync(FechaInicio, FechaFin);
                
                TotalesDiarios.Clear();
                foreach (var item in datos)
                {
                    TotalesDiarios.Add(item);
                }

                StatusMessage = $"Se encontraron {TotalesDiarios.Count} registros";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al consultar totales diarios: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusMessage = "Error en la consulta";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void ExecuteConsultarPorCategoria(object? parameter)
        {
            try
            {
                IsBusy = true;
                StatusMessage = "Consultando por categoría...";

                var datos = await _reporteService.GetTotalesPorCategoriaAsync(FechaInicio, FechaFin);
                
                TotalesPorCategoria.Clear();
                foreach (var item in datos)
                {
                    TotalesPorCategoria.Add(item);
                }

                StatusMessage = $"Se encontraron {TotalesPorCategoria.Count} categorías";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al consultar por categoría: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusMessage = "Error en la consulta";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void ExecuteConsultarPorGrupo(object? parameter)
        {
            try
            {
                IsBusy = true;
                StatusMessage = "Consultando por grupo...";

                var datos = await _reporteService.GetTotalesPorGrupoAsync(FechaInicio, FechaFin);
                
                TotalesPorGrupo.Clear();
                foreach (var item in datos)
                {
                    TotalesPorGrupo.Add(item);
                }

                StatusMessage = $"Se encontraron {TotalesPorGrupo.Count} grupos";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al consultar por grupo: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusMessage = "Error en la consulta";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void ExecuteConsultarHistorialPorDNI(object? parameter)
        {
            try
            {
                IsBusy = true;
                StatusMessage = $"Consultando historial de DNI: {DNIBusqueda}...";

                var datos = await _reporteService.GetHistorialIndividualPorDNIAsync(DNIBusqueda, FechaInicio, FechaFin);
                
                HistorialIndividual.Clear();
                foreach (var item in datos)
                {
                    HistorialIndividual.Add(item);
                }

                if (HistorialIndividual.Count == 0)
                {
                    MessageBox.Show($"No se encontró historial para el DNI: {DNIBusqueda}", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                    StatusMessage = "No se encontraron registros";
                }
                else
                {
                    StatusMessage = $"Total de asistencias: {HistorialIndividual.First().TotalDias}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al consultar historial: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusMessage = "Error en la consulta";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool CanConsultarPorNombre(object? parameter)
        {
            return !string.IsNullOrWhiteSpace(NombresBusqueda) && !string.IsNullOrWhiteSpace(ApellidosBusqueda);
        }

        private async void ExecuteConsultarHistorialPorNombre(object? parameter)
        {
            try
            {
                IsBusy = true;
                StatusMessage = $"Consultando historial de {NombresBusqueda} {ApellidosBusqueda}...";

                var datos = await _reporteService.GetHistorialIndividualPorNombreAsync(NombresBusqueda, ApellidosBusqueda, FechaInicio, FechaFin);
                
                HistorialIndividual.Clear();
                foreach (var item in datos)
                {
                    HistorialIndividual.Add(item);
                }

                if (HistorialIndividual.Count == 0)
                {
                    MessageBox.Show($"No se encontró historial para: {NombresBusqueda} {ApellidosBusqueda}", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                    StatusMessage = "No se encontraron registros";
                }
                else
                {
                    StatusMessage = $"Total de asistencias: {HistorialIndividual.First().TotalDias}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al consultar historial: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusMessage = "Error en la consulta";
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion

        #region Command Handlers - Paneles Analíticos

        private async void ExecuteConsultarPuntualidad(object? parameter)
        {
            try
            {
                IsBusy = true;
                StatusMessage = "Consultando reporte de puntualidad...";

                var datos = await _reporteService.GetReportePuntualidadAsync(FechaInicio, FechaFin, HoraPuntualidad);
                
                ReportePuntualidad.Clear();
                foreach (var item in datos)
                {
                    ReportePuntualidad.Add(item);
                }

                StatusMessage = $"Se encontraron {ReportePuntualidad.Count} asistentes";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al consultar puntualidad: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusMessage = "Error en la consulta";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void ExecuteConsultarHastaCierre(object? parameter)
        {
            try
            {
                IsBusy = true;
                StatusMessage = "Consultando asistentes hasta cierre...";

                var datos = await _reporteService.GetReporteHastaCierreAsync(FechaInicio, FechaFin);
                
                ReporteHastaCierre.Clear();
                foreach (var item in datos)
                {
                    ReporteHastaCierre.Add(item);
                }

                StatusMessage = $"Se encontraron {ReporteHastaCierre.Count} asistentes";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al consultar hasta cierre: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusMessage = "Error en la consulta";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void ExecuteConsultarPorGrupoDetalle(object? parameter)
        {
            try
            {
                IsBusy = true;
                StatusMessage = "Consultando asistencia por grupo...";

                var datos = await _reporteService.GetReporteAsistenciaPorGrupoAsync(FechaInicio, FechaFin);
                
                ReportePorGrupoDetalle.Clear();
                foreach (var item in datos)
                {
                    ReportePorGrupoDetalle.Add(item);
                }

                StatusMessage = $"Se encontraron {ReportePorGrupoDetalle.Count} registros";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al consultar por grupo: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusMessage = "Error en la consulta";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void ExecuteConsultarDiaMayorAsistencia(object? parameter)
        {
            try
            {
                IsBusy = true;
                StatusMessage = "Consultando día de mayor asistencia...";

                var resultado = await _reporteService.GetDiaMayorAsistenciaAsync(FechaInicio, FechaFin);
                
                DiaMayorAsistencia = resultado;

                if (DiaMayorAsistencia == null)
                {
                    StatusMessage = "No se encontraron registros en el período";
                }
                else
                {
                    StatusMessage = $"Día con mayor asistencia: {DiaMayorAsistencia.Fecha:dd/MM/yyyy} ({DiaMayorAsistencia.TotalAsistencias} asist.)";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al consultar día mayor asistencia: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusMessage = "Error en la consulta";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void ExecuteConsultarTopConstantes(object? parameter)
        {
            try
            {
                IsBusy = true;
                StatusMessage = $"Consultando top {TopConstantes} asistentes más constantes...";

                var datos = await _reporteService.GetTopAsistentesConstantesAsync(FechaInicio, FechaFin, TopConstantes);
                
                TopAsistentesConstantes.Clear();
                foreach (var item in datos)
                {
                    TopAsistentesConstantes.Add(item);
                }

                StatusMessage = $"Se encontraron {TopAsistentesConstantes.Count} asistentes";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al consultar top constantes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusMessage = "Error en la consulta";
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion

        #region Command Handlers - Exportación

        private async void ExecuteExportarTotalesDiariosExcel(object? parameter)
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    FileName = $"TotalesDiarios_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                    Filter = "Archivos Excel (*.xlsx)|*.xlsx",
                    Title = "Exportar Totales Diarios a Excel"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    IsBusy = true;
                    StatusMessage = "Exportando a Excel...";

                    await _reporteService.ExportarTotalesDiariosExcelAsync(FechaInicio, FechaFin, saveDialog.FileName);
                    
                    MessageBox.Show($"Reporte exportado exitosamente:\n{saveDialog.FileName}", "Exportación Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
                    StatusMessage = "Exportación completada";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusMessage = "Error en la exportación";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void ExecuteExportarTotalesDiariosPDF(object? parameter)
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    FileName = $"TotalesDiarios_{DateTime.Now:yyyyMMdd_HHmmss}.pdf",
                    Filter = "Archivos PDF (*.pdf)|*.pdf",
                    Title = "Exportar Totales Diarios a PDF"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    IsBusy = true;
                    StatusMessage = "Exportando a PDF...";

                    await _reporteService.ExportarTotalesDiariosPDFAsync(FechaInicio, FechaFin, saveDialog.FileName);
                    
                    MessageBox.Show($"Reporte exportado exitosamente:\n{saveDialog.FileName}", "Exportación Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
                    StatusMessage = "Exportación completada";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusMessage = "Error en la exportación";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void ExecuteExportarPorCategoriaExcel(object? parameter)
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    FileName = $"PorCategoria_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                    Filter = "Archivos Excel (*.xlsx)|*.xlsx",
                    Title = "Exportar Por Categoría a Excel"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    IsBusy = true;
                    StatusMessage = "Exportando a Excel...";

                    await _reporteService.ExportarPorCategoriaExcelAsync(FechaInicio, FechaFin, saveDialog.FileName);
                    
                    MessageBox.Show($"Reporte exportado exitosamente:\n{saveDialog.FileName}", "Exportación Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
                    StatusMessage = "Exportación completada";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusMessage = "Error en la exportación";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void ExecuteExportarPorCategoriaPDF(object? parameter)
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    FileName = $"PorCategoria_{DateTime.Now:yyyyMMdd_HHmmss}.pdf",
                    Filter = "Archivos PDF (*.pdf)|*.pdf",
                    Title = "Exportar Por Categoría a PDF"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    IsBusy = true;
                    StatusMessage = "Exportando a PDF...";

                    await _reporteService.ExportarPorCategoriaPDFAsync(FechaInicio, FechaFin, saveDialog.FileName);
                    
                    MessageBox.Show($"Reporte exportado exitosamente:\n{saveDialog.FileName}", "Exportación Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
                    StatusMessage = "Exportación completada";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusMessage = "Error en la exportación";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void ExecuteExportarHistorialExcel(object? parameter)
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    FileName = $"Historial_{DNIBusqueda}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                    Filter = "Archivos Excel (*.xlsx)|*.xlsx",
                    Title = "Exportar Historial Individual a Excel"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    IsBusy = true;
                    StatusMessage = "Exportando a Excel...";

                    await _reporteService.ExportarHistorialIndividualExcelAsync(DNIBusqueda, FechaInicio, FechaFin, saveDialog.FileName);
                    
                    MessageBox.Show($"Reporte exportado exitosamente:\n{saveDialog.FileName}", "Exportación Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
                    StatusMessage = "Exportación completada";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusMessage = "Error en la exportación";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void ExecuteExportarHistorialPDF(object? parameter)
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    FileName = $"Historial_{DNIBusqueda}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf",
                    Filter = "Archivos PDF (*.pdf)|*.pdf",
                    Title = "Exportar Historial Individual a PDF"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    IsBusy = true;
                    StatusMessage = "Exportando a PDF...";

                    await _reporteService.ExportarHistorialIndividualPDFAsync(DNIBusqueda, FechaInicio, FechaFin, saveDialog.FileName);
                    
                    MessageBox.Show($"Reporte exportado exitosamente:\n{saveDialog.FileName}", "Exportación Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
                    StatusMessage = "Exportación completada";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusMessage = "Error en la exportación";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void ExecuteExportarPuntualidadExcel(object? parameter)
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    FileName = $"Puntualidad_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                    Filter = "Archivos Excel (*.xlsx)|*.xlsx",
                    Title = "Exportar Reporte de Puntualidad a Excel"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    IsBusy = true;
                    StatusMessage = "Exportando a Excel...";

                    await _reporteService.ExportarPuntualidadExcelAsync(FechaInicio, FechaFin, HoraPuntualidad, saveDialog.FileName);
                    
                    MessageBox.Show($"Reporte exportado exitosamente:\n{saveDialog.FileName}", "Exportación Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
                    StatusMessage = "Exportación completada";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusMessage = "Error en la exportación";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void ExecuteExportarHastaCierreExcel(object? parameter)
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    FileName = $"HastaCierre_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                    Filter = "Archivos Excel (*.xlsx)|*.xlsx",
                    Title = "Exportar Reporte Hasta Cierre a Excel"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    IsBusy = true;
                    StatusMessage = "Exportando a Excel...";

                    await _reporteService.ExportarHastaCierreExcelAsync(FechaInicio, FechaFin, saveDialog.FileName);
                    
                    MessageBox.Show($"Reporte exportado exitosamente:\n{saveDialog.FileName}", "Exportación Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
                    StatusMessage = "Exportación completada";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusMessage = "Error en la exportación";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void ExecuteExportarPorGrupoDetalleExcel(object? parameter)
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    FileName = $"PorGrupo_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                    Filter = "Archivos Excel (*.xlsx)|*.xlsx",
                    Title = "Exportar Asistencia Por Grupo a Excel"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    IsBusy = true;
                    StatusMessage = "Exportando a Excel...";

                    await _reporteService.ExportarAsistenciaPorGrupoExcelAsync(FechaInicio, FechaFin, saveDialog.FileName);
                    
                    MessageBox.Show($"Reporte exportado exitosamente:\n{saveDialog.FileName}", "Exportación Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
                    StatusMessage = "Exportación completada";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusMessage = "Error en la exportación";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void ExecuteExportarTopConstantesExcel(object? parameter)
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    FileName = $"TopConstantes_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                    Filter = "Archivos Excel (*.xlsx)|*.xlsx",
                    Title = "Exportar Top Asistentes Constantes a Excel"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    IsBusy = true;
                    StatusMessage = "Exportando a Excel...";

                    await _reporteService.ExportarTopConstantesExcelAsync(FechaInicio, FechaFin, TopConstantes, saveDialog.FileName);
                    
                    MessageBox.Show($"Reporte exportado exitosamente:\n{saveDialog.FileName}", "Exportación Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
                    StatusMessage = "Exportación completada";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusMessage = "Error en la exportación";
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion
    }
}
