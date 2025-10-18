# ========================================
# Desinstalador del Sistema MARTE
# Version: 1.3.0
# Fecha: Octubre 2025
# ========================================

# Configuracion
$ErrorActionPreference = "Stop"
$AppName = "MARTE"
$InstallPath = "C:\Program Files\$AppName"
$AppDataPath = "$env:LOCALAPPDATA\$AppName"
$DesktopPath = [Environment]::GetFolderPath("Desktop")
$StartMenuPath = "$env:ProgramData\Microsoft\Windows\Start Menu\Programs"

# Banner
Clear-Host
Write-Host "========================================" -ForegroundColor Red
Write-Host "                                        " -ForegroundColor Red
Write-Host " DESINSTALADOR DEL SISTEMA MARTE v1.3.0" -ForegroundColor Red
Write-Host "   Sistema de Gestion de Asistencias   " -ForegroundColor Red
Write-Host "                                        " -ForegroundColor Red
Write-Host "========================================" -ForegroundColor Red
Write-Host ""

# Verificar permisos de administrador
if (-NOT ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")) {
    Write-Host "[ERROR] Este script requiere permisos de administrador." -ForegroundColor Red
    Write-Host ""
    Write-Host "Por favor:" -ForegroundColor Yellow
    Write-Host "1. Click derecho en uninstall-marte.ps1" -ForegroundColor Yellow
    Write-Host "2. Seleccione 'Ejecutar como administrador'" -ForegroundColor Yellow
    Write-Host ""
    Read-Host "Presione ENTER para salir"
    exit 1
}

Write-Host "[OK] Permisos de administrador verificados" -ForegroundColor Green
Write-Host ""

# ========================================
# ADVERTENCIA Y CONFIRMACION
# ========================================
Write-Host "[ADVERTENCIA] ACCION IRREVERSIBLE" -ForegroundColor Yellow -BackgroundColor DarkRed
Write-Host ""
Write-Host "Esta accion eliminara:" -ForegroundColor Yellow
Write-Host "  - Archivos de la aplicacion MARTE" -ForegroundColor White
Write-Host "  - Accesos directos (Escritorio y Menu Inicio)" -ForegroundColor White
Write-Host ""
Write-Host "NO se eliminara automaticamente:" -ForegroundColor Cyan
Write-Host "  - Base de datos (para preservar sus datos)" -ForegroundColor White
Write-Host "  - SQL Server LocalDB (puede ser usado por otras apps)" -ForegroundColor White
Write-Host ""

$confirm = Read-Host "Esta seguro que desea desinstalar MARTE? (S/N)"

if ($confirm -ne "S" -and $confirm -ne "s") {
    Write-Host ""
    Write-Host "[OK] Desinstalacion cancelada por el usuario." -ForegroundColor Green
    Read-Host "Presione ENTER para salir"
    exit 0
}

Write-Host ""

# ========================================
# PASO 1: Cerrar aplicacion si esta en ejecucion
# ========================================
Write-Host "========================================" -ForegroundColor DarkGray
Write-Host "PASO 1: Verificando procesos en ejecucion..." -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor DarkGray

$marteProcess = Get-Process -Name "Marte.WPF" -ErrorAction SilentlyContinue

if ($marteProcess) {
    Write-Host "[AVISO] MARTE esta en ejecucion" -ForegroundColor Yellow
    Write-Host "Cerrando aplicacion..." -ForegroundColor Cyan
    
    try {
        $marteProcess | Stop-Process -Force
        Start-Sleep -Seconds 2
        Write-Host "[OK] Aplicacion cerrada correctamente" -ForegroundColor Green
    } catch {
        Write-Host "[ERROR] Error al cerrar la aplicacion" -ForegroundColor Red
        Write-Host "Por favor cierre MARTE manualmente antes de continuar" -ForegroundColor Yellow
        Read-Host "Presione ENTER cuando haya cerrado MARTE"
    }
} else {
    Write-Host "[OK] MARTE no esta en ejecucion" -ForegroundColor Green
}

Write-Host ""

# ========================================
# PASO 2: Backup de base de datos (opcional)
# ========================================
Write-Host "========================================" -ForegroundColor DarkGray
Write-Host "PASO 2: Backup de base de datos..." -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor DarkGray

