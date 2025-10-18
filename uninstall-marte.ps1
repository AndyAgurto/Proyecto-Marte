# ========================================
# Desinstalador del Sistema MARTE
# Versión: 1.3.0
# Fecha: Octubre 2025
# ========================================

# Configuración
$ErrorActionPreference = "Stop"
$AppName = "MARTE"
$InstallPath = "C:\Program Files\$AppName"
$AppDataPath = "$env:LOCALAPPDATA\$AppName"
$DesktopPath = [Environment]::GetFolderPath("Desktop")
$StartMenuPath = "$env:ProgramData\Microsoft\Windows\Start Menu\Programs"

# Banner
Clear-Host
Write-Host "╔═══════════════════════════════════════════════════════════╗" -ForegroundColor Red
Write-Host "║                                                           ║" -ForegroundColor Red
Write-Host "║        DESINSTALADOR DEL SISTEMA MARTE v1.3.0            ║" -ForegroundColor Red
Write-Host "║     Sistema de Gestión de Asistencias                    ║" -ForegroundColor Red
Write-Host "║                                                           ║" -ForegroundColor Red
Write-Host "╚═══════════════════════════════════════════════════════════╝" -ForegroundColor Red
Write-Host ""

# Verificar permisos de administrador
if (-NOT ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")) {
    Write-Host "❌ Error: Este script requiere permisos de administrador." -ForegroundColor Red
    Write-Host ""
    Write-Host "Por favor:" -ForegroundColor Yellow
    Write-Host "1. Click derecho en uninstall-marte.ps1" -ForegroundColor Yellow
    Write-Host "2. Seleccione 'Ejecutar como administrador'" -ForegroundColor Yellow
    Write-Host ""
    Read-Host "Presione ENTER para salir"
    exit 1
}

Write-Host "✅ Permisos de administrador verificados" -ForegroundColor Green
Write-Host ""

# ========================================
# ADVERTENCIA Y CONFIRMACIÓN
# ========================================
Write-Host "⚠️  ADVERTENCIA" -ForegroundColor Yellow -BackgroundColor DarkRed
Write-Host ""
Write-Host "Esta acción eliminará:" -ForegroundColor Yellow
Write-Host "  • Archivos de la aplicación MARTE" -ForegroundColor White
Write-Host "  • Accesos directos (Escritorio y Menú Inicio)" -ForegroundColor White
Write-Host ""
Write-Host "NO se eliminará automáticamente:" -ForegroundColor Cyan
Write-Host "  • Base de datos (para preservar sus datos)" -ForegroundColor White
Write-Host "  • SQL Server LocalDB (puede ser usado por otras apps)" -ForegroundColor White
Write-Host ""

$confirm = Read-Host "¿Está seguro que desea desinstalar MARTE? (S/N)"

if ($confirm -ne "S" -and $confirm -ne "s") {
    Write-Host ""
    Write-Host "✅ Desinstalación cancelada por el usuario." -ForegroundColor Green
    Read-Host "Presione ENTER para salir"
    exit 0
}

Write-Host ""

# ========================================
# PASO 1: Cerrar aplicación si está en ejecución
# ========================================
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkGray
Write-Host "PASO 1: Verificando procesos en ejecución..." -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkGray

$marteProcess = Get-Process -Name "Marte.WPF" -ErrorAction SilentlyContinue

if ($marteProcess) {
    Write-Host "⚠️  MARTE está en ejecución" -ForegroundColor Yellow
    Write-Host "Cerrando aplicación..." -ForegroundColor Cyan
    
    try {
        $marteProcess | Stop-Process -Force
        Start-Sleep -Seconds 2
        Write-Host "✅ Aplicación cerrada correctamente" -ForegroundColor Green
    } catch {
        Write-Host "❌ Error al cerrar la aplicación" -ForegroundColor Red
        Write-Host "Por favor cierre MARTE manualmente antes de continuar" -ForegroundColor Yellow
        Read-Host "Presione ENTER cuando haya cerrado MARTE"
    }
} else {
    Write-Host "✅ MARTE no está en ejecución" -ForegroundColor Green
}

Write-Host ""

# ========================================
# PASO 2: Backup de base de datos (opcional)
# ========================================
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkGray
Write-Host "PASO 2: Backup de base de datos..." -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkGray

