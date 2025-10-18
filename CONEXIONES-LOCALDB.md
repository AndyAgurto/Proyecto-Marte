# Configuración de Conexión - Sistema MARTE (LocalDB)

**Versión**: 1.3.0  
**Fecha**: 18 de octubre de 2025  
**Base de Datos**: SQL Server LocalDB  
**Modo**: Aplicación Standalone (Sin servidor)

---

## 📋 Configuración Actual - LocalDB

### Connection String en Producción (Release)

```csharp
Server=(localdb)\mssqllocaldb;
AttachDbFilename=%LocalAppData%\MARTE\Data\MarteDb.mdf;
Database=MarteDb;
Trusted_Connection=True;
MultipleActiveResultSets=True;
```

**Características**:
- ✅ **No requiere SQL Server instalado**
- ✅ **Base de datos portable en AppData del usuario**
- ✅ **Instalación automática de LocalDB**
- ✅ **Usuario NO configura nada**
- ✅ **Ideal para instalación en clientes**

---

### Connection String en Desarrollo (Debug)

```csharp
Server=.\DRUAGURTO;
Database=MarteDb;
Trusted_Connection=True;
TrustServerCertificate=True;
```

**Características**:
- 🔧 **Desarrollo en instancia DRUAGURTO**
- 🔧 **Permite crear/aplicar migraciones**
- 🔧 **Testing con SQL Server completo**

---

## 📁 Ubicación de Archivos

### Base de Datos (Producción)

```
C:\Users\[NombreUsuario]\AppData\Local\MARTE\Data\
├── MarteDb.mdf          (Archivo de base de datos)
└── MarteDb_log.ldf      (Archivo de log transaccional)
```

**Acceso rápido**: 
- Presionar `Win + R`
- Escribir: `%LocalAppData%\MARTE\Data`
- Enter

---

### Archivos de Aplicación

```
C:\Program Files\MARTE\
├── Marte.WPF.exe
├── Marte.Application.dll
├── Marte.Infrastructure.dll
├── Marte.Domain.dll
└── ... (dependencias)
```

---

## 🔧 Configuración en Código

### App.xaml.cs (Runtime)

```csharp
private void ConfigureServices(IServiceCollection services)
{
    // Configuración con compilación condicional
    var appDataPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "MARTE",
        "Data"
    );
    
    Directory.CreateDirectory(appDataPath);
    
    var dbFilePath = Path.Combine(appDataPath, "MarteDb.mdf");
    
    #if DEBUG
    // Desarrollo: Usar DRUAGURTO
    var connectionString = @"Server=.\DRUAGURTO;Database=MarteDb;Trusted_Connection=True;TrustServerCertificate=True;";
    #else
    // Producción: Usar LocalDB
    var connectionString = $@"Server=(localdb)\mssqllocaldb;
                             AttachDbFilename={dbFilePath};
                             Database=MarteDb;
                             Trusted_Connection=True;
                             MultipleActiveResultSets=True;";
    #endif
    
    services.AddDbContext<MarteDbContext>(options =>
        options.UseSqlServer(connectionString));
}
```

---

### MarteDesignTimeDbContextFactory.cs (Migraciones)

```csharp
public MarteDbContext CreateDbContext(string[] args)
{
    var optionsBuilder = new DbContextOptionsBuilder<MarteDbContext>();

    #if DEBUG
    // Desarrollo: DRUAGURTO para crear migraciones
    optionsBuilder.UseSqlServer(@"Server=.\DRUAGURTO;Database=MarteDb;Trusted_Connection=True;TrustServerCertificate=True;");
    #else
    // Producción: LocalDB
    optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=MarteDb;Trusted_Connection=True;");
    #endif

    return new MarteDbContext(optionsBuilder.Options);
}
```

---

## 🚀 Proceso de Instalación

### Usuario Final (Sin conocimientos técnicos)

1. **Descomprimir** `MARTE-Installer.zip`
2. **Ejecutar** `install-marte.ps1` como administrador
3. **Esperar** instalación automática (~2-3 minutos)
4. **Ejecutar MARTE** desde escritorio
5. **Primera vez**: BD se crea automáticamente
6. **Login** con credenciales por defecto

**NO requiere**:
- ❌ Instalar SQL Server manualmente
- ❌ Configurar cadenas de conexión
- ❌ Crear base de datos
- ❌ Aplicar migraciones
- ❌ Ejecutar scripts SQL

---

## 📊 Comparación: Antes vs Ahora

| Aspecto | ANTES (SQL Server) | AHORA (LocalDB) |
|---------|-------------------|-----------------|
| **Instalación** | Manual - 30+ min | Automática - 3 min |
| **Requisitos** | SQL Server Express | Solo .NET 9 |
| **Configuración** | Usuario configura | Automática |
| **Portabilidad** | Baja | Alta |
| **BD ubicación** | Servidor | AppData usuario |
| **Backup** | Complejo | Copiar carpeta Data |
| **Múltiples usuarios** | Sí (en red) | No (standalone) |
| **Ideal para** | Organizaciones | Escuelas pequeñas |

---

## 🔄 Migración de SQL Server a LocalDB

Si ya tienes datos en SQL Server y quieres migrar a LocalDB:

### Opción 1: Backup y Restore

```sql
-- 1. En SQL Server DRUAGURTO - Crear backup
BACKUP DATABASE MarteDb 
TO DISK = 'C:\Temp\MarteDb.bak'
WITH FORMAT, INIT, COMPRESSION;

-- 2. Conectar a LocalDB
sqllocaldb start mssqllocaldb
sqlcmd -S (localdb)\mssqllocaldb

-- 3. Restore en LocalDB
RESTORE DATABASE MarteDb
FROM DISK = 'C:\Temp\MarteDb.bak'
WITH MOVE 'MarteDb' TO 'C:\Users\[Usuario]\AppData\Local\MARTE\Data\MarteDb.mdf',
     MOVE 'MarteDb_log' TO 'C:\Users\[Usuario]\AppData\Local\MARTE\Data\MarteDb_log.ldf';
```

