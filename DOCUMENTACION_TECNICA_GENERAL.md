# Documentación Técnica General - MARTE

Fecha: 18/10/2025  
Versión: 1.3.0  
Última actualización: 18/10/2025  
Ámbito: Sistema completo de gestión de asistencias  
Stack: .NET 9 (WPF), EF Core 9, SQL Server LocalDB (Producción) / SQL Server (Desarrollo).

---

## 1) Alcance actual (Versión 1.3.0)

### Modalidad de Despliegue

**Producción (Release)**: SQL Server LocalDB
- Base de datos embebida sin necesidad de configuración por el usuario
- Archivo `.mdf` ubicado en `%LocalAppData%\MARTE\Data\`
- Instalación completamente automatizada
- Sin dependencias de instancias de SQL Server en red

**Desarrollo (Debug)**: SQL Server Tradicional
- Instancia: `.\DRUAGURTO`
- Conexión directa para desarrollo y pruebas
- Migraciones aplicadas desde entorno de desarrollo

### Módulos Implementados

✅ **Login y Autenticación**
- Autenticación segura con BCrypt
- Gestión de sesiones
- Auditoría de accesos
- Validación de usuarios activos/inactivos

✅ **Gestión de Usuarios**
- CRUD completo con validaciones
- Roles (Administrador/Guardia)
- Cambio de contraseña
- Habilitar/Deshabilitar usuarios
- Auditoría completa de operaciones

✅ **Configuración del Sistema**
- Configuración de hora de cierre
- Nombre de filial
- Navegación a submódulos
- Auditoría de cambios

✅ **Gestión de Categorías**
- CRUD completo
- Validación de categorías en uso
- Categorías predefinidas + personalizadas
- Auditoría de operaciones

✅ **Gestión de Asistentes**
- CRUD completo con DNI único
- Categorización dinámica
- Número de grupo (solo para "Miembros")
- Estados (Activo/Inactivo)
- Validaciones robustas
- Auditoría detallada

✅ **Control de Asistencia**
- Registro de ingreso por DNI
- Registro de salida con observaciones
- Cierre automático al finalizar el día
- Historial por fecha
- Actualización en tiempo real (30 seg)
- Validaciones completas
- Auditoría de asistencias

✅ **Reportes y Estadísticas**
- 10+ tipos de reportes especializados
- Exportación a Excel y PDF
- Interfaz Master-Detail
- Filtros por fecha, categoría y grupo
- Gráficos y estadísticas
- Ventana maximizada

✅ **Bitácora de Auditoría**
- Consulta por fecha y usuario
- Exportación a TXT
- Trazabilidad completa de operaciones
- Filtros avanzados

✅ **Dashboard Principal** ⭐ ACTUALIZADO
- Indicadores en tiempo real
- Selector dinámico de categorías
- Carga dinámica de grupos de miembros
- Top 5 asistentes más constantes
- Actualización automática

### Características Técnicas

- **Database Seeder**: Datos iniciales (roles, categorías, usuario admin)
- **Arquitectura Limpia**: Separación por capas (Domain, Application, Infrastructure, Presentation)
- **Patrón MVVM**: ViewModels con comandos RelayCommand
- **Dependency Injection**: Microsoft.Extensions.DependencyInjection
- **ORM**: Entity Framework Core 9
- **Base de Datos**: SQL Server LocalDB (Producción) / SQL Server (Desarrollo)
- **Seguridad**: Hash BCrypt, RBAC por roles
- **Auditoría**: Registro completo en tabla AuditLogs
- **UI/UX**: WPF con tema institucional (#8B0000)
- **Distribución**: Instaladores PowerShell automatizados con SQL LocalDB embebido

---

## 2) Configuración de Base de Datos y Despliegue

### 2.1) Estrategia de Conexión (Compilación Condicional)

El sistema utiliza **compilación condicional** (#if DEBUG / #else) para diferenciar entre entornos de desarrollo y producción:

**Archivo**: `Marte.WPF/App.xaml.cs`

```csharp
protected override void ConfigureServices(IServiceCollection services)
{
#if DEBUG
    // Entorno de Desarrollo - SQL Server Tradicional
    var connectionString = @"Server=.\DRUAGURTO;Database=MarteDb;
                            Trusted_Connection=True;TrustServerCertificate=True;
                            MultipleActiveResultSets=True;";
#else
    // Entorno de Producción - SQL Server LocalDB
    var appDataPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "MARTE", "Data");
    
    if (!Directory.Exists(appDataPath))
        Directory.CreateDirectory(appDataPath);
    
    var dbFilePath = Path.Combine(appDataPath, "MarteDb.mdf");
    var connectionString = $@"Server=(localdb)\mssqllocaldb;
                             AttachDbFilename={dbFilePath};
                             Database=MarteDb;Trusted_Connection=True;
                             MultipleActiveResultSets=True;";