if (Test-Path "$AppDataPath\Data") {
    Write-Host "⚠️  Se detectó una base de datos en:" -ForegroundColor Yellow
    Write-Host "   $AppDataPath\Data" -ForegroundColor White
    Write-Host ""
    
    $backup = Read-Host "¿Desea crear un backup antes de desinstalar? (S/N)"
    
    if ($backup -eq "S" -or $backup -eq "s") {
        $backupPath = "$env:USERPROFILE\Desktop\MARTE_Backup_$(Get-Date -Format 'yyyyMMdd_HHmmss')"
        
        try {
            Write-Host "Creando backup en: $backupPath" -ForegroundColor Cyan
            Copy-Item -Path "$AppDataPath\Data" -Destination $backupPath -Recurse -Force
            Write-Host "✅ Backup creado exitosamente" -ForegroundColor Green
            Write-Host "   Ubicación: $backupPath" -ForegroundColor White
            
            # Crear archivo de instrucciones
            $instructions = @"
╔═══════════════════════════════════════════════════════════╗
║           BACKUP DE BASE DE DATOS MARTE                   ║
╚═══════════════════════════════════════════════════════════╝

Fecha de backup: $(Get-Date -Format "dd/MM/yyyy HH:mm:ss")
Versión de MARTE: 1.3.0

CONTENIDO:
  • MarteDb.mdf     - Archivo de base de datos
  • MarteDb_log.ldf - Archivo de log transaccional

PARA RESTAURAR:
1. Reinstalar MARTE (ejecutar install-marte.ps1)
2. Cerrar MARTE si está abierto
3. Copiar los archivos de este backup a:
   %LocalAppData%\MARTE\Data\
4. Ejecutar MARTE

IMPORTANTE:
  • Guardar este backup en un lugar seguro
  • No modificar los archivos .mdf o .ldf
  • Para múltiples backups, conservar este directorio completo

SOPORTE:
  • Si necesita ayuda, contacte soporte técnico
  • Incluya este archivo README.txt en su consulta

"@
            $instructions | Out-File "$backupPath\README.txt" -Encoding UTF8
            
        } catch {
            Write-Host "❌ Error al crear backup: $($_.Exception.Message)" -ForegroundColor Red
            $continueWithoutBackup = Read-Host "¿Desea continuar sin backup? (S/N)"
            if ($continueWithoutBackup -ne "S" -and $continueWithoutBackup -ne "s") {
                Write-Host "Desinstalación cancelada." -ForegroundColor Yellow
                exit 0
            }
        }
    } else {
        Write-Host "⏭️  Backup omitido" -ForegroundColor Yellow
    }
} else {
    Write-Host "ℹ️  No se encontró base de datos para backup" -ForegroundColor Gray
}

Write-Host ""

# ========================================
# PASO 3: Eliminar archivos de aplicación
# ========================================
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkGray
Write-Host "PASO 3: Eliminando archivos de aplicación..." -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkGray

if (Test-Path $InstallPath) {
    try {
        Write-Host "Eliminando: $InstallPath" -ForegroundColor Cyan
        Remove-Item -Path $InstallPath -Recurse -Force
        Write-Host "✅ Archivos de aplicación eliminados" -ForegroundColor Green
    } catch {
        Write-Host "❌ Error al eliminar archivos: $($_.Exception.Message)" -ForegroundColor Red
    }
} else {
    Write-Host "ℹ️  No se encontraron archivos de aplicación" -ForegroundColor Gray
}

Write-Host ""

# ========================================
# PASO 4: Eliminar accesos directos
# ========================================
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkGray
Write-Host "PASO 4: Eliminando accesos directos..." -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkGray

# Acceso directo del escritorio
$desktopShortcut = "$DesktopPath\MARTE.lnk"
if (Test-Path $desktopShortcut) {
    Remove-Item $desktopShortcut -Force
    Write-Host "✅ Acceso directo del escritorio eliminado" -ForegroundColor Green
} else {
    Write-Host "ℹ️  No se encontró acceso directo en escritorio" -ForegroundColor Gray
}

# Acceso directo del menú inicio
$startMenuShortcut = "$StartMenuPath\MARTE.lnk"
if (Test-Path $startMenuShortcut) {
    Remove-Item $startMenuShortcut -Force
    Write-Host "✅ Acceso directo del menú inicio eliminado" -ForegroundColor Green
} else {
    Write-Host "ℹ️  No se encontró acceso directo en menú inicio" -ForegroundColor Gray
}

Write-Host ""

# ========================================
# PASO 5: Eliminar datos de aplicación (opcional)
# ========================================
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkGray
Write-Host "PASO 5: Eliminando datos de aplicación..." -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkGray

if (Test-Path $AppDataPath) {
    Write-Host "⚠️  Se encontraron datos de aplicación (incluye base de datos)" -ForegroundColor Yellow
    Write-Host "   Ubicación: $AppDataPath" -ForegroundColor White
    Write-Host ""
    Write-Host "   ADVERTENCIA: Si elimina estos datos, perderá:" -ForegroundColor Red
    Write-Host "   • Todos los asistentes registrados" -ForegroundColor White
    Write-Host "   • Historial de asistencias" -ForegroundColor White
    Write-Host "   • Usuarios y configuraciones" -ForegroundColor White
    Write-Host ""
    
    $deleteData = Read-Host "¿Desea eliminar la base de datos y configuraciones? (S/N)"
    
    if ($deleteData -eq "S" -or $deleteData -eq "s") {
        try {
            Remove-Item -Path $AppDataPath -Recurse -Force
            Write-Host "✅ Datos de aplicación eliminados" -ForegroundColor Green
        } catch {
            Write-Host "❌ Error al eliminar datos: $($_.Exception.Message)" -ForegroundColor Red
        }
    } else {
        Write-Host "⏭️  Datos de aplicación conservados" -ForegroundColor Cyan
        Write-Host "   Puede eliminarlos manualmente desde:" -ForegroundColor White
        Write-Host "   $AppDataPath" -ForegroundColor White
    }
} else {
    Write-Host "ℹ️  No se encontraron datos de aplicación" -ForegroundColor Gray
}

