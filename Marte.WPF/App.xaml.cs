using System;
using System.IO;
using System.Windows;
using Marte.Application.Interfaces;
using Marte.Application.Services;
using Marte.Infrastructure.Data;
using Marte.Infrastructure.Interfaces;
using Marte.Infrastructure.Repositories;
using Marte.WPF.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Marte.WPF
{
    public partial class App : System.Windows.Application
    {
        private IServiceProvider _serviceProvider;

        public IServiceProvider ServiceProvider => _serviceProvider;

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                var services = new ServiceCollection();
                ConfigureServices(services);
                _serviceProvider = services.BuildServiceProvider();

                // ⭐ INICIALIZACIÓN AUTOMÁTICA DE BASE DE DATOS
                // Crear archivos .mdf y .ldf explícitamente si no existen
                var appDataPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "MARTE",
                    "Data"
                );
                var dbFilePath = Path.Combine(appDataPath, "MarteDb.mdf");
                var logFilePath = Path.Combine(appDataPath, "MarteDb_log.ldf");

                if (!File.Exists(dbFilePath))
                {
                    // Crear base de datos física con SQL si no existe
                    var createDbSql = $@"
                    IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'MarteDb')
                    BEGIN
                        CREATE DATABASE [MarteDb]
                        ON (NAME = N'MarteDb', FILENAME = '{dbFilePath}')
                        LOG ON (NAME = N'MarteDb_log', FILENAME = '{logFilePath}')
                    END
                    ";
                    try
                    {
                        using (var sqlConnection = new System.Data.SqlClient.SqlConnection("Server=(localdb)\\mssqllocaldb;Integrated Security=True;"))
                        {
                            sqlConnection.Open();
                            using (var cmd = sqlConnection.CreateCommand())
                            {
                                cmd.CommandText = createDbSql;
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error creando archivos físicos de la base de datos: {ex.Message}", "Error de BD", MessageBoxButton.OK, MessageBoxImage.Error);
                        Shutdown(1);
                        return;
                    }
                }

                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<MarteDbContext>();
                    // Verificar si la BD existe
                    var dbExists = await dbContext.Database.CanConnectAsync();
                    // Aplicar migraciones pendientes y seed siempre
                    await dbContext.Database.MigrateAsync();
                    await DatabaseSeeder.SeedAsync(_serviceProvider);
                    // Si la base de datos fue creada en este arranque, mostrar mensaje
                    if (!dbExists)
                    {
                        MessageBox.Show(
                            "✅ Base de datos inicializada correctamente.\n\n" +
                            "Usuario por defecto:\n" +
                            "Usuario: druagurto\n" +
                            "Contraseña: @Druagurto00\n\n" +
                            "Por favor cambie la contraseña después del primer inicio de sesión.",
                            "Inicialización Completada",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                }
                var loginWindow = _serviceProvider.GetRequiredService<LoginWindow>();
                loginWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"❌ Error al iniciar la aplicación:\n\n{ex.Message}\n\n" +
                    $"Detalles técnicos:\n{ex.InnerException?.Message}\n\n" +
                    "Si el problema persiste, contacte con soporte técnico.",
                    "Error de Inicio",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Shutdown(1);
            }
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // ⭐ CONFIGURACIÓN LOCALDB - Base de datos portable
            // La BD se crea automáticamente en AppData del usuario
            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "MARTE",
                "Data"
            );
            
            // Crear directorio si no existe
            Directory.CreateDirectory(appDataPath);
            
            var dbFilePath = Path.Combine(appDataPath, "MarteDb.mdf");

            #if DEBUG
            // En desarrollo, usar la instancia actual DRUAGURTO
            var connectionString = @"Server=.\DRUAGURTO;Database=MarteDb;Trusted_Connection=True;TrustServerCertificate=True;";
            #else
            // En producción, usar LocalDB con archivo MDF portable
            var connectionString = $@"Server=(localdb)\mssqllocaldb;AttachDbFilename={dbFilePath};Integrated Security=True;MultipleActiveResultSets=True;";
            #endif
            
            services.AddDbContext<MarteDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Repositories
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<IRolRepository, RolRepository>();
            services.AddScoped<IConfiguracionEscuelaRepository, ConfiguracionEscuelaRepository>();
            services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            services.AddScoped<IAsistenteRepository, AsistenteRepository>();
            services.AddScoped<IAsistenciaRepository, AsistenciaRepository>();

            // Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserManagementService, UserManagementService>();
            services.AddScoped<ISessionService, SessionService>();
            services.AddScoped<IConfiguracionService, ConfiguracionService>();
            services.AddScoped<ICategoriaService, CategoriaService>();
            services.AddScoped<IAsistenteService, AsistenteService>();
            services.AddScoped<IAsistenciaService, AsistenciaService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddSingleton<AuditService>();

            // Views
            services.AddTransient<LoginWindow>();
            services.AddTransient<MainWindow>();
            services.AddTransient<UserManagementView>();
            services.AddTransient<ConfigurationView>();
            services.AddTransient<CategoryManagementView>();
            services.AddTransient<AsistenteManagementView>();
            services.AddTransient<AsistenciaControlView>();
            services.AddTransient<ReportesView>();
            services.AddTransient<AuditLogView>();
            services.AddTransient<AboutWindow>();

            // Registrar ReporteRepository y ReporteService
            services.AddScoped<IReporteRepository, ReporteRepository>();
            services.AddScoped<IReporteService, ReporteService>();

            // Registrar AuditLogService
            services.AddScoped<IAuditLogService, AuditLogService>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
            base.OnExit(e);
        }
    }
}
