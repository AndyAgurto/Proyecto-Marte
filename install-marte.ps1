# ========================================
# Instalador del Sistema MARTE
# Version: 1.3.0
# Fecha: Octubre 2025
# ========================================

# Configuracion
$ErrorActionPreference = "Stop"
$AppName = "MARTE"
$AppVersion = "1.3.0"
$InstallPath = "C:\Program Files\$AppName"
$DesktopPath = [Environment]::GetFolderPath("Desktop")

# Banner
Clear-Host
Write-Host "========================================" -ForegroundColor Cyan
Write-Host " INSTALADOR DEL SISTEMA MARTE v$AppVersion " -ForegroundColor Cyan
Write-Host " Sistema de Gestion de Asistencias     " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verificar permisos de administrador
if (-NOT ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")) {
    Write-Host "[ERROR] Este script requiere permisos de administrador." -ForegroundColor Red
    Write-Host ""
    Write-Host "Instrucciones:" -ForegroundColor Yellow
    Write-Host "1. Click derecho en install-marte.ps1" -ForegroundColor Yellow
    Write-Host "2. Seleccione 'Ejecutar como administrador'" -ForegroundColor Yellow
    Write-Host ""
    Read-Host "Presione ENTER para salir"
    exit 1
}

Write-Host "[OK] Permisos de administrador verificados" -ForegroundColor Green
Write-Host ""

# ========================================
# PASO 1: Verificar .NET 9 Runtime
# ========================================
Write-Host "----------------------------------------" -ForegroundColor DarkGray
Write-Host "PASO 1: Verificando .NET 9 Runtime..." -ForegroundColor Cyan
Write-Host "----------------------------------------" -ForegroundColor DarkGray

try {
    $dotnetVersion = dotnet --version 2>$null
    if ($dotnetVersion -like "9.*") {
        Write-Host "[OK] .NET 9 Runtime detectado: $dotnetVersion" -ForegroundColor Green
    } else {
        throw "Version incorrecta de .NET"
    }
} catch {
    Write-Host "[AVISO] .NET 9 Runtime no encontrado" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Descargando .NET 9 Runtime Desktop..." -ForegroundColor Cyan
    
    # URL de descarga de .NET 9 Runtime Desktop
    $dotnetUrl = "https://download.visualstudio.microsoft.com/download/pr/3aa4e942-42cd-4bf5-afe7-fc23bd9c69c5/64da54c8864e473c19a7d3de15790418/windowsdesktop-runtime-9.0.0-win-x64.exe"
    $dotnetInstaller = "$env:TEMP\dotnet9-desktop-runtime.exe"
    
    try {
        Invoke-WebRequest -Uri $dotnetUrl -OutFile $dotnetInstaller
        Write-Host "[OK] Descarga completada" -ForegroundColor Green
        Write-Host ""
        Write-Host "Instalando .NET 9 Runtime..." -ForegroundColor Cyan
        Start-Process -FilePath $dotnetInstaller -ArgumentList "/install /quiet /norestart" -Wait
        if ($LASTEXITCODE -eq 0) {
            Write-Host "[OK] .NET 9 Runtime instalado correctamente" -ForegroundColor Green
        } else {
            Write-Host "[ERROR] Error al instalar .NET 9 Runtime" -ForegroundColor Red
            Write-Host "Por favor, instale manualmente desde: https://dotnet.microsoft.com/download/dotnet/9.0" -ForegroundColor Yellow
            Read-Host "Presione ENTER para salir"
            exit 1
        }
    } catch {
        Write-Host "[ERROR] No se pudo descargar .NET 9 Runtime" -ForegroundColor Red
        Write-Host "Por favor, instale manualmente desde: https://dotnet.microsoft.com/download/dotnet/9.0" -ForegroundColor Yellow
        Read-Host "Presione ENTER para salir"
        exit 1
    }
}

Write-Host ""

# ========================================
# PASO 2: Verificar/Instalar SQL Server LocalDB
# ========================================
Write-Host "----------------------------------------" -ForegroundColor DarkGray
Write-Host "PASO 2: Verificando SQL Server LocalDB..." -ForegroundColor Cyan
Write-Host "----------------------------------------" -ForegroundColor DarkGray

$localDbInstalled = Get-Command sqllocaldb -ErrorAction SilentlyContinue