Write-Host ""

# ========================================
# PASO 6: Desinstalar SQL Server LocalDB (opcional)
# ========================================
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkGray
Write-Host "PASO 6: SQL Server LocalDB..." -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkGray

$localDbInstalled = Get-Command sqllocaldb -ErrorAction SilentlyContinue

if ($localDbInstalled) {
    Write-Host "⚠️  SQL Server LocalDB está instalado" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "   NOTA: LocalDB puede ser usado por otras aplicaciones." -ForegroundColor Cyan
    Write-Host "   No se recomienda desinstalarlo a menos que esté seguro." -ForegroundColor Cyan
    Write-Host ""
    
    $uninstallLocalDb = Read-Host "¿Desea desinstalar SQL Server LocalDB? (S/N)"
    
    if ($uninstallLocalDb -eq "S" -or $uninstallLocalDb -eq "s") {
        Write-Host ""
        Write-Host "Para desinstalar SQL Server LocalDB:" -ForegroundColor Yellow
        Write-Host "1. Abra Panel de Control" -ForegroundColor White
        Write-Host "2. Vaya a 'Programas y características'" -ForegroundColor White
        Write-Host "3. Busque 'Microsoft SQL Server LocalDB'" -ForegroundColor White
        Write-Host "4. Click derecho → Desinstalar" -ForegroundColor White
        Write-Host ""
        Write-Host "O ejecute el siguiente comando:" -ForegroundColor Yellow
        Write-Host "msiexec /x {GUID-DE-LOCALDB} /qn" -ForegroundColor White
        Write-Host ""
        
        $openControlPanel = Read-Host "¿Desea abrir Panel de Control ahora? (S/N)"
        if ($openControlPanel -eq "S" -or $openControlPanel -eq "s") {
            Start-Process "control" "appwiz.cpl"
        }
    } else {
        Write-Host "⏭️  SQL Server LocalDB conservado" -ForegroundColor Cyan
    }
} else {
    Write-Host "ℹ️  SQL Server LocalDB no está instalado" -ForegroundColor Gray
}

Write-Host ""

# ========================================
# RESUMEN DE DESINSTALACIÓN
# ========================================
Write-Host "╔═══════════════════════════════════════════════════════════╗" -ForegroundColor Green
Write-Host "║                                                           ║" -ForegroundColor Green
Write-Host "║         ✅ DESINSTALACIÓN COMPLETADA                       ║" -ForegroundColor Green
Write-Host "║                                                           ║" -ForegroundColor Green
Write-Host "╚═══════════════════════════════════════════════════════════╝" -ForegroundColor Green
Write-Host ""
Write-Host "📋 Resumen de acciones realizadas:" -ForegroundColor Cyan
Write-Host ""

if (!(Test-Path $InstallPath)) {
    Write-Host "   ✅ Archivos de aplicación eliminados" -ForegroundColor Green
} else {
    Write-Host "   ⚠️  Archivos de aplicación NO eliminados" -ForegroundColor Yellow
}

if (!(Test-Path "$DesktopPath\MARTE.lnk")) {
    Write-Host "   ✅ Acceso directo del escritorio eliminado" -ForegroundColor Green
} else {
    Write-Host "   ⚠️  Acceso directo del escritorio NO eliminado" -ForegroundColor Yellow
}

if (!(Test-Path "$StartMenuPath\MARTE.lnk")) {
    Write-Host "   ✅ Acceso directo del menú inicio eliminado" -ForegroundColor Green
} else {
    Write-Host "   ⚠️  Acceso directo del menú inicio NO eliminado" -ForegroundColor Yellow
}

if (Test-Path $AppDataPath) {
    Write-Host "   ℹ️  Datos de aplicación conservados en:" -ForegroundColor Cyan
    Write-Host "      $AppDataPath" -ForegroundColor White
} else {
    Write-Host "   ✅ Datos de aplicación eliminados" -ForegroundColor Green
}

Write-Host ""

if (Test-Path "$env:USERPROFILE\Desktop\MARTE_Backup_*") {
    $backupFolders = Get-ChildItem "$env:USERPROFILE\Desktop" -Directory -Filter "MARTE_Backup_*" | Sort-Object CreationTime -Descending | Select-Object -First 1
    Write-Host "💾 Backup de base de datos creado en:" -ForegroundColor Cyan
    Write-Host "   $($backupFolders.FullName)" -ForegroundColor White
    Write-Host ""
}

Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkGray
Write-Host ""
Write-Host "Gracias por usar Sistema MARTE" -ForegroundColor Cyan
Write-Host ""

Read-Host "Presione ENTER para finalizar"
