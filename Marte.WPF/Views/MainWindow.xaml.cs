using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Marte.Application.Interfaces;
using Marte.Infrastructure.Interfaces;
using Marte.WPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Marte.WPF.Views
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        private readonly ISessionService _sessionService;
        private readonly IAuthService _authService;
        private readonly IServiceProvider _serviceProvider;
        private readonly string? _currentUser;
        
        // Referencia a la ventana de Control de Asistencia
        private AsistenciaControlView? _asistenciaWindow;

        public MainWindow(ISessionService sessionService, IAuthService authService, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _sessionService = sessionService;
            _authService = authService;
            _serviceProvider = serviceProvider;
            _currentUser = _sessionService.CurrentUser?.NombreUsuario;
            
            InicializarReloj();
            CargarUsuario();
            _ = InicializarDashboardAsync(); // Carga asíncrona sin bloquear
        }

        private async Task InicializarDashboardAsync()
        {
            await CargarCategoriasAsync();
            await CargarDashboardAsync();
        }

        private void CargarUsuario()
        {
            var usuario = _sessionService.CurrentUser;
            if (usuario != null)
            {
                txtUsuarioActual.Text = usuario.NombreUsuario;
            }
        }

        private async Task CargarCategoriasAsync()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var categoriaService = scope.ServiceProvider.GetRequiredService<ICategoriaService>();
                var categorias = await categoriaService.GetAllCategoriasAsync();
                
                // Ejecutar en el thread UI
                await Dispatcher.InvokeAsync(() =>
                {
                    cmbCategoria.Items.Clear();
                    
                    // Agregar opción "Todas"
                    cmbCategoria.Items.Add(new ComboBoxItem 
                    { 
                        Content = "Todas las categorías", 
                        Tag = "TODAS" 
                    });
                    
                    foreach (var categoria in categorias)
                    {
                        cmbCategoria.Items.Add(new ComboBoxItem 
                        { 
                            Content = categoria.Nombre, 
                            Tag = categoria.Id.ToString() 
                        });
                    }
                    
                    // Seleccionar "Todas" por defecto
                    cmbCategoria.SelectedIndex = 0;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar categorías: {ex.Message}", 
                              "Error", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Warning);
            }
        }

        private async void OnCategoriaChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbCategoria.SelectedItem == null) return;
            
            var selectedItem = (ComboBoxItem)cmbCategoria.SelectedItem;
            var categoriaNombre = selectedItem.Content.ToString();
            
            // Mostrar/ocultar selector de grupo si es "Miembros"
            if (categoriaNombre != null && categoriaNombre.Contains("Miembro"))
            {
                pnlGrupo.Visibility = Visibility.Visible;
                await CargarGruposAsync();
            }
            else
            {
                pnlGrupo.Visibility = Visibility.Collapsed;
                await ActualizarContadorCategoriaAsync();
            }
        }

        private async Task CargarGruposAsync()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var asistenteService = scope.ServiceProvider.GetRequiredService<IAsistenteService>();
                var asistentes = await asistenteService.GetAllAsistentesAsync();
                
                // Obtener los grupos únicos de los miembros activos
                var gruposUnicos = asistentes
                    .Where(a => a.Estado && !string.IsNullOrWhiteSpace(a.NumeroGrupo))
                    .Select(a => a.NumeroGrupo)
                    .Distinct()
                    .OrderBy(g => g)
                    .ToList();
                
                await Dispatcher.InvokeAsync(() =>
                {
                    cmbGrupo.Items.Clear();
                    
                    foreach (var grupo in gruposUnicos)
                    {
                        cmbGrupo.Items.Add(new ComboBoxItem 
                        { 
                            Content = $"Grupo #{grupo}", 
                            Tag = grupo 
                        });
                    }
                    
                    // Seleccionar el primer grupo si existe
                    if (cmbGrupo.Items.Count > 0)
                    {
                        cmbGrupo.SelectedIndex = 0;
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar grupos: {ex.Message}", 
                              "Error", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Warning);
            }
        }

        private async void OnGrupoChanged(object sender, SelectionChangedEventArgs e)
        {
            await ActualizarContadorCategoriaAsync();
        }

        private async Task ActualizarContadorCategoriaAsync()
        {
            try
            {
                if (cmbCategoria.SelectedItem == null)
                {
                    txtCategoriaLabel.Text = "Seleccione una categoría";
                    txtCategoriaTotal.Text = "--";
                    return;
                }
                
                var selectedItem = (ComboBoxItem)cmbCategoria.SelectedItem;
                var categoriaTag = selectedItem.Tag.ToString();
                var categoriaNombre = selectedItem.Content.ToString();
                
                int total = 0;
                string label = "";
                
                using var scope = _serviceProvider.CreateScope();
                var asistenteService = scope.ServiceProvider.GetRequiredService<IAsistenteService>();
                var asistentes = await asistenteService.GetAllAsistentesAsync();
                
                if (categoriaTag == "TODAS")
                {
                    // Contar todos los asistentes activos
                    total = asistentes.Count(a => a.Estado);
                    label = "Total de asistentes";
                }
                else if (categoriaNombre != null && categoriaNombre.Contains("Miembro") && pnlGrupo.Visibility == Visibility.Visible)
                {
                    // Contar miembros por grupo específico
                    var grupoItem = (ComboBoxItem)cmbGrupo.SelectedItem;
                    var numeroGrupo = grupoItem?.Tag.ToString();
                    
                    total = asistentes.Count(a => a.Estado && 
                                                  a.CategoriaId == Guid.Parse(categoriaTag!) && 
                                                  a.NumeroGrupo == numeroGrupo);
                    label = $"Miembros Grupo #{numeroGrupo}";
                }
                else
                {
                    // Contar por categoría específica
                    total = asistentes.Count(a => a.Estado && a.CategoriaId == Guid.Parse(categoriaTag!));
                    label = categoriaNombre ?? "Categoría";
                }
                
                txtCategoriaLabel.Text = label;
                txtCategoriaTotal.Text = total.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar contador: {ex.Message}", 
                              "Error", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Warning);
                txtCategoriaLabel.Text = "Error al cargar";
                txtCategoriaTotal.Text = "--";
            }
        }

        private async Task CargarDashboardAsync()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dashboardService = scope.ServiceProvider.GetRequiredService<IDashboardService>();
                var stats = await dashboardService.GetDashboardStatsAsync();

                // Actualizar indicadores principales
                txtAsistentesHoy.Text = stats.AsistentesHoy > 0 ? stats.AsistentesHoy.ToString() : "0";
                txtHastaCierre.Text = stats.HastaCierreAyer > 0 ? stats.HastaCierreAyer.ToString() : "0";
                txtPuntualidad.Text = stats.PorcentajePuntualidad > 0 ? $"{stats.PorcentajePuntualidad}%" : "0%";
                txtTotalRegistrados.Text = stats.TotalRegistrados > 0 ? stats.TotalRegistrados.ToString() : "0";

                // Actualizar Top 5
                ActualizarTop5(stats);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar estadísticas del dashboard: {ex.Message}", 
                              "Error", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Warning);
            }
        }

        private void ActualizarTop5(Marte.Application.Models.DashboardStatsDto stats)
        {
            if (stats.TopAsistentes.Count == 0)
            {
                // Ocultar todos los paneles del top 5 y mostrar mensaje
                pnlTop1.Visibility = Visibility.Collapsed;
                pnlTop2.Visibility = Visibility.Collapsed;
                pnlTop3.Visibility = Visibility.Collapsed;
                pnlTop4.Visibility = Visibility.Collapsed;
                pnlTop5.Visibility = Visibility.Collapsed;
                txtNoRegistros.Visibility = Visibility.Visible;
                return;
            }

            txtNoRegistros.Visibility = Visibility.Collapsed;

            // Top 1
            if (stats.TopAsistentes.Count >= 1)
            {
                var top1 = stats.TopAsistentes[0];
                pnlTop1.Visibility = Visibility.Visible;
                txtTop1Nombre.Text = top1.NombreCompleto;
                txtTop1Asistencias.Text = $"{top1.TotalAsistencias} / {top1.DiasDisponibles} días";
                txtTop1Porcentaje.Text = $"{top1.Porcentaje}%";
            }
            else
            {
                pnlTop1.Visibility = Visibility.Collapsed;
            }

            // Top 2
            if (stats.TopAsistentes.Count >= 2)
            {
                var top2 = stats.TopAsistentes[1];
                pnlTop2.Visibility = Visibility.Visible;
                txtTop2Nombre.Text = top2.NombreCompleto;
                txtTop2Asistencias.Text = $"{top2.TotalAsistencias} / {top2.DiasDisponibles} días";
                txtTop2Porcentaje.Text = $"{top2.Porcentaje}%";
            }
            else
            {
                pnlTop2.Visibility = Visibility.Collapsed;
            }

            // Top 3
            if (stats.TopAsistentes.Count >= 3)
            {
                var top3 = stats.TopAsistentes[2];
                pnlTop3.Visibility = Visibility.Visible;
                txtTop3Nombre.Text = top3.NombreCompleto;
                txtTop3Asistencias.Text = $"{top3.TotalAsistencias} / {top3.DiasDisponibles} días";
                txtTop3Porcentaje.Text = $"{top3.Porcentaje}%";
            }
            else
            {
                pnlTop3.Visibility = Visibility.Collapsed;
            }

            // Top 4
            if (stats.TopAsistentes.Count >= 4)
            {
                var top4 = stats.TopAsistentes[3];
                pnlTop4.Visibility = Visibility.Visible;
                txtTop4Nombre.Text = top4.NombreCompleto;
                txtTop4Asistencias.Text = $"{top4.TotalAsistencias} / {top4.DiasDisponibles} días";
                txtTop4Porcentaje.Text = $"{top4.Porcentaje}%";
            }
            else
            {
                pnlTop4.Visibility = Visibility.Collapsed;
            }

            // Top 5
            if (stats.TopAsistentes.Count >= 5)
            {
                var top5 = stats.TopAsistentes[4];
                pnlTop5.Visibility = Visibility.Visible;
                txtTop5Nombre.Text = top5.NombreCompleto;
                txtTop5Asistencias.Text = $"{top5.TotalAsistencias} / {top5.DiasDisponibles} días";
                txtTop5Porcentaje.Text = $"{top5.Porcentaje}%";
            }
            else
            {
                pnlTop5.Visibility = Visibility.Collapsed;
            }
        }

        private void InicializarReloj()
        {
            // Configurar timer para actualizar fecha y hora
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
            
            // Mostrar fecha y hora inicial
            ActualizarFechaHora();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            ActualizarFechaHora();
        }

        private void ActualizarFechaHora()
        {
            txtFechaHora.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }

        private async void OnLogoutClick(object sender, RoutedEventArgs e)
        {
            var resultado = MessageBox.Show("¿Está seguro que desea cerrar sesión?", 
                                          "Cerrar Sesión", 
                                          MessageBoxButton.YesNo, 
                                          MessageBoxImage.Question);
            
            if (resultado == MessageBoxResult.Yes)
            {
                timer?.Stop();

                var usuario = _sessionService.CurrentUser;
                if (usuario != null)
                {
                    // Registrar cierre de sesión en auditoría
                    await _authService.LogoutAsync(usuario.NombreUsuario);
                }

                await _sessionService.SignOutAsync();
                
                // Abrir ventana de login
                var loginWindow = ((App)System.Windows.Application.Current).ServiceProvider.GetRequiredService<LoginWindow>();
                loginWindow.Show();
                
                // Cerrar ventana principal
                this.Close();
            }
        }

        private void OnAsistentesClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var asistenteService = _serviceProvider.GetRequiredService<IAsistenteService>();
                var categoriaService = _serviceProvider.GetRequiredService<ICategoriaService>();
                var viewModel = new AsistenteManagementViewModel(asistenteService, categoriaService, _currentUser!);
                var window = new AsistenteManagementView(viewModel);
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir Gestión de Asistentes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnAsistenciaClick(object sender, RoutedEventArgs e)
        {
            try
            {
                // Verificar si la ventana ya está abierta
                if (_asistenciaWindow != null && _asistenciaWindow.IsLoaded)
                {
                    // Si ya está abierta, traerla al frente
                    _asistenciaWindow.Activate();
                    _asistenciaWindow.Focus();
                    return;
                }

                // Crear nueva ventana
                var asistenciaService = _serviceProvider.GetRequiredService<IAsistenciaService>();
                var viewModel = new AsistenciaControlViewModel(asistenciaService, _currentUser!);
                _asistenciaWindow = new AsistenciaControlView(viewModel);
                
                // Manejar el evento de cierre para limpiar la referencia
                _asistenciaWindow.Closed += (s, args) => _asistenciaWindow = null;
                
                // Abrir como ventana secundaria (no modal) con Show en lugar de ShowDialog
                _asistenciaWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir Control de Asistencia: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnReportesClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var reporteService = _serviceProvider.GetRequiredService<IReporteService>();
                var viewModel = new ReportesViewModel(reporteService, _currentUser!);
                var window = new ReportesView(viewModel);
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir Reportes y Estadísticas: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnConfiguracionClick(object sender, RoutedEventArgs e)
        {
            var usuario = _sessionService.CurrentUser;
            
            // Verificar si el usuario es administrador
            if (usuario != null && usuario.Rol?.Nombre == "Administrador")
            {
                var configurationView = new ConfigurationView();
                configurationView.ShowDialog();
            }
            else
            {
                MessageBox.Show("No tiene permisos para acceder a la configuración.\n\nSolo los administradores pueden acceder a esta sección.", 
                              "Acceso Denegado", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Warning);
            }
        }

        private void OnAuditoriaClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var auditLogService = _serviceProvider.GetRequiredService<IAuditLogService>();
                var viewModel = new AuditLogViewModel(auditLogService);
                var window = new AuditLogView(viewModel);
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir Bitácora de Auditoría: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            timer?.Stop();
            base.OnClosed(e);
        }
    }
}
