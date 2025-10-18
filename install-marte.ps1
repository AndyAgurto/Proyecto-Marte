# ========================================
# Instalador del Sistema MARTE
# Versión: 1.3.0
# Fecha: Octubre 2025
# ========================================

# Configuración
$ErrorActionPreference = "Stop"
$AppName = "MARTE"
$AppVersion = "1.3.0"
$InstallPath = "C:\Program Files\$AppName"
$DesktopPath = [Environment]::GetFolderPath("Desktop")

# Banner
Clear-Host
Write-Host "╔═══════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║                                                           ║" -ForegroundColor Cyan
Write-Host "║         INSTALADOR DEL SISTEMA MARTE v$AppVersion         ║" -ForegroundColor Cyan
Write-Host "║     Sistema de Gestión de Asistencias                    ║" -ForegroundColor Cyan
Write-Host "║                                                           ║" -ForegroundColor Cyan
Write-Host "╚═══════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host ""

# Verificar permisos de administrador
if (-NOT ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")) {
    Write-Host "❌ Error: Este script requiere permisos de administrador." -ForegroundColor Red
    Write-Host ""
    Write-Host "Por favor:" -ForegroundColor Yellow
    Write-Host "1. Click derecho en install-marte.ps1" -ForegroundColor Yellow
    Write-Host "2. Seleccione 'Ejecutar como administrador'" -ForegroundColor Yellow
    Write-Host ""
    Read-Host "Presione ENTER para salir"
    exit 1
}

Write-Host "✅ Permisos de administrador verificados" -ForegroundColor Green
Write-Host ""

# ========================================
# PASO 1: Verificar .NET 9 Runtime
# ========================================
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkGray
Write-Host "PASO 1: Verificando .NET 9 Runtime..." -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkGray

try {
    $dotnetVersion = dotnet --version 2>$null
    if ($dotnetVersion -like "9.*") {
        Write-Host "✅ .NET 9 Runtime detectado: $dotnetVersion" -ForegroundColor Green
    } else {
        throw "Versión incorrecta de .NET: $dotnetVersion"
    }
} catch {
    Write-Host "⚠️  .NET 9 Runtime no encontrado" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Descargando .NET 9 Runtime Desktop..." -ForegroundColor Cyan
    
    # URL de descarga de .NET 9 Runtime Desktop
    $dotnetUrl = "https://download.visualstudio.microsoft.com/download/pr/907765b0-2bf8-494e-93aa-5ef9553c5d68/a9308dc010617e6716c0e6abd53b05ce/windowsdesktop-runtime-9.0.0-win-x64.exe"
    $dotnetInstaller = "$env:TEMP\dotnet9-runtime.exe"
    
    try {
        Invoke-WebRequest -Uri $dotnetUrl -OutFile $dotnetInstaller
        Write-Host "✅ Descarga completada" -ForegroundColor Green
        
        Write-Host "Instalando .NET 9 Runtime..." -ForegroundColor Cyan
        Start-Process -FilePath $dotnetInstaller -ArgumentList "/quiet", "/norestart" -Wait
        
        Remove-Item $dotnetInstaller -Force
        Write-Host "✅ .NET 9 Runtime instalado correctamente" -ForegroundColor Green
    } catch {
        Write-Host "❌ Error al instalar .NET 9 Runtime" -ForegroundColor Red
        Write-Host "Por favor descargue e instale manualmente desde:" -ForegroundColor Yellow
        Write-Host "https://dotnet.microsoft.com/download/dotnet/9.0" -ForegroundColor White
        Read-Host "Presione ENTER para salir"
        exit 1
    }
}

Write-Host ""

# ========================================
# PASO 2: Verificar/Instalar SQL Server LocalDB
# ========================================
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkGray
Write-Host "PASO 2: Verificando SQL Server LocalDB..." -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkGray

$localDbInstalled = Get-Command sqllocaldb -ErrorAction SilentlyContinue