if (Test-Path "$AppDataPath\Data") {
    Write-Host "[AVISO] Se detecto una base de datos en:" -ForegroundColor Yellow
    Write-Host "   $AppDataPath\Data" -ForegroundColor White
    Write-Host ""
    
    $backup = Read-Host "Desea crear un backup antes de desinstalar? (S/N)"
    
    if ($backup -eq "S" -or $backup -eq "s") {
        $backupPath = "$env:USERPROFILE\Desktop\MARTE_Backup_$(Get-Date -Format 'yyyyMMdd_HHmmss')"
        
        try {
            Write-Host "Creando backup en: $backupPath" -ForegroundColor Cyan
            Copy-Item -Path "$AppDataPath\Data" -Destination $backupPath -Recurse -Force
            Write-Host "[OK] Backup creado exitosamente" -ForegroundColor Green
            Write-Host "   Ubicacion: $backupPath" -ForegroundColor White
            
            # Crear archivo de instrucciones
            $instructions = @"
========================================
     BACKUP DE BASE DE DATOS MARTE
========================================

Fecha de backup: $(Get-Date -Format "dd/MM/yyyy HH:mm:ss")
Version de MARTE: 1.3.0

CONTENIDO:
  - MarteDb.mdf     - Archivo de base de datos
  - MarteDb_log.ldf - Archivo de log transaccional

PARA RESTAURAR:
1. Reinstalar MARTE (ejecutar install-marte.ps1)
2. Cerrar MARTE si esta abierto
3. Copiar los archivos de este backup a:
   %LocalAppData%\MARTE\Data\
4. Ejecutar MARTE

IMPORTANTE:
  - Guardar este backup en un lugar seguro
  - No modificar los archivos .mdf o .ldf
  - Para multiples backups, conservar este directorio completo

SOPORTE:
  - Si necesita ayuda, contacte soporte tecnico
  - Incluya este archivo README.txt en su consulta

"@
            $instructions | Out-File "$backupPath\README.txt" -Encoding UTF8
            
        } catch {
            Write-Host "[ERROR] Error al crear backup: $($_.Exception.Message)" -ForegroundColor Red
            $continueWithoutBackup = Read-Host "Desea continuar sin backup? (S/N)"
            if ($continueWithoutBackup -ne "S" -and $continueWithoutBackup -ne "s") {
                Write-Host "Desinstalacion cancelada." -ForegroundColor Yellow
                exit 0
            }
        }
    } else {
        Write-Host "[INFO] Backup omitido" -ForegroundColor Yellow
    }
} else {
    Write-Host "[INFO] No se encontro base de datos para backup" -ForegroundColor Gray
}

Write-Host ""

# ========================================
# PASO 3: Eliminar archivos de aplicacion
# ========================================
Write-Host "========================================" -ForegroundColor DarkGray
Write-Host "PASO 3: Eliminando archivos de aplicacion..." -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor DarkGray

if (Test-Path $InstallPath) {
    try {
        Write-Host "Eliminando: $InstallPath" -ForegroundColor Cyan
        Remove-Item -Path $InstallPath -Recurse -Force
        Write-Host "[OK] Archivos de aplicacion eliminados" -ForegroundColor Green
    } catch {
        Write-Host "[ERROR] Error al eliminar archivos: $($_.Exception.Message)" -ForegroundColor Red
    }
} else {
    Write-Host "[INFO] No se encontraron archivos de aplicacion" -ForegroundColor Gray
}

Write-Host ""

# ========================================
# PASO 4: Eliminar accesos directos
# ========================================
Write-Host "========================================" -ForegroundColor DarkGray
Write-Host "PASO 4: Eliminando accesos directos..." -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor DarkGray

# Acceso directo del escritorio
$desktopShortcut = "$DesktopPath\MARTE.lnk"
if (Test-Path $desktopShortcut) {
    Remove-Item $desktopShortcut -Force
    Write-Host "[OK] Acceso directo del escritorio eliminado" -ForegroundColor Green
} else {
    Write-Host "[INFO] No se encontro acceso directo en escritorio" -ForegroundColor Gray
}

# Acceso directo del menu inicio
$startMenuShortcut = "$StartMenuPath\MARTE.lnk"
if (Test-Path $startMenuShortcut) {
    Remove-Item $startMenuShortcut -Force
    Write-Host "[OK] Acceso directo del menu inicio eliminado" -ForegroundColor Green
} else {
    Write-Host "[INFO] No se encontro acceso directo en menu inicio" -ForegroundColor Gray
}

Write-Host ""

# ========================================
# PASO 5: Eliminar datos de aplicacion (opcional)
# ========================================
Write-Host "========================================" -ForegroundColor DarkGray
Write-Host "PASO 5: Eliminando datos de aplicacion..." -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor DarkGray

if (Test-Path $AppDataPath) {
    Write-Host "[AVISO] Se encontraron datos de aplicacion (incluye base de datos)" -ForegroundColor Yellow
    Write-Host "   Ubicacion: $AppDataPath" -ForegroundColor White
    Write-Host ""
    Write-Host "   ADVERTENCIA: Si elimina estos datos, perdera:" -ForegroundColor Red
    Write-Host "   - Todos los asistentes registrados" -ForegroundColor White
    Write-Host "   - Historial de asistencias" -ForegroundColor White
    Write-Host "   - Usuarios y configuraciones" -ForegroundColor White
    Write-Host ""
    
    $deleteData = Read-Host "Desea eliminar la base de datos y configuraciones? (S/N)"
    
    if ($deleteData -eq "S" -or $deleteData -eq "s") {
        try {
            Remove-Item -Path $AppDataPath -Recurse -Force
            Write-Host "[OK] Datos de aplicacion eliminados" -ForegroundColor Green
        } catch {
            Write-Host "[ERROR] Error al eliminar datos: $($_.Exception.Message)" -ForegroundColor Red
        }
    } else {
        Write-Host "[INFO] Datos de aplicacion conservados" -ForegroundColor Cyan
        Write-Host "   Puede eliminarlos manualmente desde:" -ForegroundColor White
        Write-Host "   $AppDataPath" -ForegroundColor White
    }
} else {
    Write-Host "[INFO] No se encontraron datos de aplicacion" -ForegroundColor Gray
}