### Opción 2: Export/Import con EF Core

```powershell
# 1. Exportar datos de SQL Server (en modo Debug)
dotnet run --project Marte.WPF -- export-data

# 2. Compilar en Release (LocalDB)
dotnet build -c Release

# 3. Ejecutar app e importar datos
dotnet run --project Marte.WPF -c Release -- import-data
```

---

## 🛠️ Comandos Útiles de LocalDB

### Ver instancias instaladas
```powershell
sqllocaldb info
```

### Ver información de instancia específica
```powershell
sqllocaldb info mssqllocaldb
```

### Crear nueva instancia
```powershell
sqllocaldb create MiInstancia
```

### Iniciar instancia
```powershell
sqllocaldb start mssqllocaldb
```

### Detener instancia
```powershell
sqllocaldb stop mssqllocaldb
```

### Eliminar instancia
```powershell
sqllocaldb delete MiInstancia
```

### Conectar con SSMS
```
Nombre del servidor: (localdb)\mssqllocaldb
Autenticación: Windows Authentication
```

---

## 🔒 Backup y Recuperación

### Backup Manual

```powershell
# 1. Cerrar MARTE
# 2. Copiar carpeta de datos
Copy-Item -Path "$env:LOCALAPPDATA\MARTE\Data" -Destination "C:\Backups\MARTE_$(Get-Date -Format 'yyyyMMdd')" -Recurse
```

### Backup Automático (Script)

```powershell
# backup-marte.ps1
$backupPath = "C:\Backups\MARTE"
$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$sourcePath = "$env:LOCALAPPDATA\MARTE\Data"

if (Test-Path $sourcePath) {
    Copy-Item -Path $sourcePath -Destination "$backupPath\MARTE_$timestamp" -Recurse
    Write-Host "✅ Backup creado: $backupPath\MARTE_$timestamp"
    
    # Limpiar backups antiguos (mantener últimos 7 días)
    Get-ChildItem $backupPath -Directory | 
        Where-Object { $_.CreationTime -lt (Get-Date).AddDays(-7) } | 
        Remove-Item -Recurse -Force
}
```

### Restauración

```powershell
# 1. Cerrar MARTE
# 2. Restaurar desde backup
Copy-Item -Path "C:\Backups\MARTE_20251018\*" -Destination "$env:LOCALAPPDATA\MARTE\Data" -Recurse -Force
# 3. Ejecutar MARTE
```

---

## 🆘 Resolución de Problemas

### Error: "Cannot open database MarteDb"

**Solución**:
```powershell
# Verificar instancia LocalDB
sqllocaldb info mssqllocaldb

# Si no existe, crear e iniciar
sqllocaldb create mssqllocaldb
sqllocaldb start mssqllocaldb
```

---

### Error: "Database file not found"

**Solución**:
```powershell
# Verificar carpeta de datos
Test-Path "$env:LOCALAPPDATA\MARTE\Data"

# Si no existe, crear
New-Item -ItemType Directory -Path "$env:LOCALAPPDATA\MARTE\Data" -Force

# Ejecutar MARTE (creará BD automáticamente)
```

---

### Error: "LocalDB instance API error"

**Solución**:
```powershell
# Reinstalar LocalDB
# 1. Descargar SqlLocalDB.msi
# 2. Ejecutar instalador
msiexec /i SqlLocalDB.msi /qn IACCEPTSQLLOCALDBLICENSETERMS=YES
```

---

## 📈 Rendimiento

### Optimizaciones Aplicadas

```csharp
// Connection pooling habilitado por defecto
Pooling=true;

// Múltiples resultados activos
MultipleActiveResultSets=true;

// Timeout configurado
Connection Timeout=30;
Command Timeout=60;
```

### Tamaño Esperado de BD

| Uso | Tamaño Aproximado |
|-----|-------------------|
| Vacía (con seeder) | ~5 MB |
| 100 asistentes | ~10 MB |
| 1000 asistentes | ~50 MB |
| 10000 asistencias/año | ~100 MB |

---

## 📝 Notas Importantes

⚠️ **LocalDB vs SQL Server Express**

- **LocalDB**: Ideal para aplicaciones standalone (1 usuario por PC)
- **SQL Server Express**: Necesario para trabajo en red (múltiples usuarios)

⚠️ **Limitaciones de LocalDB**

- ✅ Máximo 10 GB por base de datos (suficiente para MARTE)
- ✅ Solo conexiones locales (no red)
- ✅ Se inicia bajo demanda (primer uso puede tardar 2-3 segundos)

⚠️ **Cambiar a SQL Server en Red**

Si posteriormente necesitas trabajo en red, contacta soporte técnico para migración.

---

## 🎯 Checklist de Instalación Exitosa

- [ ] .NET 9 Runtime instalado
- [ ] SQL Server LocalDB instalado
- [ ] Instancia `mssqllocaldb` creada e iniciada
- [ ] Archivos de MARTE en `C:\Program Files\MARTE`
- [ ] Acceso directo en escritorio
- [ ] Primera ejecución completada
- [ ] Base de datos creada en `%LocalAppData%\MARTE\Data`
- [ ] Login exitoso con credenciales por defecto
- [ ] Contraseña cambiada

---

**Última actualización**: 18 de octubre de 2025  
**Mantenido por**: Andy Agurto Urcia - Ing. de Sistemas  
**Versión del documento**: 3.0 (LocalDB Implementation)