#endif

    services.AddDbContext<MarteDbContext>(options =>
        options.UseSqlServer(connectionString));
    
    // ... resto de configuración de servicios
}
```

**Características**:
- ✅ **Debug**: Conexión directa a instancia SQL Server local para desarrollo
- ✅ **Release**: Archivo `.mdf` embebido en carpeta de usuario
- ✅ **Portabilidad**: Base de datos viaja con los datos del usuario
- ✅ **Zero Configuration**: Usuario final no configura conexiones SQL

### 2.2) Factory para Migraciones EF Core

**Archivo**: `Marte.Infrastructure/Factories/MarteDesignTimeDbContextFactory.cs`

```csharp
public class MarteDesignTimeDbContextFactory : IDesignTimeDbContextFactory<MarteDbContext>
{
    public MarteDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MarteDbContext>();

#if DEBUG
        // Desarrollo: Usar instancia SQL Server para crear migraciones
        optionsBuilder.UseSqlServer(@"Server=.\DRUAGURTO;Database=MarteDb;
                                      Trusted_Connection=True;TrustServerCertificate=True;");
#else
        // Producción: Usar LocalDB (aunque migraciones se crean en Debug)
        optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;
                                      Database=MarteDb;Trusted_Connection=True;");
#endif

        return new MarteDbContext(optionsBuilder.Options);
    }
}
```

**Propósito**:
- Permite ejecutar comandos `dotnet ef migrations add` y `dotnet ef database update`
- Las migraciones se crean en modo Debug contra SQL Server
- Se aplican automáticamente en Release contra LocalDB

### 2.3) Inicialización y Primera Ejecución

**Archivo**: `Marte.WPF/App.xaml.cs` - Método `OnStartup`

```csharp
protected override async void OnStartup(StartupEventArgs e)
{
    base.OnStartup(e);

    using (var scope = _serviceProvider.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<MarteDbContext>();

        try
        {
            // Detectar si es primera ejecución (base de datos no existe)
            bool dbExists = await dbContext.Database.CanConnectAsync();

            if (!dbExists)
            {
                var result = MessageBox.Show(
                    "Es la primera vez que ejecutas MARTE.\n\n" +
                    "Se creará la base de datos con la configuración inicial.\n\n" +
                    "¿Deseas continuar?",
                    "Bienvenido a MARTE",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Aplicar migraciones y crear estructura
                    await dbContext.Database.MigrateAsync();

                    // Inicializar datos (roles, categorías, usuario admin)
                    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
                    await seeder.SeedAsync();

                    MessageBox.Show(
                        "Base de datos creada exitosamente.\n\n" +
                        "Usuario por defecto:\n" +
                        "- Usuario: druagurto\n" +
                        "- Contraseña: @Druagurto00\n\n" +
                        "IMPORTANTE: Cambia la contraseña después de iniciar sesión.",
                        "Configuración Completada",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else
                {
                    Shutdown();
                    return;
                }
            }
            else
            {
                // Base de datos existe, aplicar migraciones pendientes
                await dbContext.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Error al inicializar la base de datos:\n\n{ex.Message}\n\n" +
                "Verifica que SQL Server LocalDB esté instalado.",
                "Error de Conexión",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown();
            return;
        }
    }

    // Mostrar ventana de login
    var loginWindow = _serviceProvider.GetRequiredService<LoginWindow>();
    loginWindow.Show();
}
```

**Flujo de Primera Ejecución**:
1. Aplicación detecta que no existe archivo `.mdf` en `%LocalAppData%\MARTE\Data\`
2. Muestra mensaje de bienvenida solicitando confirmación
3. Aplica todas las migraciones de EF Core
4. Ejecuta `DatabaseSeeder` para crear:
   - Roles: Administrador, Guardia
   - Categorías: Miembros, Visitantes, Invitados, Pastores, Obreros, Ancianos, Evangelistas
   - Usuario admin: `druagurto` / `@Druagurto00`
5. Informa al usuario las credenciales por defecto
6. Usuario puede iniciar sesión inmediatamente

### 2.4) Ubicaciones de Archivos

**Producción (Release)**:
- **Aplicación**: `C:\Program Files\MARTE\`
- **Base de Datos**: `%LocalAppData%\MARTE\Data\MarteDb.mdf` y `.ldf`
- **Logs** (si se implementan): `%LocalAppData%\MARTE\Logs\`

**Desarrollo (Debug)**:
- **Aplicación**: Carpeta de compilación del proyecto
- **Base de Datos**: Instancia SQL Server `.\DRUAGURTO` → Base de datos `MarteDb`

**Exportaciones** (Reportes):
- Ubicación elegida por el usuario (SaveFileDialog)
- Formatos: `.xlsx` (Excel), `.pdf` (PDF), `.txt` (Auditoría)

---

## 3) Sistema de Distribución y Despliegue

### 3.1) Requisitos del Sistema

**Sistema Operativo**: Windows 10/11 (64-bit)

**Dependencias**:
- .NET 9.0 Runtime (Desktop)
- SQL Server LocalDB 2022

**Espacio en Disco**:
- Aplicación: ~50 MB
- .NET Runtime: ~200 MB
- SQL LocalDB: ~200 MB
- Base de datos: Variable (inicia vacía)

### 3.2) Scripts de Instalación Automatizada

#### 3.2.1) install-marte.ps1

**Ubicación**: Raíz del proyecto  
**Propósito**: Instalador completamente automatizado para usuarios finales  
**Características**:

1. **Verificación de Permisos Administrativos**
   ```powershell
   if (-NOT ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator"))
   {
       Write-Host "Este script requiere permisos de administrador." -ForegroundColor Red
       exit
   }
   ```

2. **Instalación de .NET 9 Runtime**
   - Detecta si está instalado: `dotnet --version`
   - Si no existe, descarga desde Microsoft CDN
   - Instala silenciosamente: `/install /quiet /norestart`

3. **Instalación de SQL Server LocalDB**
   - Detecta instalación: `sqllocaldb.exe`
   - Descarga desde Microsoft (si no está incluido en distribución)
   - Instala: `SqlLocalDB.msi /quiet IACCEPTSQLLOCALDBLICENSETERMS=YES`
   - Crea instancia: `sqllocaldb create mssqllocaldb`
   - Inicia instancia: `sqllocaldb start mssqllocaldb`

4. **Copia de Archivos de Aplicación**
   - Destino: `C:\Program Files\MARTE\`
   - Preserva estructura de carpetas
   - Sobrescribe si existe (para actualizaciones)

5. **Creación de Accesos Directos**
   - Desktop: `MARTE.lnk` con icono personalizado
   - Menú Inicio: `C:\ProgramData\Microsoft\Windows\Start Menu\Programs\MARTE.lnk`

6. **Confirmación Final**
   - Mensaje de éxito con ubicaciones
   - Instrucciones para primer inicio

**Ejecución**:
```powershell
# Click derecho en install-marte.ps1 → Ejecutar como Administrador
# O desde PowerShell elevado:
.\install-marte.ps1
```

#### 3.2.2) uninstall-marte.ps1

**Ubicación**: Raíz del proyecto  
**Propósito**: Desinstalador con opción de backup  
**Características**:

1. **Cierre de Procesos Activos**
   ```powershell
   Get-Process -Name "Marte.WPF" -ErrorAction SilentlyContinue | Stop-Process -Force
   ```

2. **Backup Opcional de Base de Datos**
   - Pregunta al usuario si desea crear backup
   - Copia `.mdf` y `.ldf` a carpeta con timestamp
   - Crea `README.txt` con instrucciones de restauración

3. **Eliminación Selectiva**
   - Archivos de aplicación: `C:\Program Files\MARTE\`
   - Accesos directos: Desktop y Menú Inicio
   - Opcional: Datos de usuario (`%LocalAppData%\MARTE\`)
   - Opcional: LocalDB (con advertencia si se usa en otros programas)

4. **Verificación Post-Desinstalación**
   - Confirma eliminación exitosa
   - Lista ubicaciones eliminadas
   - Informa sobre datos preservados (si aplica)

**Ejecución**:
```powershell
.\uninstall-marte.ps1
```

#### 3.2.3) crear-paquete-distribucion.ps1

**Ubicación**: Raíz del proyecto  
**Propósito**: Crear paquete de distribución para deployment  
**Características**:

1. **Compilación en Modo Release**
   ```powershell
   dotnet publish .\Marte.WPF\Marte.WPF.csproj `
       -c Release `
       -r win-x64 `
       --self-contained false `
       -o .\MARTE-Installer\Archivos
   ```

2. **Estructura del Paquete**
   ```
   MARTE-Installer/
   ├── Archivos/               # Binarios compilados
   │   ├── Marte.WPF.exe
   │   ├── Marte.Application.dll
   │   ├── Marte.Domain.dll
   │   ├── Marte.Infrastructure.dll
   │   └── [dependencias]
   ├── install-marte.ps1       # Instalador
   ├── uninstall-marte.ps1     # Desinstalador
   ├── README-INSTALACION.md   # Guía para el usuario
   └── VERSION.txt             # Información de versión
   ```

3. **Generación de VERSION.txt**
   ```
   Sistema MARTE
   Versión: 1.3.0
   Fecha de compilación: 18/10/2025 14:35:00
   Framework: .NET 9.0
   Base de Datos: SQL Server LocalDB

   Para instalar:
   1. Click derecho en install-marte.ps1
   2. Ejecutar como administrador
   3. Seguir las instrucciones

   Para desinstalar:
   1. Click derecho en uninstall-marte.ps1
   2. Ejecutar como administrador
   3. Seguir las instrucciones (incluye opción de backup)

   Para actualizar:
   1. Ejecutar install-marte.ps1 de la nueva versión
   2. Responder 'S' cuando pregunte si desea sobrescribir
   3. Los datos se conservan automáticamente

   Credenciales por defecto (solo primera instalación):
   Usuario: druagurto
   Contraseña: @Druagurto00
   ```

4. **Compresión ZIP** (Opcional)
   - Crea `MARTE-Installer-vX.X.X.zip`
   - Listo para distribuir por email, USB, descarga web

**Ejecución**:
```powershell
.\crear-paquete-distribucion.ps1
```

### 3.3) Documentación de Usuario

#### README-INSTALACION.md

**Contenido**:
- **Requisitos del Sistema**: Windows 10/11, espacio en disco
- **Instalación Automática**: Paso a paso con capturas conceptuales
- **Instalación Manual**: Para casos especiales
- **Primera Ejecución**: Qué esperar, credenciales por defecto
- **Seguridad**: Instrucciones para cambiar contraseña
- **Ubicaciones de Archivos**: Dónde encontrar ejecutable, datos, logs
- **Desinstalación Automática**: Con opción de backup
- **Desinstalación Manual**: 6 pasos detallados
- **Actualización/Upgrade**: Procedimiento con rollback
- **Solución de Problemas**: 7 errores comunes y soluciones
  1. "No se encuentra dotnet"
  2. "Error al conectar a LocalDB"
  3. "Aplicación no inicia"
  4. "Error de permisos"
  5. "Base de datos corrupta"
  6. "LocalDB ya existe"
  7. "Sin conexión a internet" (para instaladores offline)
- **Backup y Recuperación**: Procedimientos manuales
- **Contacto**: Soporte técnico

#### GUIA_INSTALACION_CLIENTE.md

**Contenido**:
- **Opción A: LocalDB** (implementada)
  - Ventajas: Simplicidad, portabilidad, cero configuración
  - Desventajas: No multiusuario simultáneo
  - Código de implementación
  - Procedimiento de instalación
- **Opción B: SQL Server en Red** (no implementada, documentada para referencia)
  - Ventajas: Multiusuario, centralización
  - Desventajas: Requiere servidor, configuración de red
  - Código de ejemplo
  - Arquitectura Cliente-Servidor
- **Tabla Comparativa**: Cuándo usar cada opción
- **Recomendaciones**: Basadas en caso de uso

#### CONEXIONES-LOCALDB.md

**Contenido Técnico**:
- **Connection Strings**: Debug vs Release
- **Ubicaciones de Archivos**: Paths exactos
- **Configuración en Código**: App.xaml.cs y Factory
- **Proceso de Instalación**: Diagrama de flujo
- **Comparativa Antes/Después**: SQL Server → LocalDB
- **Migración**: Cómo migrar de SQL Server a LocalDB
- **Comandos LocalDB**: Referencia de `sqllocaldb.exe`
  - `sqllocaldb info` - Listar instancias
  - `sqllocaldb start mssqllocaldb` - Iniciar instancia
  - `sqllocaldb stop mssqllocaldb` - Detener instancia
  - `sqllocaldb delete mssqllocaldb` - Eliminar instancia
- **Backup y Recuperación**: Procedimientos con archivos `.mdf`
- **Troubleshooting**: Problemas específicos de LocalDB
- **Notas de Rendimiento**: Limitaciones y recomendaciones

### 3.4) Proceso de Actualización

**Escenario**: Usuario tiene MARTE 1.2.0 instalado y desea actualizar a 1.3.0

**Pasos**:
1. Usuario descarga `MARTE-Installer-v1.3.0.zip`
2. Extrae contenido
3. Ejecuta `install-marte.ps1` como administrador
4. Script detecta instalación existente
5. Pregunta: "¿Desea sobrescribir archivos? (S/N)"
6. Usuario confirma con "S"
7. Script sobrescribe archivos en `C:\Program Files\MARTE\`
8. **Base de datos NO se toca** (permanece en `%LocalAppData%\MARTE\Data\`)
9. Al iniciar aplicación:
   - `App.xaml.cs` ejecuta `dbContext.Database.MigrateAsync()`
   - Se aplican migraciones pendientes automáticamente
   - Datos existentes se preservan
10. Usuario inicia sesión con credenciales existentes
11. Sistema operativo con nueva versión

**Rollback en Caso de Error**:
1. Usuario ejecuta `uninstall-marte.ps1` con opción de backup
2. Reinstala versión anterior desde `MARTE-Installer-v1.2.0.zip`
3. Base de datos queda intacta (compatible hacia atrás)

### 3.5) Distribución Offline vs Online

**Online** (Recomendado):
- `MARTE-Installer.zip` (~10 MB) solo con archivos de aplicación
- Instalador descarga .NET 9 y LocalDB desde Microsoft CDN
- Requiere conexión a internet durante instalación

**Offline** (Para redes sin internet):
- `MARTE-Installer-Full.zip` (~500 MB) incluye:
  - Aplicación
  - .NET 9 Runtime (`windowsdesktop-runtime-9.0.x-win-x64.exe`)
  - SQL LocalDB (`SqlLocalDB.msi`)
  - Scripts de instalación
- Instalador detecta archivos locales y no descarga
- Instalación completamente offline

**Creación de Paquete Offline**:
```powershell
# Modificar crear-paquete-distribucion.ps1 para incluir instaladores
# Descargar manualmente:
# - https://download.visualstudio.microsoft.com/download/pr/.../windowsdesktop-runtime-9.0.x-win-x64.exe
# - https://download.microsoft.com/download/.../SqlLocalDB.msi
# Copiar a carpeta MARTE-Installer/Instaladores/
```

---

## 4) Arquitectura y dependencias clave

- Capas
  - Domain: Entidades del negocio (`Usuario`, `Rol`, `Asistente`, `Categoria`, `Asistencia`, `AuditLog`, `ConfiguracionEscuela`).
  - Application: Servicios de caso de uso (`IUserManagementService`, `IAuthService`, `ISessionService`, `IAsistenteService`, `IAsistenciaService`, `ICategoriaService`, `IConfiguracionService`, `IAuditLogService`), ViewModels (MVVM).
  - Infrastructure: `MarteDbContext`, Repositorios (Usuario, Rol, Asistente, Asistencia, Categoria, AuditLog, ConfiguracionEscuela), Migraciones, Seeder.
  - WPF (Presentation): Views XAML (LoginWindow, MainWindow, UserManagementView, AsistenteManagementView, AsistenciaControlView, CategoryManagementView, ConfigurationView) y code-behind.
- ORM: Entity Framework Core 9 (SQL Server)
- DI: Microsoft.Extensions.DependencyInjection
- Seguridad: BCrypt.Net-Next (hash de contraseñas), RBAC por `Rol`.
- Auditoría: Tabla `AuditLogs` para trazabilidad de acciones.
- Comandos WPF: RelayCommand con integración a `CommandManager.RequerySuggested` para re-evaluación automática.

Ubicaciones clave:
- DbContext: `Marte.Infrastructure/Data/MarteDbContext.cs`
- Factory (EF Tools): `Marte.Infrastructure/Factories/MarteDesignTimeDbContextFactory.cs`
- Seeder: `Marte.Infrastructure/Data/DatabaseSeeder.cs`
- Servicios App: `Marte.Application/Services/*`
- Repositorios: `Marte.Infrastructure/Repositories/*`
- Vistas: `Marte.WPF/Views/*`
- ViewModels: `Marte.WPF/ViewModels/*`

---

## 5) Login (Autenticación + Sesión)

Puntos importantes:
- Flujo
  1. Usuario ingresa credenciales en LoginWindow.
  2. `IAuthService` verifica `password` con BCrypt contra `ContraseñaHash` en BD.
  3. En éxito: `ISessionService` establece `CurrentUser` y se registra auditoría (login).
  4. MainWindow se muestra; permisos (RBAC) habilitan/ocultan funciones.
- Clases/Servicios involucrados
  - `AuthService`: Hash/Verify, log de login/logout.
  - `SessionService`: Manejo de sesión en memoria (`CurrentUser`).
  - `LoginViewModel`: Binding UI y validaciones.
  - `AuditService`/`AuditLogRepository`: persistencia de auditoría.
- Seguridad
  - BCrypt con salt automático; verificación constante en tiempo.
  - No se almacenan contraseñas en texto plano.
  - Acceso a User Management limitado a rol `Administrador` (ver `MainWindow.xaml.cs` -> `OnConfiguracionClick`).
- Errores típicos
  - Credenciales inválidas: mensaje en UI, no revela cuál campo falló.
  - Usuario inactivo (`Estado=false`): acceso denegado.

---

## 6) Dashboard Principal (MainWindow)

⭐ **Estado**: ACTUALIZADO - Versión 1.3.0

### Descripción General

El Dashboard es la ventana principal del sistema que muestra información en tiempo real sobre asistencias, estadísticas y permite acceso a todos los módulos.

### Componentes Principales

#### 4.1) Indicadores en Tiempo Real

**Archivo**: `Marte.WPF/Views/MainWindow.xaml`

**Métricas mostradas**:
- **Asistentes Hoy**: Total de ingresos del día actual
- **Hasta Cierre Ayer**: Asistentes que permanecieron hasta la hora de cierre el día anterior
- **Puntualidad**: Porcentaje de asistentes puntuales
- **Total Registrados**: Cantidad total de asistentes activos en el sistema

**Implementación**:
```csharp
// MainWindow.xaml.cs
private async Task CargarDashboardAsync()
{
    using var scope = _serviceProvider.CreateScope();
    var dashboardService = scope.ServiceProvider.GetRequiredService<IDashboardService>();
    var stats = await dashboardService.GetDashboardStatsAsync();
    
    txtAsistentesHoy.Text = stats.AsistentesHoy.ToString();
    txtHastaCierre.Text = stats.HastaCierreAyer.ToString();
    txtPuntualidad.Text = $"{stats.PorcentajePuntualidad}%";
    txtTotalRegistrados.Text = stats.TotalRegistrados.ToString();
}
```

#### 4.2) Selector Dinámico de Categorías ⭐ NUEVO

**Características**:
- ComboBox de categorías con carga dinámica desde la base de datos
- Opción "Todas las categorías" para ver total general
- ComboBox de grupos visible **solo para categoría "Miembros"**
- Carga dinámica de grupos desde asistentes existentes
- Contador actualizado en tiempo real

**Implementación Técnica**:

1. **Carga de Categorías**:
```csharp
private async Task CargarCategoriasAsync()
{
    using var scope = _serviceProvider.CreateScope();
    var categoriaService = scope.ServiceProvider.GetRequiredService<ICategoriaService>();
    var categorias = await categoriaService.GetAllCategoriasAsync();
    
    await Dispatcher.InvokeAsync(() =>
    {
        cmbCategoria.Items.Clear();
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
        
        cmbCategoria.SelectedIndex = 0;
    });
}
```

2. **Carga Dinámica de Grupos** (solo para Miembros):
```csharp
private async Task CargarGruposAsync()
{
    using var scope = _serviceProvider.CreateScope();
    var asistenteService = scope.ServiceProvider.GetRequiredService<IAsistenteService>();
    var asistentes = await asistenteService.GetAllAsistentesAsync();
    
    // Obtener grupos únicos de miembros activos
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
        
        if (cmbGrupo.Items.Count > 0)
            cmbGrupo.SelectedIndex = 0;
    });
}
```

3. **Actualización del Contador**:
```csharp
private async Task ActualizarContadorCategoriaAsync()
{
    using var scope = _serviceProvider.CreateScope();
    var asistenteService = scope.ServiceProvider.GetRequiredService<IAsistenteService>();
    var asistentes = await asistenteService.GetAllAsistentesAsync();
    
    if (categoriaTag == "TODAS")
    {
        total = asistentes.Count(a => a.Estado);
        label = "Total de asistentes";
    }
    else if (categoriaNombre.Contains("Miembro") && pnlGrupo.Visibility == Visibility.Visible)
    {
        var numeroGrupo = ((ComboBoxItem)cmbGrupo.SelectedItem)?.Tag.ToString();
        total = asistentes.Count(a => a.Estado && 
                                      a.CategoriaId == Guid.Parse(categoriaTag) && 
                                      a.NumeroGrupo == numeroGrupo);
        label = $"Miembros Grupo #{numeroGrupo}";
    }
    else
    {
        total = asistentes.Count(a => a.Estado && a.CategoriaId == Guid.Parse(categoriaTag));
        label = categoriaNombre;
    }
    
    txtCategoriaLabel.Text = label;
    txtCategoriaTotal.Text = total.ToString();
}
```

**Estilo Visual**:
- Color de fondo: `#1A1A1A` (negro oscuro)
- Borde: `#8B0000` (rojo institucional)
- Highlight al seleccionar: `#8B0000`
- Fuente: Cinzel (Times New Roman fallback)

#### 4.3) Top 5 Más Constantes

**Características**:
- Muestra los 5 asistentes con mayor porcentaje de asistencia
- Carga automática desde la base de datos
- Indicadores visuales con colores diferenciados (oro, plata, bronce)
- Mensaje "No existen registros actualmente" si no hay datos

**Servicio DashboardService**:
```csharp
public async Task<DashboardStatsDto> GetDashboardStatsAsync()
{
    var stats = new DashboardStatsDto();
    var hoy = DateTime.Today;
    
    // ... cálculos de indicadores ...
    
    // Top 5 asistentes más constantes
    stats.Top5Asistentes = await GetTop5AsistentesAsync();
    
    return stats;
}
```

### Patrón de Acceso a DbContext

**Problema Resuelto**: Concurrencia de DbContext

**Solución Implementada**: Patrón "Scope por Operación"

Cada operación async crea su propio scope de DI para obtener una nueva instancia del DbContext:

```csharp
// ❌ ANTES (causaba errores de concurrencia)
private readonly IDashboardService _dashboardService;

public MainWindow(IDashboardService dashboardService)
{
    _dashboardService = dashboardService;
}

// ✅ AHORA (cada operación tiene su propio scope)
private readonly IServiceProvider _serviceProvider;

public MainWindow(IServiceProvider serviceProvider)
{
    _serviceProvider = serviceProvider;
}

private async Task CargarDashboardAsync()
{
    using var scope = _serviceProvider.CreateScope();
    var dashboardService = scope.ServiceProvider.GetRequiredService<IDashboardService>();
    var stats = await dashboardService.GetDashboardStatsAsync();
    // ... usar stats ...
}
```

**Ventajas**:
- ✅ Elimina errores de concurrencia del DbContext
- ✅ Cada operación es independiente
- ✅ Mejor manejo de recursos (dispose automático)
- ✅ Compatible con operaciones paralelas si es necesario

### Inicialización Secuencial

Para evitar conflictos, la carga del dashboard se realiza secuencialmente:

```csharp
private async Task InicializarDashboardAsync()
{
    await CargarCategoriasAsync();  // Primero carga categorías
    await CargarDashboardAsync();    // Luego carga estadísticas
}
```

### Thread Safety en UI

Todas las actualizaciones de controles WPF se ejecutan en el Dispatcher:

```csharp
await Dispatcher.InvokeAsync(() =>
{
    cmbCategoria.Items.Clear();
    // ... actualizar UI ...
});
```

### Eliminación de Datos Ficticios

**Importante**: Todas las vistas han sido limpiadas de datos ficticios:
- ✅ Usuario: `""` (vacío, se llena al iniciar sesión)
- ✅ Indicadores: `"0"` (se actualizan desde BD)
- ✅ Top 5: Campos vacíos `""` (se llenan desde BD)
- ✅ Listas: Sin items predefinidos (carga dinámica)

---

## 7) Gestión de Usuarios (CRUD)

Puntos importantes:
- Alcance
  - Crear, listar, actualizar datos, cambiar contraseña, deshabilitar/habilitar (soft delete por `Estado`).
  - Validaciones UI (coincidencia de contraseñas, campos requeridos) y servidor (unicidad `NombreUsuario`).
  - Auditoría de operaciones: crear/actualizar/cambiar contraseña/habilitar/deshabilitar.
  - **Visualización automática**: Al seleccionar un usuario de la lista, sus datos se cargan automáticamente en el formulario (modo solo lectura).
  - **Modo Edición**: Hacer clic en "Editar" habilita los campos para modificación.
- Componentes
  - View: `Marte.WPF/Views/UserManagementView.xaml` (manejo especial de `PasswordBox`).
  - ViewModel: `UserManagementViewModel` (Bindings, `ICommand`, validaciones, propiedad `IsFormReadOnly`).
  - Servicio: `IUserManagementService` / `UserManagementService` (lógica de negocio CRUD).
  - Repositorios: `IUsuarioRepository`, `IRolRepository`, `IAuditLogRepository`.
- Reglas de negocio
  - `NombreUsuario` único (índice único en BD).
  - Cambio de contraseña sólo vía acción dedicada (nunca expone la original).
  - Soft delete via `Estado=false`; habilitación reversa.
  - Campos bloqueados en modo visualización; desbloqueados en modo edición/nuevo.
- Flujo de trabajo
  1. Seleccionar usuario → datos se cargan en formulario (solo lectura)
  2. Clic en "Editar" → campos se habilitan para modificación
  3. Modificar datos → clic en "Guardar"
  4. Sistema valida y persiste cambios

---

## 8) Configuración del Sistema

Puntos importantes:
- Alcance
  - Hub central para acceder a configuraciones del sistema.
  - Configurar hora de cierre de la escuela/filial.
  - Navegación a Gestión de Usuarios.
  - Navegación a Gestión de Categorías.
- Componentes
  - View: `Marte.WPF/Views/ConfigurationView.xaml`
  - ViewModel: `ConfigurationViewModel`
  - Servicio: `IConfiguracionService` / `ConfiguracionService`
  - Repositorio: `IConfiguracionEscuelaRepository` / `ConfiguracionEscuelaRepository`
  - Entidad: `ConfiguracionEscuela` (NombreFilial, HoraCierre)
- Módulos integrados
  1. **Configuración de Hora de Cierre**
     - Campos: Nombre de Filial, Hora (0-23), Minuto (0-59)
     - Botón: "Guardar Configuración"
     - Auditoría: Registra cambios en `AuditLog`
  2. **Gestión de Usuarios**
     - Botón: "Abrir Gestión de Usuarios"
     - Abre `UserManagementView` en modo diálogo
  3. **Gestión de Categorías**
     - Botón: "Abrir Gestión de Categorías"
     - Abre `CategoryManagementView` en modo diálogo
- Acceso
  - Solo usuarios con rol `Administrador`.
  - MainWindow → Botón "Configuración" → ConfigurationView.
  - RBAC validado en `OnConfiguracionClick`.

---

## 9) Gestión de Categorías (CRUD)

Puntos importantes:
- Alcance
  - Crear, listar, actualizar y eliminar categorías para clasificar asistentes.
  - Campos: Nombre (único), Número de Grupo (ej: #01, #02, No Aplica).
  - Validaciones: No permite nombres duplicados, campos obligatorios.
  - Auditoría completa de operaciones CUD.
- Componentes
  - View: `Marte.WPF/Views/CategoryManagementView.xaml`
  - ViewModel: `CategoryManagementViewModel` (ObservableCollection, comandos CRUD)
  - Servicio: `ICategoriaService` / `CategoriaService`
  - Repositorio: `ICategoriaRepository` / `CategoriaRepository`
  - Entidad: `Categoria` (Id, Nombre, NumeroGrupo)
- Operaciones
  - **Nueva Categoría**: Crea registro con validación de unicidad.
  - **Editar**: Modifica nombre y número de grupo.
  - **Guardar**: Persiste CREATE o UPDATE según modo.
  - **Eliminar**: Borra permanentemente con confirmación.
  - **Cancelar**: Limpia formulario y cancela operación.
- Interfaz
  - Panel izquierdo: DataGrid con lista de categorías (Nombre, Número de Grupo).
  - Panel derecho: Formulario con campos editables (habilitados solo en modo edición/nuevo).
  - Botones: Nueva, Guardar, Editar, Cancelar, Eliminar.
- Validaciones
  - Nombre de categoría único (checked en BD antes de guardar).
  - Campos requeridos: Nombre y Número de Grupo.
  - Confirmación antes de eliminar: "¿Está seguro que desea eliminar...?"
- Auditoría
  - Todas las operaciones (CREATE, UPDATE, DELETE) se registran en `AuditLog`.
  - Detalles incluyen: nombre de categoría, número de grupo, acción realizada.

---

## 10) Database Seeder (datos rígidos)

Puntos importantes:
- Ubicación: `Marte.Infrastructure/Data/DatabaseSeeder.cs`.
- Idempotente: usa GUIDs fijos para roles y admin; no duplica si ya existen.
- Datos insertados
  - Roles: `Administrador`, `Guardia` (GUIDs fijos).
  - Categorías (9): Estructural Jefe de Filial, Secretarios, GG.FF, GG.MM, Guardia de Seguridad, Miembros, Filosofía, Visitas, Otros.
  - Usuario Admin: `druagurto` con contraseña `@Druagurto00` (hash BCrypt) y rol Administrador. Solo para entorno de desarrollo.
- Ejecución
  - Automática en `App.xaml.cs` (`OnStartup`): `await DatabaseSeeder.SeedAsync(serviceProvider)`.
  - Runner dedicado (opcional): proyecto `SeedRunner` para ejecutar seed fuera de la UI.
  - Verificación rápida (SQLCMD)
  - Roles: 2, Usuarios: 1, Categorías: 9.

---

## 11) Configuración y conexiones

**Obsoleto**: Esta sección ha sido reemplazada por la Sección 2 "Configuración de Base de Datos y Despliegue" que incluye detalles sobre LocalDB y compilación condicional.

Consultar:
- Sección 2.1: Estrategia de Conexión (Compilación Condicional)
- Sección 2.2: Factory para Migraciones EF Core
- Sección 3: Sistema de Distribución y Despliegue
- `CONEXIONES-LOCALDB.md`: Documentación técnica detallada

---

## 12) Migraciones EF Core

- Connection string actual (desarrollo):
  - `Server=.\\DRUAGURTO;Database=MarteDb;Trusted_Connection=True;TrustServerCertificate=True;`
- Dónde está configurada
  - Runtime: `Marte.WPF/App.xaml.cs` (AddDbContext)
  - Herramientas EF (migraciones): `Marte.Infrastructure/Factories/MarteDesignTimeDbContextFactory.cs`
  - Referencia: `Marte.WPF/appsettings.json`
- Notas
  - Asegurar SQL Server Browser/instancia `DRUAGURTO` activa.
  - Cambiar a variables de entorno/Secret Manager para producción.

---

## 12) Migraciones EF Core

**Migraciones Aplicadas**:
- `20251016233131_InitialCreateSqlServer`: Estructura inicial
- `20251018072026_AddNumeroGrupoAndEstadoToAsistente`: Campos para asistentes
- `20251018072747_RemoveNumeroGrupoFromCategoria`: Simplificación de categorías

**Comandos Útiles** (Desarrollo):
```bash
# Listar migraciones
dotnet ef migrations list --project Marte.Infrastructure --startup-project Marte.WPF

# Crear nueva migración
dotnet ef migrations add NombreMigracion --project Marte.Infrastructure --startup-project Marte.WPF

# Aplicar migraciones manualmente (solo desarrollo)
dotnet ef database update --project Marte.Infrastructure --startup-project Marte.WPF

# Generar script SQL
dotnet ef migrations script --project Marte.Infrastructure --startup-project Marte.WPF -o migration.sql
```

**Importante**: 
- En **producción (Release)**, las migraciones se aplican **automáticamente** al iniciar la aplicación mediante `dbContext.Database.MigrateAsync()` en `App.xaml.cs`
- Los comandos anteriores son solo para desarrollo/debug

---

## 13) Cómo ejecutar localmente (dev)

**Requisitos**: 
- .NET 9 SDK
- SQL Server (instancia DRUAGURTO)
- Windows 10/11

**Pasos**:
1. **Compilar solución** (modo Debug):
   ```bash
   dotnet build Marte.sln -c Debug
   ```

2. **Migrar base de datos** (solo primera vez):
   ```bash
   dotnet ef database update --project Marte.Infrastructure --startup-project Marte.WPF
   ```

3. **Iniciar aplicación**:
   ```bash
   dotnet run --project Marte.WPF
   ```
   - Al primer inicio, se ejecuta `DatabaseSeeder` automáticamente
   - Crea roles, categorías y usuario admin

4. **Login inicial**:
   - Usuario: `druagurto`
   - Contraseña: `@Druagurto00`

**Compilación para Producción (Release)**:
```bash
# Compilar en modo Release (usa LocalDB)
dotnet build Marte.sln -c Release

# O crear paquete de distribución completo
.\crear-paquete-distribucion.ps1
```

---

## 14) Troubleshooting rápido

**Desarrollo**:

**Desarrollo**:
- **Error 26/instancia no encontrada**: Validar servicio `MSSQL$DRUAGURTO` y SQL Browser en ejecución
- **Login falla**: Usuario deshabilitado (`Estado=false`) o contraseña incorrecta
- **Migración falla**: Verificar connection string en `App.xaml.cs` apunte a instancia correcta
- **DbContext disposed**: Usar patrón "Scope por Operación" (`using var scope = _serviceProvider.CreateScope()`)

**Producción**:
- **"No se encuentra dotnet"**: Instalar .NET 9 Runtime desde Microsoft
- **"Error al conectar a LocalDB"**: 
  - Verificar SQL LocalDB instalado: `sqllocaldb info`
  - Crear instancia: `sqllocaldb create mssqllocaldb`
  - Iniciar instancia: `sqllocaldb start mssqllocaldb`
- **"Aplicación no inicia"**: 
  - Verificar permisos de carpeta `%LocalAppData%\MARTE\`
  - Revisar logs de Windows Event Viewer
- **"Base de datos corrupta"**: 
  - Eliminar archivos `.mdf` y `.ldf` de `%LocalAppData%\MARTE\Data\`
  - Reiniciar aplicación (recrea desde cero)

**Consultar**: `README-INSTALACION.md` para troubleshooting detallado de instalación

---

## 15) Gestión de Asistentes
- Migración falla: actualizar connection string en los 3 puntos y reintentar.
- EF Tools no encuentran DbContext: confirmar Factory en `Infrastructure/Factories` y namespace `Marte.Infrastructure.Factories`.
- Botones no se habilitan: verificar que `RelayCommand.CanExecuteChanged` esté suscrito a `CommandManager.RequerySuggested` y que se llame `CommandManager.InvalidateRequerySuggested()` en setters de propiedades relevantes.

---

## 13) Gestión de Asistentes

Puntos importantes:
- Entidad
  - `Asistente`: `Id`, `Nombres`, `Apellidos`, `DNI` (único), `CategoriaId`, `NumeroGrupo` (nullable), `Estado`, `FechaCreacion`, `FechaModificacion`.
  - Relación: `Asistente` → `Categoria` (muchos a uno).
  - `NumeroGrupo`: Solo aplicable para categoría "Miembros", formato: 01, 02, 03... (validación regex `^\d{2}$`).
  
- Servicio (`IAsistenteService`)
  - `GetAllAsistentesAsync()`: Retorna todos los asistentes (incluye inactivos).
  - `GetAsistentesActivosAsync()`: Retorna solo asistentes con `Estado = true`.
  - `CreateAsistenteAsync(nombres, apellidos, dni, categoriaId, numeroGrupo, usuario)`: Validaciones de DNI único, campos obligatorios, formato de grupo. Audita creación.
  - `UpdateAsistenteAsync(id, nombres, apellidos, dni, categoriaId, numeroGrupo, usuario)`: Valida DNI único (excluyendo el mismo), audita con detalle de cambios.
  - `DeleteAsistenteAsync(id, usuario)`: Eliminación física (PERMANENTE). Audita.
  - `InactivarAsistenteAsync(id, usuario)`: Cambia `Estado` a `false`. Audita.
  - `ActivarAsistenteAsync(id, usuario)`: Cambia `Estado` a `true`. Audita.
  - `ValidarDNIUnicoAsync(dni, excludeId)`: Helper de validación.

- Repositorio (`IAsistenteRepository`)
  - `GetAllAsync()`: Incluye `Categoria`, ordena por `Apellidos`, `Nombres`.
  - `GetActivosAsync()`: Filtra por `Estado = true`.
  - `GetByIdAsync(id)`: Incluye `Categoria`.
  - `GetByDNIAsync(dni)`: Busca por DNI.
  - `ExisteDNIAsync(dni, excludeId)`: Valida unicidad.
  - `CreateAsync(asistente)`: Agrega y recarga relaciones.
  - `UpdateAsync(asistente)`: Actualiza `FechaModificacion`.
  - `DeleteAsync(id)`: Eliminación física.
  - `InactivarAsync(id)`: Cambia estado a inactivo.
  - `ActivarAsync(id)`: Cambia estado a activo.

- ViewModel (`AsistenteManagementViewModel`)
  - Propiedades: `Asistentes`, `Categorias`, `SelectedAsistente`, `Nombres`, `Apellidos`, `DNI`, `SelectedCategoria`, `NumeroGrupo`, `MostrarSoloActivos`.
  - Propiedades computadas: `IsNumeroGrupoVisible` (true si categoría = "Miembros"), `IsNumeroGrupoEnabled` (editable solo en modos edit/new), `CanEdit`, `CanDelete`, `CanInactivar`, `CanActivar`.
  - Comandos: `NewAsistenteCommand`, `SaveAsistenteCommand`, `EditAsistenteCommand`, `DeleteAsistenteCommand`, `InactivarAsistenteCommand`, `ActivarAsistenteCommand`, `CancelCommand`.
  - Validaciones UI: Campo DNI solo editable en modo nuevo, `NumeroGrupo` visible/requerido solo para "Miembros".

- Vista (`AsistenteManagementView.xaml`)
  - DataGrid: Columnas (DNI, Apellidos, Nombres, Categoría, Grupo, Estado).
  - Formulario: DNI, Nombres, Apellidos, Categoría (ComboBox), Número de Grupo (condicional).
  - CheckBox: "Mostrar solo activos" (filtra lista).
  - Botones: NUEVO, GUARDAR, EDITAR, INACTIVAR, ACTIVAR, ELIMINAR, CANCELAR.
  - Estilo: Consistente con MARTE (rojo #8B0000, Times New Roman, fondo oscuro).

- Validaciones
  - DNI: Obligatorio, único (case-insensitive en SQL Server), no editable después de creación.
  - Nombres/Apellidos: Obligatorios.
  - Categoría: Obligatoria.
  - Número de Grupo: Obligatorio si categoría = "Miembros", formato `^\d{2}$`, automáticamente `null` para otras categorías.
  - Estado: Inactivar solo si activo, Activar solo si inactivo.

- Auditoría
  - Crear: Registra nombre completo y DNI.
  - Actualizar: Detalla cambios (DNI, Categoría, Grupo).
  - Eliminar: Registra asistente eliminado (PERMANENTE).
  - Inactivar/Activar: Registra cambio de estado.

- Migraciones
  - `20251018072026_AddNumeroGrupoAndEstadoToAsistente`: Agregó `NumeroGrupo` (nullable) y `Estado` (bool, default true) a tabla `Asistentes`.

- Flujos típicos
  1. **Crear Miembro**: NUEVO → DNI, Nombres, Apellidos, Categoría="Miembros", Número="01" → GUARDAR.
  2. **Crear Visita**: NUEVO → DNI, Nombres, Apellidos, Categoría="Visitas" → GUARDAR (número no visible).
  3. **Editar categoría a Miembros**: Seleccionar → EDITAR → Cambiar categoría → Campo "Número de Grupo" aparece → Ingresar → GUARDAR.
  4. **Inactivar**: Seleccionar activo → INACTIVAR → Confirmar.
  5. **Activar**: Desmarcar "Mostrar solo activos" → Seleccionar inactivo → ACTIVAR → Confirmar.
  6. **Eliminar**: Seleccionar → ELIMINAR → Confirmar (ACCIÓN IRREVERSIBLE).

---

## 16) Gestión de Categorías

Puntos importantes:
- Entidad
  - `Categoria`: `Id`, `Nombre`.
  - **NOTA**: Campo `NumeroGrupo` fue **eliminado** de `Categoria` (ahora solo existe en `Asistente`).
  - Relación: `Categoria` → `Asistentes` (uno a muchos).

- Servicio (`ICategoriaService`)
  - `GetAllCategoriasAsync()`: Retorna todas las categorías.
  - `GetCategoriaByIdAsync(id)`: Retorna categoría por ID.
  - `CreateCategoriaAsync(nombre)`: Valida nombre único. Audita creación.
  - `UpdateCategoriaAsync(id, nombre)`: Valida nombre único (excluyendo misma). Audita actualización.
  - `DeleteCategoriaAsync(id)`: Eliminación física. Audita.
  - `ExisteNombreCategoriaAsync(nombre, excludeId)`: Helper de validación.

- Vista (`CategoryManagementView.xaml`)
  - DataGrid: Columna única (Nombre).
  - Formulario: Campo único (Nombre de Categoría).
  - Botones: Nueva Categoría, Guardar, Editar, Cancelar, Eliminar Categoría.

- Migraciones
  - `20251018072747_RemoveNumeroGrupoFromCategoria`: Eliminó columna `NumeroGrupo` de tabla `Categorias`.

---

## 17) Control de Asistencia

### Objetivo
Registrar las entradas y salidas de asistentes por día y hora, con validaciones y actualización en tiempo real.

### Entidad (`Marte.Domain.Entities.Asistencia`)

```csharp
public class Asistencia
{
    public Guid Id { get; set; }
    public Guid AsistenteId { get; set; }
    public Asistente Asistente { get; set; }
    public DateTime Fecha { get; set; } // Solo fecha (Today)
    public DateTime HoraIngreso { get; set; } // Fecha + hora completa
    public DateTime? HoraSalida { get; set; } // Nullable
    public bool HastaCierre { get; set; } = false; // True si se aplicó cierre automático
    public string? Observacion { get; set; }
}
```

### Servicio (`IAsistenciaService`)

**Métodos principales**:

1. **`RegistrarIngresoAsync(string dni, string registradoPor)`**
   - Busca al asistente por DNI (único).
   - Validaciones:
     * DNI no puede estar vacío.
     * Asistente debe existir (si no existe, mensaje: "Debe registrarse primero en Gestión de Asistentes").
     * Asistente debe estar activo (`Estado = true`).
     * No debe tener asistencia abierta hoy (sin salida registrada).
   - Crea registro con `HoraIngreso = DateTime.Now` y `HoraSalida = null`.
   - Auditoría: "Registrar Ingreso" con detalles (nombre, DNI, hora).
   - Retorna: `(bool Success, string Message)`.

2. **`RegistrarSalidaAsync(Guid asistenciaId, string registradoPor, string? observacion)`**
   - Busca la asistencia por ID.
   - Validaciones:
     * No debe tener ya salida registrada.
     * Hora de salida no puede ser menor que hora de ingreso.
   - Actualiza `HoraSalida = DateTime.Now` y `Observacion`.
   - Auditoría: "Registrar Salida" con detalles.
   - Retorna: `(bool Success, string Message)`.

3. **`AplicarCierreAutomaticoAsync(string aplicadoPor)`**
   - Obtiene la hora de cierre configurada (`ConfiguracionEscuela.HoraCierre`).
   - Busca todas las asistencias abiertas de hoy.
   - Para cada una: establece `HoraSalida = Today + HoraCierre`, `HastaCierre = true`, `Observacion = "Cierre automático"`.
   - Auditoría: "Cierre Automático" con cantidad de registros cerrados.
   - Retorna: `(bool Success, string Message, int Actualizados)`.

4. **`GetAsistenciasPresentesAsync()`**
   - Retorna asistencias de hoy sin salida (`HoraSalida == null`).
   - Ordenadas por apellido, nombre.
   - Incluye relación: `Asistente.Categoria`.

5. **`GetAsistenciasByFechaAsync(DateTime fecha)`**
   - Retorna todas las asistencias de una fecha específica.
   - Incluye relación: `Asistente.Categoria`.

6. **`GetAllAsistenciasAsync()`**
   - Retorna todas las asistencias históricas.

### Repositorio (`IAsistenciaRepository`)

**Métodos**:
- `GetAllAsync()`: Todas las asistencias con `Include(Asistente.Categoria)`.
- `GetByIdAsync(id)`: Busca por ID con relación.
- `GetByFechaAsync(fecha)`: Filtra por `Fecha.Date == fecha.Date`.
- `GetAsistenciasPresentesAsync()`: Filtra `Fecha.Date == Today && HoraSalida == null`.
- `GetAsistenciaAbiertaByAsistenteIdAsync(asistenteId)`: Busca asistencia abierta hoy para un asistente.
- `ExisteAsistenciaAbiertaAsync(asistenteId)`: Valida si existe asistencia abierta.
- `AddAsync(asistencia)`, `UpdateAsync(asistencia)`, `SaveChangesAsync()`.

### ViewModel (`AsistenciaControlViewModel`)

**Propiedades**:
- `AsistenciasPresentes`: ObservableCollection de asistencias sin salida hoy.
- `HistorialDia`: ObservableCollection de asistencias del día seleccionado.
- `SelectedAsistenciaPresente`: Asistencia seleccionada para registrar salida.
- `DNIBusqueda`: Campo para buscar asistente por DNI.
- `ObservacionSalida`: Observación opcional al registrar salida.
- `FechaSeleccionada`: Fecha para filtrar historial (default: hoy).
- `TotalPresentes`: Contador de asistentes sin salida.
- `CanSalir`: Habilita botón de salida si hay asistencia seleccionada.

**Comandos**:
- `RegistrarIngresoCommand`: Valida DNI no vacío, llama a `RegistrarIngresoAsync`.
- `RegistrarSalidaCommand`: Valida `CanSalir`, muestra confirmación, llama a `RegistrarSalidaAsync`.
- `AplicarCierreCommand`: Muestra confirmación con total de presentes, llama a `AplicarCierreAutomaticoAsync`.
- `ActualizarCommand`: Recarga datos de presentes e historial.
- `BuscarHistorialCommand`: Recarga historial por fecha seleccionada.

**Timer de actualización automática**:
- `DispatcherTimer` cada 30 segundos.
- Actualiza `AsistenciasPresentes` automáticamente.

### Vista (`AsistenciaControlView.xaml`)

**Distribución**:

```
┌──────────────────────────────────────────────────────────┐
│           CONTROL DE ASISTENCIA (Header)                 │
├──────────────────────────────────────────────────────────┤
│ REGISTRAR INGRESO                                        │
│ DNI/Código: [________] [Registrar Ingreso]              │
├─────────────────────────┬────────────────────────────────┤
│ ASISTENTES PRESENTES    │  HISTORIAL DE ASISTENCIAS     │
│ Total presentes: 5      │  Fecha: [__/__/____] [Buscar] │
│                         │                                │
│ [DataGrid: Sin Salida]  │  [DataGrid: Todas del día]    │
│ - DNI                   │  - DNI                         │
│ - Apellidos             │  - Apellidos                   │
│ - Nombres               │  - Nombres                     │
│ - Categoría             │  - Ingreso                     │
│ - Ingreso               │  - Salida                      │
│                         │  - Cierre (CheckBox)           │
│ Observación:            │  - Observación                 │
│ [____________]          │                                │
│                         │                                │
│ [Registrar Salida]      │                                │
│ [Aplicar Cierre]        │                                │
│ [Actualizar]            │                                │
└─────────────────────────┴────────────────────────────────┘
```

**Características**:
- Panel superior: Input de DNI para registro rápido de ingreso.
- Panel izquierdo: Lista de presentes + botones de acción.
- Panel derecho: Historial filtrable por fecha.
- Actualización automática cada 30 segundos.
- Estilo consistente: Fondo oscuro, botones rojos, hover crema.

### Validaciones

| Validación | Regla | Mensaje |
|------------|-------|---------|
| DNI vacío | No permitir | "El DNI/Código es obligatorio" |
| Asistente no registrado | Verificar en BD | "No existe un asistente con DNI... Debe registrarse primero" |
| Asistente inactivo | `Estado = false` | "El asistente está inactivo" |
| Entrada duplicada | Ya tiene asistencia abierta | "Ya tiene un ingreso registrado hoy sin salida" |
| Salida sin ingreso | No existe asistencia abierta | "No se encontró el registro de asistencia" |
| Salida duplicada | `HoraSalida != null` | "Este registro ya tiene una salida registrada" |
| Hora de salida inválida | `HoraSalida < HoraIngreso` | "La hora de salida no puede ser menor que la hora de ingreso" |

### Auditoría

Cada operación registra en `AuditLog`:

| Acción | Detalles |
|--------|----------|
| Registrar Ingreso | Nombre completo, DNI, hora de ingreso |
| Registrar Salida | Nombre completo, hora de salida, observación |
| Cierre Automático | Cantidad de asistencias cerradas, hora de cierre aplicada |

### Flujos típicos

#### 1. Registrar ingreso normal
1. Usuario ingresa DNI en campo superior.
2. Presiona ENTER o clic en "Registrar Ingreso".
3. Sistema valida asistente (existe, activo, sin ingreso previo).
4. Crea registro con `HoraIngreso = DateTime.Now`.
5. Audita acción.
6. Muestra mensaje de éxito con hora.
7. Actualiza lista de presentes.

#### 2. Registrar salida manual
1. Usuario selecciona asistente de lista "Presentes".
2. (Opcional) Ingresa observación.
3. Clic en "Registrar Salida".
4. Sistema muestra confirmación con hora de ingreso.
5. Si confirma, registra `HoraSalida = DateTime.Now`.
6. Audita acción.
7. Asistente desaparece de lista "Presentes".
8. Aparece en "Historial" con salida registrada.

#### 3. Aplicar cierre automático
1. Al final del día, usuario clic en "Aplicar Cierre".
2. Sistema cuenta asistencias sin salida.
3. Muestra confirmación: "¿Aplicar cierre a X asistencias?"
4. Si confirma, obtiene hora de cierre de configuración.
5. Para cada asistencia abierta:
   - `HoraSalida = Today + ConfiguracionEscuela.HoraCierre`
   - `HastaCierre = true`
   - `Observacion = "Cierre automático"`
6. Audita operación masiva.
7. Lista "Presentes" queda vacía.
8. Todas las asistencias aparecen en historial con checkbox "Cierre" marcado.

#### 4. Consultar historial
1. Usuario selecciona fecha en DatePicker.
2. Clic en "Buscar".
3. Sistema carga todas las asistencias de esa fecha.
4. DataGrid muestra: Ingreso, Salida, Cierre, Observación.

### Integración con otros módulos

- **Gestión de Asistentes**: Valida que el asistente exista y esté activo antes de permitir ingreso.
- **Configuración del Sistema**: Utiliza `HoraCierre` para función de cierre automático.
- **Auditoría**: Registra todas las operaciones con usuario y timestamp.

### Notas de implementación

- **Timer**: El DispatcherTimer debe ser detenido (`_refreshTimer.Stop()`) al cerrar la ventana para evitar fugas de memoria.
- **Rendimiento**: Las consultas usan `Include` para cargar relaciones necesarias en una sola consulta.
- **Concurrencia**: En entornos multiusuario, considerar agregar locks o versiones optimistas para evitar conflictos al registrar salidas simultáneas.

---

## 18) Reportes y Estadísticas

Módulo completo de consultas y exportación de reportes con **visualización detallada de asistentes** y exportaciones completas. Permite analizar asistencias por rango de fechas con 10+ tipos de reportes, visualización master-detail en UI, y exportación detallada a Excel/PDF.

### 16.1) Alcance y Objetivo

- Consultas de asistencia por rango de fechas o fecha específica
- **Ventana maximizada** para mejor visualización de datos
- Reportes agregados: Totales diarios (con detalle expandible de asistentes), Por categoría (diferenciado por grupo para Miembros), Por grupo (miembros)
- Reportes individuales: Historial completo por DNI o Nombre/Apellidos
- Paneles analíticos: Puntualidad, Permanencia hasta cierre, Asistencia por grupo detallada, Día de mayor asistencia, Top N asistentes más constantes
- **Exportación detallada**: Excel y PDF con información completa de cada asistente (DNI, nombre, categoría, grupo, fecha, horarios, estado)
- **Master-Detail UI**: DataGrid con RowDetailsTemplate para expandir y ver detalles de asistentes por día
- Interfaz organizada en 3 pestañas: Consultas Básicas, Historial Individual, Paneles Analíticos
- **Métricas simplificadas**: Eliminadas "Sin Salida", "% del Total" y "Asistentes Diferentes" (solo manual o hasta cierre)

### 16.2) DTOs de Reportes (Domain Layer)

Ubicación: `Marte.Domain/Entities/ReporteDTOs.cs`

**10 DTOs principales (actualizado v1.5):**

1. **ReporteTotalDiario**: Totales de asistencia por fecha con detalle de asistentes
```csharp
public class ReporteTotalDiario
{
    public DateTime Fecha { get; set; }
    public int TotalAsistencias { get; set; }
    public int TotalHastaCierre { get; set; }
    // NUEVO v1.5: Desglose detallado de asistentes del día
    public List<DetalleAsistenteDia> Asistentes { get; set; } = new List<DetalleAsistenteDia>();
}
```

2. **DetalleAsistenteDia**: ⭐ NUEVO v1.5 - Detalle de asistente en un día específico
```csharp
public class DetalleAsistenteDia
{
    public string DNI { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public string? NumeroGrupo { get; set; } // Solo para Miembros
    public TimeSpan HoraIngreso { get; set; }
    public TimeSpan? HoraSalida { get; set; }
    public bool HastaCierre { get; set; }
}
```

3. **ReportePorCategoria**: Totales agrupados por categoría (actualizado v1.5)
```csharp
public class ReportePorCategoria
{
    public string Categoria { get; set; } = string.Empty;
    public string? NumeroGrupo { get; set; } // NUEVO v1.5: Solo para Miembros
    public int Total { get; set; }
    public int HastaCierre { get; set; }
    // ELIMINADO v1.5: public double PorcentajeDelTotal { get; set; }
}
```

4. **ReportePorGrupo**: Totales por número de grupo (actualizado v1.5)
```csharp
public class ReportePorGrupo
{
    public string NumeroGrupo { get; set; } = string.Empty;
    public int TotalAsistencias { get; set; }
    // ELIMINADO v1.5: public int AsistentesDiferentes { get; set; }
    public double Promedio { get; set; }
}
```

5. **ReporteDiaMayorAsistencia**: Día con mayor asistencia (actualizado v1.5)
```csharp
public class ReporteDiaMayorAsistencia
{
    public DateTime Fecha { get; set; }
    public string DiaSemana { get; set; } = string.Empty;
    public int TotalAsistencias { get; set; }
    // ELIMINADO v1.5: public int AsistentesDiferentes { get; set; }
}
```

6-10. **Otros DTOs sin cambios**:
- `ReporteHistorialIndividual`: Historial completo de un asistente
- `ReportePuntualidad`: Análisis de puntualidad por asistente
- `ReporteHastaCierre`: Análisis de permanencia hasta cierre
- `ReporteAsistenciaGrupoDetalle`: Detalle de asistencia por grupo y fecha
- `ReporteTopConstantes`: Top N asistentes más constantes

### 16.3) Repositorio (Infrastructure Layer)

**Interface**: `Marte.Infrastructure/Interfaces/IReporteRepository.cs`

**11 métodos de consulta:**

```csharp
Task<List<ReporteTotalDiario>> GetTotalesDiariosAsync(DateTime fechaInicio, DateTime fechaFin);
Task<List<ReportePorCategoria>> GetTotalesPorCategoriaAsync(DateTime fechaInicio, DateTime fechaFin);
Task<List<ReportePorGrupo>> GetTotalesPorGrupoAsync(DateTime fechaInicio, DateTime fechaFin);
Task<List<ReporteHistorialIndividual>> GetHistorialIndividualPorDNIAsync(string dni, DateTime fechaInicio, DateTime fechaFin);
Task<List<ReporteHistorialIndividual>> GetHistorialIndividualPorNombreAsync(string nombres, string apellidos, DateTime fechaInicio, DateTime fechaFin);
Task<List<ReportePuntualidad>> GetReportePuntualidadAsync(DateTime fechaInicio, DateTime fechaFin, TimeSpan horaPuntualidad);
Task<List<ReporteHastaCierre>> GetReporteHastaCierreAsync(DateTime fechaInicio, DateTime fechaFin);
Task<List<ReporteAsistenciaGrupoDetalle>> GetReporteAsistenciaPorGrupoAsync(DateTime fechaInicio, DateTime fechaFin);
Task<ReporteDiaMayorAsistencia?> GetDiaMayorAsistenciaAsync(DateTime fechaInicio, DateTime fechaFin);
Task<List<ReporteTopConstantes>> GetTopAsistentesConstantesAsync(DateTime fechaInicio, DateTime fechaFin, int top);
```

**Implementación**: `Marte.Infrastructure/Repositories/ReporteRepository.cs` (~350 líneas)

Características principales:
- **LINQ complejo**: Queries con `GroupBy`, `Include`, `ThenInclude`, agregaciones
- **Cultura española**: `new CultureInfo("es-ES")` para nombres de días
- **Cálculos matemáticos**: Porcentajes con `Math.Round(..., 2)`, promedios de TimeSpan
- **Diccionarios**: Para contar miembros por grupo eficientemente
- **Ordenes**: `.OrderBy()`, `.OrderByDescending()` para resultados organizados

Ejemplo de query (GetReportePuntualidadAsync):
```csharp
var result = asistencias
    .GroupBy(a => new { 
        a.Asistente.DNI, 
        NombreCompleto = a.Asistente.Nombres + " " + a.Asistente.Apellidos,
        Categoria = a.Asistente.Categoria.Nombre 
    })
    .Select(g => new ReportePuntualidad {
        DNI = g.Key.DNI,
        NombreCompleto = g.Key.NombreCompleto,
        Categoria = g.Key.Categoria,
        TotalAsistencias = g.Count(),
        AsistenciasPuntuales = g.Count(a => a.HoraIngreso.TimeOfDay <= horaPuntualidad),
        PorcentajePuntualidad = g.Count() > 0 
            ? Math.Round((double)g.Count(a => a.HoraIngreso.TimeOfDay <= horaPuntualidad) / g.Count() * 100, 2) 
            : 0,
        PromedioHoraIngreso = TimeSpan.FromTicks((long)g.Average(a => a.HoraIngreso.TimeOfDay.Ticks))
    })
    .OrderByDescending(r => r.PorcentajePuntualidad)
    .ToList();
```

### 16.4) Servicio (Application Layer)

**Interface**: `Marte.Application/Interfaces/IReporteService.cs`

**20 métodos** (10 consultas + 10 exportaciones):

Consultas (pass-through a repository):
- `GetTotalesDiariosAsync(DateTime, DateTime)`
- `GetTotalesPorCategoriaAsync(DateTime, DateTime)`
- `GetTotalesPorGrupoAsync(DateTime, DateTime)`
- `GetHistorialIndividualPorDNIAsync(string, DateTime, DateTime)`
- `GetHistorialIndividualPorNombreAsync(string, string, DateTime, DateTime)`
- `GetReportePuntualidadAsync(DateTime, DateTime, TimeSpan)`
- `GetReporteHastaCierreAsync(DateTime, DateTime)`
- `GetReporteAsistenciaPorGrupoAsync(DateTime, DateTime)`
- `GetDiaMayorAsistenciaAsync(DateTime, DateTime)`
- `GetTopAsistentesConstantesAsync(DateTime, DateTime, int)`

Exportaciones (retornan `Task<string>` con ruta del archivo):
- `ExportarTotalesDiariosExcelAsync(DateTime, DateTime, string)` / `PDFAsync`
- `ExportarPorCategoriaExcelAsync(DateTime, DateTime, string)` / `PDFAsync`
- `ExportarHistorialIndividualExcelAsync(string, DateTime, DateTime, string)` / `PDFAsync`
- `ExportarPuntualidadExcelAsync(DateTime, DateTime, TimeSpan, string)`
- `ExportarHastaCierreExcelAsync(DateTime, DateTime, string)`
- `ExportarAsistenciaPorGrupoExcelAsync(DateTime, DateTime, string)`
- `ExportarTopConstantesExcelAsync(DateTime, DateTime, int, string)`

**Implementación**: `Marte.Application/Services/ReporteService.cs` (~700 líneas)

Dependencias externas:
- **ClosedXML 0.105.0**: Para exportación Excel (.xlsx)
- **QuestPDF 2025.7.3**: Para exportación PDF (A4)

Licencia QuestPDF: Se establece en constructor `QuestPDF.Settings.License = LicenseType.Community`

#### 16.4.1) Exportación Excel (ClosedXML)

Patrón de implementación:
```csharp
using var workbook = new XLWorkbook();
var worksheet = workbook.Worksheets.Add("NombreHoja");

// Título (row 1, merged, bold, 16pt)
worksheet.Cell(1, 1).Value = "REPORTE DE [NOMBRE]";
worksheet.Cell(1, 1).Style.Font.Bold = true;
worksheet.Cell(1, 1).Style.Font.FontSize = 16;
worksheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
worksheet.Range(1, 1, 1, totalColumnas).Merge();

// Período (row 2, merged)
worksheet.Cell(2, 1).Value = $"Período: {fechaInicio:dd/MM/yyyy} - {fechaFin:dd/MM/yyyy}";
worksheet.Range(2, 1, 2, totalColumnas).Merge();

// Headers (row 4, styled)
var headerRow = worksheet.Row(4);
headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#8B0000"); // Dark red
headerRow.Style.Font.FontColor = XLColor.White;
headerRow.Style.Font.Bold = true;
headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

// Data (starting row 5)
int currentRow = 5;
foreach (var item in datos) {
    worksheet.Cell(currentRow, 1).Value = item.Propiedad1;
    worksheet.Cell(currentRow, 2).Value = item.Propiedad2;
    // ... más columnas
    currentRow++;
}

// Totales (last row, bold)
worksheet.Cell(currentRow, 1).Value = "TOTAL:";
worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
worksheet.Cell(currentRow, 2).Value = totalCalculado;
worksheet.Cell(currentRow, 2).Style.Font.Bold = true;

// Auto-adjust columns
worksheet.Columns().AdjustToContents();

workbook.SaveAs(rutaArchivo);
```

Formato de columnas:
- Fechas: `dd/MM/yyyy`
- Horas: `HH:mm`
- Porcentajes: `0.00%` o valores numéricos con 2 decimales
- Booleanos: "Sí"/"No" en español

#### 16.4.2) Exportación PDF (QuestPDF)

Patrón de implementación:
```csharp
Document.Create(container => {
    container.Page(page => {
        page.Size(PageSizes.A4);
        page.Margin(2, Unit.Centimetre);
        page.PageColor(Colors.White);
        page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));
        
        // Header
        page.Header().Element(header => {
            header.Column(column => {
                column.Item().Text("REPORTE DE [NOMBRE]")
                    .FontSize(18).Bold().FontColor(Colors.Red.Darken3);
                column.Item().Text($"Período: {fechaInicio:dd/MM/yyyy} - {fechaFin:dd/MM/yyyy}")
                    .FontSize(11);
                column.Item().Text($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}")
                    .FontSize(9).FontColor(Colors.Grey.Medium);
            });
        });
        
        // Content (Table)
        page.Content().Element(content => {
            content.PaddingTop(1, Unit.Centimetre).Table(table => {
                // Column definitions
                table.ColumnsDefinition(columns => {
                    columns.RelativeColumn(2); // Columna 1 (flexible)
                    columns.RelativeColumn(1); // Columna 2
                    // ... más columnas
                });
                
                // Header row
                table.Header(header => {
                    header.Cell().Element(CellStyle).Background(Colors.Red.Darken3)
                        .Text("Columna 1").FontColor(Colors.White).Bold();
                    header.Cell().Element(CellStyle).Background(Colors.Red.Darken3)
                        .Text("Columna 2").FontColor(Colors.White).Bold();
                    // ... más headers
                });
                
                // Data rows
                foreach (var item in datos) {
                    table.Cell().Element(CellStyle).Text(item.Propiedad1);
                    table.Cell().Element(CellStyle).Text(item.Propiedad2.ToString());
                    // ... más celdas
                }
                
                // Totales row (fondo gris)
                table.Cell().Element(CellStyle).Background(Colors.Grey.Lighten3)
                    .Text("TOTAL:").Bold();
                table.Cell().Element(CellStyle).Background(Colors.Grey.Lighten3)
                    .Text(totalCalculado.ToString()).Bold();
            });
        });
        
        // Footer (page numbers)
        page.Footer().AlignCenter().Text(text => {
            text.Span("Página ");
            text.CurrentPageNumber();
            text.Span(" de ");
            text.TotalPages();
        });
    });
}).GeneratePdf(rutaArchivo);

// Helper for cell styling
IContainer CellStyle(IContainer container) {
    return container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5);
}
```

Características PDF:
- Tamaño: A4 (21 x 29.7 cm)
- Márgenes: 2 cm en todos los lados
- Fuente: Arial 10pt por defecto
- Color principal: Red.Darken3 (#8B0000 aproximado)
- Bordes: Gris claro entre celdas
- Paginación: Automática con números en footer

### 16.5) ViewModel (Presentation Layer)

**Ubicación**: `Marte.WPF/ViewModels/ReportesViewModel.cs` (~800 líneas)

**Propiedades** (19 totales):

Filtros:
- `DateTime FechaInicio` (default: `DateTime.Today.AddMonths(-1)`)
- `DateTime FechaFin` (default: `DateTime.Today`)
- `string DNIBusqueda`
- `string NombresBusqueda`
- `string ApellidosBusqueda`
- `TimeSpan HoraPuntualidad` (default: `new TimeSpan(19, 30, 0)`)
- `int TopConstantes` (default: 10)

Colecciones (ObservableCollection):
- `TotalesDiarios`
- `TotalesPorCategoria`
- `TotalesPorGrupo`
- `HistorialIndividual`
- `ReportePuntualidad`
- `ReporteHastaCierre`
- `ReportePorGrupoDetalle`
- `TopAsistentesConstantes`

Objeto único:
- `ReporteDiaMayorAsistencia? DiaMayorAsistencia`

Estado:
- `bool IsBusy` (para indicador de carga)
- `string StatusMessage` (mensajes de estado en UI)

**Comandos** (30 totales):

15 comandos de consulta:
- `ConsultarTotalesDiariosCommand`
- `ConsultarPorCategoriaCommand`
- `ConsultarPorGrupoCommand`
- `ConsultarHistorialPorDNICommand` (CanExecute: DNI no vacío)
- `ConsultarHistorialPorNombreCommand` (CanExecute: Nombres y Apellidos no vacíos)
- `ConsultarPuntualidadCommand`
- `ConsultarHastaCierreCommand`
- `ConsultarPorGrupoDetalleCommand`
- `ConsultarDiaMayorAsistenciaCommand`
- `ConsultarTopConstantesCommand`

15 comandos de exportación:
- `ExportarTotalesDiariosExcelCommand` / `PDFCommand`
- `ExportarPorCategoriaExcelCommand` / `PDFCommand`
- `ExportarHistorialIndividualExcelCommand` / `PDFCommand` (CanExecute: DNI no vacío)
- `ExportarPuntualidadExcelCommand`
- `ExportarHastaCierreExcelCommand`
- `ExportarPorGrupoDetalleExcelCommand`
- `ExportarTopConstantesExcelCommand`

**Patrón de comandos de consulta:**
```csharp
private async void ExecuteConsultar...(object? parameter)
{
    try {
        IsBusy = true;
        StatusMessage = "Consultando...";
        
        var datos = await _reporteService.Get...Async(...);
        
        Collection.Clear();
        foreach (var item in datos) {
            Collection.Add(item);
        }
        
        StatusMessage = $"Se encontraron {Collection.Count} registros";
    }
    catch (Exception ex) {
        MessageBox.Show($"Error al consultar: {ex.Message}", "Error", 
            MessageBoxButton.OK, MessageBoxImage.Error);
        StatusMessage = "Error en la consulta";
    }
    finally {
        IsBusy = false;
    }
}
```

**Patrón de comandos de exportación:**
```csharp
private async void ExecuteExportar...(object? parameter)
{
    try {
        var saveDialog = new SaveFileDialog {
            FileName = $"NombreReporte_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
            Filter = "Archivos Excel (*.xlsx)|*.xlsx",
            Title = "Exportar Reporte a Excel"
        };
        
        if (saveDialog.ShowDialog() == true) {
            IsBusy = true;
            StatusMessage = "Exportando...";
            
            await _reporteService.Exportar...Async(..., saveDialog.FileName);
            
            MessageBox.Show(
                $"Reporte exportado exitosamente:\n{saveDialog.FileName}", 
                "Exportación Exitosa", 
                MessageBoxButton.OK, 
                MessageBoxImage.Information
            );
            StatusMessage = "Exportación completada";
        }
    }
    catch (Exception ex) {
        MessageBox.Show($"Error al exportar: {ex.Message}", "Error", 
            MessageBoxButton.OK, MessageBoxImage.Error);
        StatusMessage = "Error en la exportación";
    }
    finally {
        IsBusy = false;
    }
}
```

### 16.6) Vista (Presentation Layer)

**Ubicación**: `Marte.WPF/Views/ReportesView.xaml` (~900 líneas)

**Estructura general:**
```
Window (1600x850px, centered)
└── Grid (4 rows)
    ├── [Row 0] Header: "REPORTES Y ESTADÍSTICAS" (red banner, 50px)
    ├── [Row 1] Global Filters (100px):
    │   ├── DatePicker: Fecha Inicio → Fecha Fin
    │   └── StatusMessage TextBlock
    ├── [Row 2] TabControl (main content, *):
    │   ├── Tab 1: 📊 Consultas Básicas
    │   ├── Tab 2: 👤 Historial Individual
    │   └── Tab 3: 📈 Paneles Analíticos
    └── [Row 3] Loading Indicator (40px, visible when IsBusy)
```

**Tab 1 - Consultas Básicas** (ScrollViewer con 3 secciones):

1. **Totales Diarios**
   - Border con Grid (3 rows)
   - Título: "TOTALES DIARIOS" (18pt, bold, red)
   - Botones: Consultar, Exportar Excel, Exportar PDF
   - DataGrid (Height=250): Fecha, Total Asistencias, Hasta Cierre, Sin Salida

2. **Por Categoría**
   - Border con Grid (3 rows)
   - Título: "POR CATEGORÍA"
   - Botones: Consultar, Exportar Excel, Exportar PDF
   - DataGrid (Height=200): Categoría, Total, Hasta Cierre, % del Total

3. **Por Grupo (Miembros)**
   - Border con Grid (3 rows)
   - Título: "POR GRUPO (MIEMBROS)"
   - Botón: Consultar only
   - DataGrid (Height=200): Grupo, Total Asistencias, Asistentes Diferentes, Promedio

**Tab 2 - Historial Individual** (ScrollViewer):

- **Búsqueda** (Border con Grid):
  * Horizontal row: Label "DNI/Código", TextBox (200px), Botones (Consultar, Exportar Excel, Exportar PDF)
  * Horizontal row: Label "Nombres", TextBox (150px), Label "Apellidos", TextBox (150px), Botón Consultar
  
- **Resultados** (Border con Grid):
  * DataGrid: Fecha, Hora Ingreso, Hora Salida, Hasta Cierre (CheckBox), Observación
  * StringFormat: Fechas `{0:dd/MM/yyyy}`, Horas `{0:HH:mm}`

**Tab 3 - Paneles Analíticos** (ScrollViewer con 5 secciones):

1. **Tasa de Puntualidad**
   - Input: Hora límite TextBox (default: "19:30"), hint "(Formato: 19:30)"
   - Botones: Consultar, Exportar Excel
   - DataGrid (Height=250, 7 cols): DNI, Nombre Completo, Categoría, Total Asist., Puntuales, % Puntual, Prom. Ingreso

2. **Asistentes Hasta Cierre**
   - Botones: Consultar, Exportar Excel
   - DataGrid (Height=250, 6 cols): DNI, Nombre Completo, Categoría, Total Días, Días Hasta Cierre, % Hasta Cierre

3. **Asistencia Por Grupo (Detalle)**
   - Botones: Consultar, Exportar Excel
   - DataGrid (Height=250, 5 cols): Grupo, Fecha, Total Miembros, Asistieron, % Asist.

4. **Día de Mayor Asistencia**
   - Botón: Consultar only
   - Display especial (Grid con 3 columnas, VerticalAlignment=Center):
     * Columna 1: FECHA (18pt label), dd/MM/yyyy (32pt), DiaSemana (14pt, red)
     * Columna 2: TOTAL ASISTENCIAS (11pt label), Número (32pt, red, bold)
     * Columna 3: ASISTENTES DIFERENTES (11pt label), Número (32pt, red, bold)
   - Bindings con `TargetNullValue="--"` para manejo de nulos

5. **Top Asistentes Más Constantes**
   - Input: Top TextBox (default: "10"), hint "Ingrese el número de asistentes a mostrar"
   - Botones: Consultar, Exportar Excel
   - DataGrid (Height=300, 7 cols): Pos., DNI, Nombre Completo, Categoría, Grupo, Total Asist., % Asist.

**Estilos consistentes:**
- Fondo: `#1E1E1E` (gris oscuro)
- Acento: `#8B0000` (rojo oscuro - MARTE theme)
- Botones: Fondo rojo, texto blanco, hover cream `#80FFFEEE`
- DataGrid: Fondo oscuro `#2D2D30`, headers rojos, hover/selection effects
- Bordes: 2px red, CornerRadius 8px
- Fuente: Times New Roman (CinzelFont)

### 16.7) Mejoras v1.5 - Exportación Detallada y UI Master-Detail ⭐

#### 16.7.1) Ventana Maximizada

**Cambio en ReportesView.xaml:**
```xaml
<!-- ANTES (v1.4) -->
<Window Height="850" Width="1600">

<!-- AHORA (v1.5) -->
<Window WindowState="Maximized">
```

La ventana ahora se abre maximizada automáticamente para aprovechar todo el espacio de pantalla y visualizar mejor los datos detallados.

#### 16.7.2) Master-Detail en Totales Diarios

**Implementación de RowDetailsTemplate:**

El DataGrid de Totales Diarios ahora incluye un patrón master-detail que permite expandir cada fila para ver el detalle completo de asistentes.

```xaml
<DataGrid RowDetailsVisibilityMode="VisibleWhenSelected">
    <!-- Columnas principales -->
    <DataGrid.Columns>
        <DataGridTextColumn Header="Fecha" />
        <DataGridTextColumn Header="Total Asistencias" />
        <DataGridTextColumn Header="Hasta Cierre" />
        <DataGridTemplateColumn Header="Detalle">
            <TextBlock Text="👁 Ver detalle" />
        </DataGridTemplateColumn>
    </DataGrid.Columns>
    
    <!-- Detalle expandible -->
    <DataGrid.RowDetailsTemplate>
        <DataTemplate>
            <DataGrid ItemsSource="{Binding Asistentes}">
                <DataGrid.Columns>
                    <DataGridTextColumn Header="DNI" Binding="{Binding DNI}"/>
                    <DataGridTextColumn Header="Nombre Completo" Binding="{Binding NombreCompleto}"/>
                    <DataGridTextColumn Header="Categoría" Binding="{Binding Categoria}"/>
                    <DataGridTextColumn Header="Grupo" Binding="{Binding NumeroGrupo, TargetNullValue='-'}"/>
                    <DataGridTextColumn Header="Hora Ingreso" Binding="{Binding HoraIngreso, StringFormat='hh\\:mm'}"/>
                    <DataGridTextColumn Header="Hora Salida" Binding="{Binding HoraSalida, StringFormat='hh\\:mm', TargetNullValue='-'}"/>
                    <DataGridTemplateColumn Header="Estado">
                        <!-- Verde si HastaCierre=true, Naranja si false -->
                        <TextBlock Text="{Binding HastaCierre, Converter=...}"/>
                    </DataGridTemplateColumn>
                </DataGrid.Columns>
            </DataGrid>
        </DataTemplate>
    </DataGrid.RowDetailsTemplate>
</DataGrid>
```

**Interacción del usuario:**
1. Usuario consulta Totales Diarios
2. Ve tabla resumen con totales por fecha
3. Hace clic en cualquier fila
4. Se expande mostrando tabla interna con todos los asistentes del día
5. Puede ver: DNI, Nombre, Categoría, Grupo, Horarios, Estado
6. Al hacer clic en otra fila, la anterior se colapsa automáticamente

#### 16.7.3) Exportación Excel Detallada

**Estructura del archivo Excel (Totales Diarios):**

```
REPORTE DETALLADO DE ASISTENCIAS DIARIAS
Período: 01/10/2025 - 18/10/2025

┌──────────┬──────────┬─────────────────┬───────────┬───────┬──────────────┬─────────────┬───────────────┐
│  Fecha   │   DNI    │ Nombre Completo │ Categoría │ Grupo │ Hora Ingreso │ Hora Salida │    Estado     │
├──────────┼──────────┼─────────────────┼───────────┼───────┼──────────────┼─────────────┼───────────────┤
│15/10/2025│ 12345678 │ Juan Pérez      │ Miembros  │  G1   │    18:30     │    20:45    │ Salida Manual │
│15/10/2025│ 87654321 │ Ana García      │ Visitantes│   -   │    18:45     │      -      │ Hasta Cierre  │
│16/10/2025│ 12345678 │ Juan Pérez      │ Miembros  │  G1   │    18:25     │      -      │ Hasta Cierre  │
...

RESUMEN GENERAL
Total de días:         18
Total asistencias:     1,250
Hasta cierre:          875
```

**Características:**
- ✅ Cada fila = una asistencia individual con todos sus datos
- ✅ Colores alternados por día (blanco/gris) para mejor lectura
- ✅ NumeroGrupo visible solo para categoría "Miembros", "-" para otros
- ✅ Resumen consolidado al final
- ✅ Formato de horas consistente (HH:mm)
- ✅ Headers con fondo rojo (#8B0000) y texto blanco

**Cambios en código:**
```csharp
// ANTES (v1.4) - Solo totales
foreach (var item in datos) {
    worksheet.Cell(row, 1).Value = item.Fecha;
    worksheet.Cell(row, 2).Value = item.TotalAsistencias;
    worksheet.Cell(row, 3).Value = item.TotalHastaCierre;
}

// AHORA (v1.5) - Detalle completo
foreach (var dia in datos) {
    foreach (var asistente in dia.Asistentes) {
        worksheet.Cell(row, 1).Value = dia.Fecha;
        worksheet.Cell(row, 2).Value = asistente.DNI;
        worksheet.Cell(row, 3).Value = asistente.NombreCompleto;
        worksheet.Cell(row, 4).Value = asistente.Categoria;
        worksheet.Cell(row, 5).Value = asistente.NumeroGrupo ?? "-";
        worksheet.Cell(row, 6).Value = asistente.HoraIngreso.ToString(@"hh\:mm");
        worksheet.Cell(row, 7).Value = asistente.HoraSalida?.ToString(@"hh\:mm") ?? "-";
        worksheet.Cell(row, 8).Value = asistente.HastaCierre ? "Hasta Cierre" : "Salida Manual";
        row++;
    }
}
```

#### 16.7.4) Exportación PDF Detallada (Landscape)

**Cambio de orientación:**
```csharp
// ANTES (v1.4)
page.Size(PageSizes.A4);

// AHORA (v1.5)
page.Size(PageSizes.A4.Landscape());
```

**Estructura del PDF (Totales Diarios):**
```
REPORTE DETALLADO DE ASISTENCIAS DIARIAS
Período: 01/10/2025 - 18/10/2025

📅 15/10/2025 - Total: 45 asistentes (32 hasta cierre)
┌──────────┬─────────────────┬───────────┬───────┬─────────┬─────────┬───────────────┐
│   DNI    │ Nombre Completo │ Categoría │ Grupo │ Ingreso │ Salida  │    Estado     │
├──────────┼─────────────────┼───────────┼───────┼─────────┼─────────┼───────────────┤
│ 12345678 │ Juan Pérez      │ Miembros  │  G1   │  18:30  │  20:45  │ Salida Manual │
│ 87654321 │ Ana García      │ Visitantes│   -   │  18:45  │    -    │ Hasta Cierre  │
...

📅 16/10/2025 - Total: 38 asistentes (28 hasta cierre)
[Tabla similar...]

RESUMEN GENERAL
┌──────────────────┬───────────────────┬────────────────────────┐
│ Total de días    │ Total asistencias │ Total hasta cierre     │
│       18         │       1,250       │         875            │
└──────────────────┴───────────────────┴────────────────────────┘
```

**Características:**
- ✅ Formato apaisado (Landscape) para mayor espacio horizontal
- ✅ Icono para destacar cada fecha
- ✅ Contador de asistentes en título de cada día
- ✅ Colores semánticos: Verde para "Hasta Cierre", Naranja para "Salida Manual"
- ✅ Font size 8 para caber más información
- ✅ Cada día tiene su propia tabla separada
- ✅ Paginación automática si hay muchos días

#### 16.7.5) Exportación Por Categoría Detallada

**Agrupación por Categoría y Grupo:**

Excel y PDF ahora muestran:
1. **Lista detallada** de todas las asistencias ordenadas por Categoría → Grupo → Nombre
2. **Tabla de resumen** al final con totales por categoría/grupo

```
REPORTE DETALLADO POR CATEGORÍA

┌───────────┬───────┬──────────┬─────────────────┬──────────┬──────────────┬─────────────┬───────────────┐
│ Categoría │ Grupo │   DNI    │ Nombre Completo │  Fecha   │ Hora Ingreso │ Hora Salida │    Estado     │
├───────────┼───────┼──────────┼─────────────────┼──────────┼──────────────┼─────────────┼───────────────┤
│ Miembros  │  G1   │ 12345678 │ Juan Pérez      │15/10/2025│    18:30     │    20:45    │ Salida Manual │
│ Miembros  │  G1   │ 12345678 │ Juan Pérez      │16/10/2025│    18:25     │      -      │ Hasta Cierre  │
│ Miembros  │  G2   │ 11223344 │ Pedro López     │15/10/2025│    19:00     │      -      │ Hasta Cierre  │
│Visitantes │   -   │ 87654321 │ Ana García      │15/10/2025│    18:45     │      -      │ Hasta Cierre  │
...

RESUMEN POR CATEGORÍA
┌───────────┬───────┬───────────────────┬──────────────┐
│ Categoría │ Grupo │ Total Asistencias │ Hasta Cierre │
├───────────┼───────┼───────────────────┼──────────────┤
│ Miembros  │  G1   │        150        │     120      │
│ Miembros  │  G2   │        145        │     110      │
│Visitantes │   -   │         80        │      55      │
└───────────┴───────┴───────────────────┴──────────────┘
```

**En PDF**, cada categoría/grupo tiene su propia sección destacada:
```
[Icono] Miembros - Grupo G1 (150 asistencias)
[Tabla con todos los asistentes del grupo]

📋 Miembros - Grupo G2 (145 asistencias)
[Tabla con todos los asistentes del grupo]

📋 Visitantes (80 asistencias)
[Tabla con todos los visitantes]
```

#### 16.7.6) Métricas Eliminadas v1.5

**Cambios en DTOs y UI:**

1. **TotalSinSalida** - ELIMINADO
   - Razón: Concepto confuso, solo existe "Salida Manual" o "Hasta Cierre"
   - Impacto: Columna removida de UI y exportaciones

2. **PorcentajeDelTotal** - ELIMINADO  
   - Razón: Métrica innecesaria que no agrega valor analítico
   - Impacto: Columna removida de reporte Por Categoría

3. **AsistentesDiferentes** - ELIMINADO
   - Razón: Referencia no útil para el análisis
   - Impacto: Removido de Por Grupo y Día Mayor Asistencia

**Cambios en Repository:**
```csharp
// GetTotalesPorCategoriaAsync
// ANTES: GroupBy solo por Categoria
.GroupBy(a => a.Asistente.Categoria.Nombre)

// AHORA: GroupBy por Categoria Y NumeroGrupo (para Miembros)
.GroupBy(a => new { 
    Categoria = a.Asistente.Categoria.Nombre,
    NumeroGrupo = a.Asistente.Categoria.Nombre == "Miembros" 
        ? a.Asistente.NumeroGrupo 
        : null
})
```

#### 16.7.7) Diferenciación de Miembros por Grupo

**Lógica condicional en queries:**

Solo para asistentes con categoría "Miembros", se muestra el NumeroGrupo. Para otras categorías se muestra "-" o null.

```csharp
NumeroGrupo = a.Asistente.Categoria.Nombre == "Miembros" 
    ? a.Asistente.NumeroGrupo 
    : null
```

**Impacto en UI y exportaciones:**
- **DataGrid**: Columna "Grupo" muestra número para Miembros, "-" para otros
- **Excel**: Columna "Grupo" con valores condicionales
- **PDF**: Grupos separados solo para Miembros (ej: "Miembros - Grupo G1")

### 16.8) Integración y DI

**App.xaml.cs** (ConfigureServices):
```csharp
// Repository
services.AddScoped<IReporteRepository, ReporteRepository>();

// Service
services.AddScoped<IReporteService, ReporteService>();

// View
services.AddTransient<ReportesView>();
```

**MainWindow.xaml.cs** (OnReportesClick):
```csharp
private void OnReportesClick(object sender, RoutedEventArgs e)
{
    try {
        var reporteService = _serviceProvider.GetRequiredService<IReporteService>();
        var viewModel = new ReportesViewModel(reporteService, _currentUser!);
        var window = new ReportesView(viewModel);
        window.ShowDialog();
    }
    catch (Exception ex) {
        MessageBox.Show($"Error al abrir Reportes y Estadísticas: {ex.Message}", 
            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
```

### 16.8) Workflows Típicos

**Workflow 1: Consultar Totales Diarios**
1. Usuario abre Reportes desde MainWindow
2. Selecciona fechas en filtros globales (default: último mes)
3. Tab 1 → Sección "Totales Diarios" → Click "Consultar"
4. ViewModel ejecuta `ExecuteConsultarTotalesDiarios`
5. Service llama Repository → Query LINQ con GroupBy por Fecha
6. ObservableCollection se actualiza → DataGrid se refresca
7. Usuario ve: Fecha, Total Asistencias, Hasta Cierre, Sin Salida por cada día

**Workflow 2: Exportar a Excel**
1. Usuario consulta datos (workflow 1)
2. Click "Exportar Excel"
3. SaveFileDialog se abre con nombre sugerido: `TotalesDiarios_20251018_143500.xlsx`
4. Usuario elige ubicación y confirma
5. ViewModel ejecuta `ExportarTotalesDiariosExcelAsync`
6. Service crea XLWorkbook, aplica styling, escribe datos
7. Archivo se guarda y MessageBox confirma éxito
8. Usuario puede abrir archivo en Excel

**Workflow 3: Historial Individual por DNI**
1. Usuario ingresa DNI en TextBox (ej: "12345678")
2. Click "Consultar"
3. ViewModel valida DNI no vacío (CanExecute)
4. Service llama Repository con filtro por DNI
5. Query LINQ con `Include(Asistente)` obtiene historial completo
6. DataGrid muestra: Fecha, Hora Ingreso, Hora Salida, Hasta Cierre, Observación
7. Usuario puede exportar a Excel o PDF

**Workflow 4: Panel de Puntualidad**
1. Usuario configura hora límite: "19:30" (default)
2. Tab 3 → Sección "Tasa de Puntualidad" → Click "Consultar"
3. ViewModel parsea TimeSpan de TextBox
4. Service llama Repository con horaPuntualidad parameter
5. Query calcula porcentaje de llegadas puntuales por asistente
6. DataGrid muestra: DNI, Nombre, Categoría, Total, Puntuales, %, Promedio
7. Ordenado por % Puntualidad descendente

**Workflow 5: Exportar a PDF**
1. Usuario consulta "Por Categoría"
2. Click "Exportar PDF"
3. SaveFileDialog con filtro: `*.pdf`
4. Usuario confirma ubicación
5. Service crea Document con QuestPDF
6. Aplica layout A4, headers con título y período, tabla con datos, footer con páginas
7. PDF se genera y MessageBox confirma
8. Usuario puede abrir/imprimir PDF

### 16.9) Validaciones y Reglas

- **Rango de fechas**: FechaInicio ≤ FechaFin (validado en UI con DatePicker)
- **DNI**: No vacío para consultas/exportaciones individuales (CanExecute)
- **Nombres+Apellidos**: Ambos requeridos para búsqueda por nombre (CanExecute)
- **Hora puntualidad**: Formato `HH:mm`, validado con `TimeSpan.TryParse`
- **Top N**: Entero positivo, default 10, validado con `int.TryParse`
- **Asistente no encontrado**: Si DNI no existe, query retorna lista vacía (no exception)
- **Sin datos**: DataGrid vacío con mensaje en StatusMessage
- **Archivos**: Extensión `.xlsx` o `.pdf` según tipo de exportación

### 16.10) Consideraciones Técnicas

- **Cultura española**: `CultureInfo("es-ES")` para día de semana en español
- **Memoria**: ObservableCollection.Clear() antes de repoblar para evitar memory leaks
- **Async/await**: Todos los métodos de consulta y exportación son asíncronos
- **Error handling**: Try-catch en todos los handlers con MessageBox de error
- **IsBusy**: Desactiva UI durante operaciones largas (consultas, exportaciones)
- **SaveFileDialog**: Integrado para selección de ubicación y nombre de archivo
- **Timestamp**: Nombres de archivo incluyen `yyyyMMdd_HHmmss` para evitar sobreescrituras
- **Dependencies**: ClosedXML y QuestPDF son NuGet packages en Marte.Application.csproj
- **License**: QuestPDF requiere `LicenseType.Community` para uso gratuito (establecido en constructor)

---

## 20) Extensibilidad y próximos pasos

**Módulos Implementados** (Versión 1.3.0):
- ✅ Sistema de autenticación y sesiones con BCrypt
- ✅ Gestión de usuarios con RBAC
- ✅ Dashboard con indicadores en tiempo real
- ✅ Gestión de categorías y asistentes
- ✅ Control de asistencia con cierre automático
- ✅ Reportes y estadísticas (10+ tipos)
- ✅ Exportación Excel y PDF
- ✅ Módulo de auditoría con exportación TXT
- ✅ Sistema de distribución con LocalDB
- ✅ Instaladores automatizados PowerShell

**Posibles Mejoras Futuras**:
- [ ] Gráficos interactivos en Dashboard (ChartJS/LiveCharts)
- [ ] Notificaciones push para eventos importantes
- [ ] Exportación a más formatos (CSV, JSON)
- [ ] Tema claro/oscuro personalizable
- [ ] Backup automatizado programado
- [ ] Sincronización con SQL Server en red (modo híbrido)
- [ ] App móvil complementaria (Xamarin/MAUI)
- [ ] API REST para integraciones externas
- [ ] Reconocimiento facial para registro de asistencia
- [ ] Multi-idioma (i18n)

**Guía de Actualización de Documentación**:

Al implementar nuevos módulos, agregar sección siguiendo este formato:

```markdown
## N) Nombre del Módulo

### Objetivo
Descripción breve del propósito

### Entidad (si aplica)
public class NuevaEntidad { /* campos */ }

### Servicio
- Interface: INuevoServicio
- Implementación: NuevoServicio
- Métodos principales con firmas

### Repositorio (si aplica)
- Métodos CRUD específicos

### ViewModel
- Propiedades principales
- Comandos (ICommand)
- Lógica de presentación

### Vista
- Archivo XAML
- Controles principales
- Bindings importantes

### Validaciones y Reglas de Negocio
Tabla con reglas específicas

### Auditoría
Qué acciones se registran

### Flujos Típicos
Workflow paso a paso

### Migraciones (si aplica)
Nombre y descripción de migración EF Core

### Comandos Útiles
Comandos CLI o PowerShell relevantes
```

---

## 21) Referencias rápidas

**Documentación de Usuario**:
- `GUIA_USUARIO_MARTE.md`: Guía completa de usabilidad para interfaz de usuario
- `README-INSTALACION.md`: Guía de instalación para usuarios finales

**Documentación Técnica**:
- `CONEXIONES.md`: Guía de conexiones SQL Server (Desarrollo)
- `CONEXIONES-LOCALDB.md`: Configuración LocalDB (Producción)
- `GUIA_INSTALACION_CLIENTE.md`: Comparativa LocalDB vs SQL Server

**Migraciones Aplicadas**:
- `20251016233131_InitialCreateSqlServer`: Estructura inicial
- `20251018072026_AddNumeroGrupoAndEstadoToAsistente`: Campos para asistentes
- `20251018072747_RemoveNumeroGrupoFromCategoria`: Simplificación de categorías

**Paquetes NuGet Principales**:
- Microsoft.EntityFrameworkCore 9.0.10
- Microsoft.EntityFrameworkCore.SqlServer 9.0.10
- Microsoft.Extensions.DependencyInjection 9.0.0
- BCrypt.Net-Next 4.0.3 (Hash de contraseñas)
- ClosedXML 0.105.0 (Exportación Excel)
- QuestPDF 2025.7.3 (Generación PDF)

**Scripts PowerShell**:
- `install-marte.ps1`: Instalador automatizado con .NET 9 + LocalDB
- `uninstall-marte.ps1`: Desinstalador con opción de backup
- `crear-paquete-distribucion.ps1`: Empaquetador para distribución
- `start-hotreload.ps1`: Hot reload para desarrollo (si existe)

**Ubicaciones en Producción**:
- Aplicación: `C:\Program Files\MARTE\`
- Base de Datos: `%LocalAppData%\MARTE\Data\MarteDb.mdf`
- Exportaciones: Ubicación seleccionada por usuario

**Credenciales por Defecto**:
- Usuario: `druagurto`
- Contraseña: `@Druagurto00`
- **Importante**: Cambiar después del primer inicio

**Comandos Útiles (Desarrollo)**:
```bash
# Compilar solución
dotnet build Marte.sln -c Debug

# Compilar para producción
dotnet build Marte.sln -c Release

# Crear migración
dotnet ef migrations add NombreMigracion --project Marte.Infrastructure --startup-project Marte.WPF

# Aplicar migraciones
dotnet ef database update --project Marte.Infrastructure --startup-project Marte.WPF

# Crear paquete de distribución
.\crear-paquete-distribucion.ps1
```

**Soporte y Contacto**:
- Desarrollador: DRUAGURTO
- Framework: .NET 9.0
- Base de Datos: SQL Server LocalDB 2022
- Versión: 1.3.0
- Fecha: 18/10/2025

---

**Última actualización**: 18/10/2025 - Versión 1.3.0

### Objetivo
Mantener trazabilidad completa de todas las acciones relevantes dentro del sistema, permitiendo consultar y exportar el historial de operaciones realizadas por los usuarios.

### Funciones Principales

#### Consulta de Bitácora
- **Filtro por Rango de Fechas**: Permite buscar registros entre fecha inicio y fecha fin
- **Filtro por Usuario**: Búsqueda por nombre de usuario específico
- **Combinación de Filtros**: Aplicación simultánea de múltiples criterios
- **Ver Todos**: Carga completa del historial sin filtros
- **Visualización en Tiempo Real**: DataGrid con actualización automática

#### Exportación
- **Formato TXT**: Exportación del historial en archivo de texto plano
- **Nombre Automático**: Formato `AuditoriaMarte_YYYYMMDD_HHmmss.txt`
- **Formato Estructurado**: Incluye encabezado, detalles y pie de reporte
- **Encoding UTF-8**: Soporte completo para caracteres especiales

### Servicios y Componentes

#### Infrastructure Layer
- `IAuditLogRepository`: Interfaz del repositorio
  - `GetAllAsync()`: Obtiene todos los registros
  - `GetByUsuarioAsync(string usuario)`: Filtro por usuario
  - `GetByFechaRangoAsync(DateTime inicio, DateTime fin)`: Filtro por fechas
  - `GetByFiltrosAsync(DateTime? inicio, DateTime? fin, string? usuario)`: Filtro combinado
- `AuditLogRepository`: Implementación con Entity Framework

#### Application Layer
- `IAuditLogService`: Interfaz del servicio
- `AuditLogService`: Lógica de negocio y exportación
  - `ExportarATxtAsync()`: Genera archivo TXT con formato estructurado

#### Presentation Layer  
- `AuditLogViewModel`: ViewModel con lógica de presentación
  - Comandos: `BuscarCommand`, `LimpiarFiltrosCommand`, `ExportarCommand`, `CargarTodosCommand`
  - Propiedades: `FechaInicio`, `FechaFin`, `UsuarioFiltro`, `Logs`, `TotalRegistros`
- `AuditLogView.xaml`: Ventana WPF con diseño temático MARTE

### Entidades Afectadas
- `AuditLog`: Entidad principal con campos:
  - `Id`: Identificador único (Guid)
  - `Entidad`: Nombre de la entidad afectada
  - `EntidadId`: ID de la entidad afectada
  - `Accion`: Descripción de la acción realizada
  - `Usuario`: Usuario que ejecutó la acción
  - `Fecha`: Timestamp de la operación
  - `Detalle`: Información adicional opcional

### Integración con Sistema
- **Acceso**: Botón "Auditoría" en MainWindow
- **Seguridad**: Disponible para todos los usuarios autenticados
- **Auditoría de Auditoría**: Las consultas NO generan registros de auditoría (solo lectura)

### Formato de Exportación TXT

```
╔═══════════════════════════════════════════════════════════════════════════════╗
║                     SISTEMA MARTE - BITÁCORA DE AUDITORÍA                    ║
╚═══════════════════════════════════════════════════════════════════════════════╝

Fecha de Generación: DD/MM/YYYY HH:mm:ss
Total de Registros: N

═══════════════════════════════════════════════════════════════════════════════

Fecha/Hora: DD/MM/YYYY HH:mm:ss
Usuario: [NombreUsuario]
Acción: [Descripción]
Entidad: [NombreEntidad]
Detalle: [Información adicional]
───────────────────────────────────────────────────────────────────────────────

[Más registros...]

╔═══════════════════════════════════════════════════════════════════════════════╗
║                            FIN DEL REPORTE                                    ║
╚═══════════════════════════════════════════════════════════════════════════════╝
```

### Casos de Uso
1. **Auditoría de Seguridad**: Revisar quién y cuándo accedió al sistema
2. **Investigación de Cambios**: Rastrear modificaciones en datos críticos
3. **Cumplimiento**: Documentar operaciones para auditorías externas
4. **Análisis de Actividad**: Estudiar patrones de uso del sistema
5. **Respaldo Legal**: Generar evidencia de operaciones realizadas

### Comandos Útiles
```bash
# Compilar después de agregar el módulo
dotnet build

# Verificar registros en base de datos (Desarrollo)
SELECT TOP 100 * FROM AuditLogs ORDER BY Fecha DESC

# Limpiar registros antiguos (mantenimiento programado)
DELETE FROM AuditLogs WHERE Fecha < DATEADD(YEAR, -2, GETDATE())
```

---