Write-Host ""

# ========================================
# PASO 6: Desinstalar SQL Server LocalDB (opcional)
# ========================================
Write-Host "========================================" -ForegroundColor DarkGray
Write-Host "PASO 6: SQL Server LocalDB..." -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor DarkGray

$localDbInstalled = Get-Command sqllocaldb -ErrorAction SilentlyContinue

if ($localDbInstalled) {
    Write-Host "[AVISO] SQL Server LocalDB esta instalado" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "   NOTA: LocalDB puede ser usado por otras aplicaciones." -ForegroundColor Cyan
    Write-Host "   No se recomienda desinstalarlo a menos que este seguro." -ForegroundColor Cyan
    Write-Host ""
    
    $uninstallLocalDb = Read-Host "Desea desinstalar SQL Server LocalDB? (S/N)"
    
    if ($uninstallLocalDb -eq "S" -or $uninstallLocalDb -eq "s") {
        Write-Host ""
        Write-Host "Para desinstalar SQL Server LocalDB:" -ForegroundColor Yellow
        Write-Host "1. Abra Panel de Control" -ForegroundColor White
        Write-Host "2. Vaya a 'Programas y caracteristicas'" -ForegroundColor White
        Write-Host "3. Busque 'Microsoft SQL Server LocalDB'" -ForegroundColor White
        Write-Host "4. Click derecho -> Desinstalar" -ForegroundColor White
        Write-Host ""
        Write-Host "O ejecute el siguiente comando:" -ForegroundColor Yellow
        Write-Host "msiexec /x {GUID-DE-LOCALDB} /qn" -ForegroundColor White
        Write-Host ""
        
        $openControlPanel = Read-Host "Desea abrir Panel de Control ahora? (S/N)"
        if ($openControlPanel -eq "S" -or $openControlPanel -eq "s") {
            Start-Process "control" "appwiz.cpl"
        }
    } else {
        Write-Host "[INFO] SQL Server LocalDB conservado" -ForegroundColor Cyan
    }
} else {
    Write-Host "[INFO] SQL Server LocalDB no esta instalado" -ForegroundColor Gray
}

Write-Host ""

# ========================================
# RESUMEN DE DESINSTALACION
# ========================================
Write-Host "========================================" -ForegroundColor Green
Write-Host "                                        " -ForegroundColor Green
Write-Host "    DESINSTALACION COMPLETADA           " -ForegroundColor Green
Write-Host "                                        " -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Resumen de acciones realizadas:" -ForegroundColor Cyan
Write-Host ""

if (!(Test-Path $InstallPath)) {
    Write-Host "   [OK] Archivos de aplicacion eliminados" -ForegroundColor Green
} else {
    Write-Host "   [AVISO] Archivos de aplicacion NO eliminados" -ForegroundColor Yellow
}

if (!(Test-Path "$DesktopPath\MARTE.lnk")) {
    Write-Host "   [OK] Acceso directo del escritorio eliminado" -ForegroundColor Green
} else {
    Write-Host "   [AVISO] Acceso directo del escritorio NO eliminado" -ForegroundColor Yellow
}

if (!(Test-Path "$StartMenuPath\MARTE.lnk")) {
    Write-Host "   [OK] Acceso directo del menu inicio eliminado" -ForegroundColor Green
} else {
    Write-Host "   [AVISO] Acceso directo del menu inicio NO eliminado" -ForegroundColor Yellow
}

if (Test-Path $AppDataPath) {
    Write-Host "   [INFO] Datos de aplicacion conservados en:" -ForegroundColor Cyan
    Write-Host "      $AppDataPath" -ForegroundColor White
} else {
    Write-Host "   [OK] Datos de aplicacion eliminados" -ForegroundColor Green
}

Write-Host ""

if (Test-Path "$env:USERPROFILE\Desktop\MARTE_Backup_*") {
    $backupFolders = Get-ChildItem "$env:USERPROFILE\Desktop" -Directory -Filter "MARTE_Backup_*" | Sort-Object CreationTime -Descending | Select-Object -First 1
    Write-Host "Base de datos: Backup creado en:" -ForegroundColor Cyan
    Write-Host "   $($backupFolders.FullName)" -ForegroundColor White
    Write-Host ""
}

Write-Host "========================================" -ForegroundColor DarkGray
Write-Host ""
Write-Host "Gracias por usar Sistema MARTE" -ForegroundColor Cyan
Write-Host ""

Read-Host "Presione ENTER para finalizar"