if ($localDbInstalled) {
    Write-Host "[OK] SQL Server LocalDB ya esta instalado" -ForegroundColor Green
    
    # Verificar si la instancia existe
    $instances = sqllocaldb info
    if ($instances -contains "mssqllocaldb") {
        sqllocaldb start mssqllocaldb 2>$null
        Write-Host "[OK] Instancia creada y iniciada" -ForegroundColor Green
    } else {
        Write-Host "[OK] Instancia mssqllocaldb ya existe" -ForegroundColor Green
    }
} else {
    Write-Host "[AVISO] SQL Server LocalDB no encontrado" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Descargando SQL Server LocalDB 2022..." -ForegroundColor Cyan
    
    $localDbUrl = "https://download.microsoft.com/download/3/8/d/38de7036-2433-4207-8eae-06e247e17b25/SqlLocalDB.msi"
    $localDbInstaller = "$env:TEMP\SqlLocalDB.msi"
    
    try {
        Invoke-WebRequest -Uri $localDbUrl -OutFile $localDbInstaller
        if (Test-Path $localDbInstaller) {
            Write-Host "[OK] Descarga completada" -ForegroundColor Green
            Write-Host ""
            Write-Host "Instalando SQL Server LocalDB..." -ForegroundColor Cyan
            Write-Host "(Este proceso puede tomar varios minutos)" -ForegroundColor Yellow
            Start-Process msiexec.exe -ArgumentList "/i `"$localDbInstaller`" /qn IACCEPTSQLLOCALDBLICENSETERMS=YES" -Wait
            Write-Host "[OK] SQL Server LocalDB instalado" -ForegroundColor Green
        } else {
            Write-Host "[ERROR] Error al descargar/instalar SQL Server LocalDB" -ForegroundColor Red
            Write-Host "Por favor, instale manualmente desde:" -ForegroundColor Yellow
            Write-Host "https://learn.microsoft.com/sql/database-engine/configure-windows/sql-server-express-localdb" -ForegroundColor Yellow
            Read-Host "Presione ENTER para continuar"
        }
    } catch {
        Write-Host "[ERROR] Error al descargar SQL Server LocalDB" -ForegroundColor Red
        Write-Host "Por favor, instale manualmente" -ForegroundColor Yellow
        Read-Host "Presione ENTER para continuar"
    }
    
    # Verificar instalacion
    $localDbInstalled = Get-Command sqllocaldb -ErrorAction SilentlyContinue
    if ($localDbInstalled) {
        Write-Host "[OK] SQL Server LocalDB instalado y configurado" -ForegroundColor Green
    } else {
        Write-Host "[AVISO] SQL Server LocalDB no se pudo verificar" -ForegroundColor Yellow
        Write-Host "La aplicacion lo intentara instalar en el primer inicio" -ForegroundColor Yellow
    }
}

Write-Host ""

# ========================================
# PASO 3: Copiar archivos de aplicacion
# ========================================
Write-Host "----------------------------------------" -ForegroundColor DarkGray
Write-Host "PASO 3: Instalando archivos de $AppName..." -ForegroundColor Cyan
Write-Host "----------------------------------------" -ForegroundColor DarkGray

# Crear directorio de instalacion
if (Test-Path $InstallPath) {
    Write-Host "[AVISO] Instalacion existente detectada en $InstallPath" -ForegroundColor Yellow
    $overwrite = Read-Host "Desea sobrescribir la instalacion existente? (S/N)"
    if ($overwrite -ne "S" -and $overwrite -ne "s") {
        Write-Host "Instalacion cancelada por el usuario" -ForegroundColor Yellow
        Read-Host "Presione ENTER para salir"
        exit 0
    }
}

New-Item -ItemType Directory -Path $InstallPath -Force | Out-Null
Write-Host "[OK] Directorio creado: $InstallPath" -ForegroundColor Green

# Copiar archivos desde carpeta "Archivos" o "bin/Release"
$sourcePath = ""
if (Test-Path ".\Archivos") {
    $sourcePath = ".\Archivos"
} elseif (Test-Path ".\Marte.WPF\bin\Release\net9.0-windows") {
    $sourcePath = ".\Marte.WPF\bin\Release\net9.0-windows"
} elseif (Test-Path ".\Marte.WPF\bin\Debug\net9.0-windows") {
    Write-Host "[AVISO] Solo se encontro build de Debug. Se recomienda compilar en Release." -ForegroundColor Yellow
    $sourcePath = ".\Marte.WPF\bin\Debug\net9.0-windows"
} else {
    Write-Host "[ERROR] No se encontraron archivos de aplicacion" -ForegroundColor Red
    Read-Host "Presione ENTER para salir"
    exit 1
}

Write-Host "Copiando archivos desde $sourcePath..." -ForegroundColor Cyan
Copy-Item -Path "$sourcePath\*" -Destination $InstallPath -Recurse -Force
Write-Host "[OK] Archivos copiados correctamente" -ForegroundColor Green

Write-Host ""

# ========================================
# PASO 4: Crear acceso directo en escritorio
# ========================================
Write-Host "----------------------------------------" -ForegroundColor DarkGray
Write-Host "PASO 4: Creando acceso directo..." -ForegroundColor Cyan
Write-Host "----------------------------------------" -ForegroundColor DarkGray

$WshShell = New-Object -comObject WScript.Shell
$Shortcut = $WshShell.CreateShortcut("$DesktopPath\MARTE.lnk")
$Shortcut.TargetPath = "$InstallPath\Marte.WPF.exe"
$Shortcut.WorkingDirectory = $InstallPath
$Shortcut.IconLocation = "$InstallPath\Marte.WPF.exe,0"
$Shortcut.Description = "Sistema de Gestion de Asistencias MARTE"
$Shortcut.Save()

Write-Host "[OK] Acceso directo creado en el escritorio" -ForegroundColor Green

Write-Host ""

# ========================================
# PASO 5: Agregar al menu inicio
# ========================================
Write-Host "----------------------------------------" -ForegroundColor DarkGray
Write-Host "PASO 5: Agregando al menu inicio..." -ForegroundColor Cyan
Write-Host "----------------------------------------" -ForegroundColor DarkGray

$StartMenuPath = "$env:ProgramData\Microsoft\Windows\Start Menu\Programs"
$Shortcut = $WshShell.CreateShortcut("$StartMenuPath\MARTE.lnk")
$Shortcut.TargetPath = "$InstallPath\Marte.WPF.exe"
$Shortcut.WorkingDirectory = $InstallPath
$Shortcut.IconLocation = "$InstallPath\Marte.WPF.exe,0"
$Shortcut.Description = "Sistema de Gestion de Asistencias MARTE"
$Shortcut.Save()

Write-Host "[OK] Acceso agregado al menu inicio" -ForegroundColor Green

Write-Host ""

# ========================================
# RESUMEN DE INSTALACION
# ========================================
Write-Host "========================================" -ForegroundColor Green
Write-Host " INSTALACION COMPLETADA EXITOSAMENTE   " -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Ubicacion de instalacion:" -ForegroundColor Cyan
Write-Host "  $InstallPath" -ForegroundColor White
Write-Host ""
Write-Host "Credenciales de acceso predeterminadas:" -ForegroundColor Cyan
Write-Host "  Usuario:    druagurto" -ForegroundColor White
Write-Host "  Contrasena: @Druagurto00" -ForegroundColor White
Write-Host ""
Write-Host "[IMPORTANTE] Cambie la contrasena despues del primer inicio de sesion" -ForegroundColor Yellow
Write-Host ""
Write-Host "Base de datos:" -ForegroundColor Cyan
Write-Host "  Se creara automaticamente en la primera ejecucion" -ForegroundColor White
Write-Host "  Ubicacion: %LocalAppData%\MARTE\Data\MarteDb.mdf" -ForegroundColor White
Write-Host ""
Write-Host "Para ejecutar MARTE:" -ForegroundColor Cyan
Write-Host "  - Haga doble clic en el icono del escritorio" -ForegroundColor White
Write-Host "  - O busquelo en el menu inicio" -ForegroundColor White
Write-Host ""
Write-Host "----------------------------------------" -ForegroundColor DarkGray

Read-Host "Presione ENTER para finalizar"

# Opcional: Ejecutar MARTE automaticamente
$runNow = Read-Host "Desea ejecutar MARTE ahora? (S/N)"
if ($runNow -eq "S" -or $runNow -eq "s") {
    Start-Process "$InstallPath\Marte.WPF.exe"
}