if ($localDbInstalled) {
    Write-Host "✅ SQL Server LocalDB ya está instalado" -ForegroundColor Green
    
    # Verificar instancia
    $instances = sqllocaldb info
    if ($instances -notcontains "mssqllocaldb") {
        Write-Host "Creando instancia mssqllocaldb..." -ForegroundColor Cyan
        sqllocaldb create mssqllocaldb
        sqllocaldb start mssqllocaldb
        Write-Host "✅ Instancia creada y iniciada" -ForegroundColor Green
    } else {
        Write-Host "✅ Instancia mssqllocaldb ya existe" -ForegroundColor Green
    }
} else {
    Write-Host "⚠️  SQL Server LocalDB no encontrado" -ForegroundColor Yellow
    
    # Verificar si SqlLocalDB.msi existe en la carpeta actual
    if (Test-Path ".\SqlLocalDB.msi") {
        Write-Host "Instalando SQL Server LocalDB desde archivo local..." -ForegroundColor Cyan
        Start-Process msiexec.exe -Wait -ArgumentList "/i", "SqlLocalDB.msi", "/qn", "IACCEPTSQLLOCALDBLICENSETERMS=YES"
    } else {
        Write-Host "Descargando SQL Server LocalDB 2022..." -ForegroundColor Cyan
        
        # URL de descarga de SqlLocalDB 2022
        $sqlLocalDbUrl = "https://download.microsoft.com/download/3/8/d/38de7036-2433-4207-8eae-06e247e17b25/SqlLocalDB.msi"
        $sqlLocalDbInstaller = "$env:TEMP\SqlLocalDB.msi"
        
        try {
            Invoke-WebRequest -Uri $sqlLocalDbUrl -OutFile $sqlLocalDbInstaller
            Write-Host "✅ Descarga completada" -ForegroundColor Green
            
            Write-Host "Instalando SQL Server LocalDB..." -ForegroundColor Cyan
            Start-Process msiexec.exe -Wait -ArgumentList "/i", $sqlLocalDbInstaller, "/qn", "IACCEPTSQLLOCALDBLICENSETERMS=YES"
            
            Remove-Item $sqlLocalDbInstaller -Force
        } catch {
            Write-Host "❌ Error al descargar/instalar SQL Server LocalDB" -ForegroundColor Red
            Write-Host "Por favor descargue manualmente desde:" -ForegroundColor Yellow
            Write-Host "https://learn.microsoft.com/sql/database-engine/configure-windows/sql-server-express-localdb" -ForegroundColor White
            Read-Host "Presione ENTER para salir"
            exit 1
        }
    }
    
    # Crear instancia después de instalación
    Write-Host "Creando instancia de LocalDB..." -ForegroundColor Cyan
    sqllocaldb create mssqllocaldb
    sqllocaldb start mssqllocaldb
    Write-Host "✅ SQL Server LocalDB instalado y configurado" -ForegroundColor Green
}

Write-Host ""

# ========================================
# PASO 3: Copiar archivos de aplicación
# ========================================
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkGray
Write-Host "PASO 3: Instalando archivos de $AppName..." -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkGray

# Crear directorio de instalación
if (Test-Path $InstallPath) {
    Write-Host "⚠️  Instalación existente detectada en $InstallPath" -ForegroundColor Yellow
    $overwrite = Read-Host "¿Desea sobrescribir? (S/N)"
    if ($overwrite -ne "S" -and $overwrite -ne "s") {
        Write-Host "Instalación cancelada por el usuario." -ForegroundColor Yellow
        exit 0
    }
    Remove-Item -Path $InstallPath -Recurse -Force
}

New-Item -ItemType Directory -Path $InstallPath -Force | Out-Null
Write-Host "✅ Directorio creado: $InstallPath" -ForegroundColor Green

# Copiar archivos desde carpeta "Archivos" o "bin/Release"
$sourcePath = ""
if (Test-Path ".\Archivos") {
    $sourcePath = ".\Archivos"
} elseif (Test-Path ".\Marte.WPF\bin\Release\net9.0-windows") {
    $sourcePath = ".\Marte.WPF\bin\Release\net9.0-windows"
} elseif (Test-Path ".\Marte.WPF\bin\Debug\net9.0-windows") {
    Write-Host "⚠️  Solo se encontró build de Debug. Se recomienda compilar en Release." -ForegroundColor Yellow
    $sourcePath = ".\Marte.WPF\bin\Debug\net9.0-windows"
} else {
    Write-Host "❌ Error: No se encontraron archivos de aplicación" -ForegroundColor Red
    Write-Host "Por favor compile el proyecto primero: dotnet publish -c Release" -ForegroundColor Yellow
    Read-Host "Presione ENTER para salir"
    exit 1
}

Write-Host "Copiando archivos desde $sourcePath..." -ForegroundColor Cyan
Copy-Item -Path "$sourcePath\*" -Destination $InstallPath -Recurse -Force
Write-Host "✅ Archivos copiados correctamente" -ForegroundColor Green

Write-Host ""

# ========================================
# PASO 4: Crear acceso directo en escritorio
# ========================================
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkGray
Write-Host "PASO 4: Creando acceso directo..." -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkGray

$WshShell = New-Object -comObject WScript.Shell
$Shortcut = $WshShell.CreateShortcut("$DesktopPath\MARTE.lnk")
$Shortcut.TargetPath = "$InstallPath\Marte.WPF.exe"
$Shortcut.WorkingDirectory = $InstallPath
$Shortcut.IconLocation = "$InstallPath\Marte.WPF.exe,0"
$Shortcut.Description = "Sistema de Gestión de Asistencias MARTE"
$Shortcut.Save()

Write-Host "✅ Acceso directo creado en el escritorio" -ForegroundColor Green

Write-Host ""

# ========================================
# PASO 5: Agregar al menú inicio
# ========================================
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkGray
Write-Host "PASO 5: Agregando al menú inicio..." -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkGray

$StartMenuPath = "$env:ProgramData\Microsoft\Windows\Start Menu\Programs"
$Shortcut = $WshShell.CreateShortcut("$StartMenuPath\MARTE.lnk")
$Shortcut.TargetPath = "$InstallPath\Marte.WPF.exe"
$Shortcut.WorkingDirectory = $InstallPath
$Shortcut.IconLocation = "$InstallPath\Marte.WPF.exe,0"
$Shortcut.Description = "Sistema de Gestión de Asistencias MARTE"
$Shortcut.Save()

Write-Host "✅ Acceso agregado al menú inicio" -ForegroundColor Green

Write-Host ""

# ========================================
# RESUMEN DE INSTALACIÓN
# ========================================
Write-Host "╔═══════════════════════════════════════════════════════════╗" -ForegroundColor Green
Write-Host "║                                                           ║" -ForegroundColor Green
Write-Host "║           ✅ INSTALACIÓN COMPLETADA EXITOSAMENTE           ║" -ForegroundColor Green
Write-Host "║                                                           ║" -ForegroundColor Green
Write-Host "╚═══════════════════════════════════════════════════════════╝" -ForegroundColor Green
Write-Host ""
Write-Host "📍 Ubicación de instalación:" -ForegroundColor Cyan
Write-Host "   $InstallPath" -ForegroundColor White
Write-Host ""
Write-Host "🔐 Credenciales de acceso predeterminadas:" -ForegroundColor Cyan
Write-Host "   Usuario:    druagurto" -ForegroundColor White
Write-Host "   Contraseña: @Druagurto00" -ForegroundColor White
Write-Host ""
Write-Host "⚠️  IMPORTANTE: Cambie la contraseña después del primer inicio de sesión" -ForegroundColor Yellow
Write-Host ""
Write-Host "💾 Base de datos:" -ForegroundColor Cyan
Write-Host "   Se creará automáticamente en la primera ejecución" -ForegroundColor White
Write-Host "   Ubicación: %LocalAppData%\MARTE\Data\MarteDb.mdf" -ForegroundColor White
Write-Host ""
Write-Host "🚀 Para ejecutar MARTE:" -ForegroundColor Cyan
Write-Host "   • Haga doble clic en el icono del escritorio" -ForegroundColor White
Write-Host "   • O búsquelo en el menú inicio" -ForegroundColor White
Write-Host ""
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkGray

Read-Host "Presione ENTER para finalizar"

# Opcional: Ejecutar MARTE automáticamente
$runNow = Read-Host "¿Desea ejecutar MARTE ahora? (S/N)"
if ($runNow -eq "S" -or $runNow -eq "s") {
    Start-Process "$InstallPath\Marte.WPF.exe"
}
