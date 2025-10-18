# ========================================
# Script para crear paquete de distribución
# Sistema MARTE v1.3.0
# ========================================

param(
    [string]$OutputPath = ".\MARTE-Installer"
)

Clear-Host
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  CREADOR DE PAQUETE DE DISTRIBUCION   " -ForegroundColor Cyan
Write-Host "        MARTE v1.3.0                    " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# PASO 1: Compilar en modo Release
Write-Host "PASO 1: Compilando proyecto en modo Release..." -ForegroundColor Yellow
dotnet publish .\Marte.WPF\Marte.WPF.csproj -c Release -o .\Marte.WPF\bin\Publish --self-contained false

if ($LASTEXITCODE -ne 0) {
    Write-Host "[ERROR] Compilacion fallida" -ForegroundColor Red
    exit 1
}

Write-Host "[OK] Compilacion exitosa" -ForegroundColor Green
Write-Host ""

# PASO 2: Crear estructura de carpetas
Write-Host "PASO 2: Creando estructura de carpetas..." -ForegroundColor Yellow

if (Test-Path $OutputPath) {
    Write-Host "  Eliminando carpeta existente..." -ForegroundColor Gray
    Remove-Item -Path $OutputPath -Recurse -Force
}

New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null
New-Item -ItemType Directory -Path "$OutputPath\Archivos" -Force | Out-Null

Write-Host "[OK] Carpetas creadas: $OutputPath" -ForegroundColor Green
Write-Host ""

# PASO 3: Copiar archivos compilados
Write-Host "PASO 3: Copiando archivos de aplicacion..." -ForegroundColor Yellow
Copy-Item -Path ".\Marte.WPF\bin\Publish\*" -Destination "$OutputPath\Archivos" -Recurse -Force
Write-Host "[OK] Archivos copiados" -ForegroundColor Green
Write-Host ""

# PASO 4: Copiar scripts
Write-Host "PASO 4: Copiando scripts de instalacion..." -ForegroundColor Yellow

if (Test-Path ".\install-marte.ps1") {
    Copy-Item -Path ".\install-marte.ps1" -Destination "$OutputPath\" -Force
    Write-Host "  [OK] install-marte.ps1" -ForegroundColor Green
}

if (Test-Path ".\uninstall-marte.ps1") {
    Copy-Item -Path ".\uninstall-marte.ps1" -Destination "$OutputPath\" -Force
    Write-Host "  [OK] uninstall-marte.ps1" -ForegroundColor Green
}

if (Test-Path ".\INSTALAR-MARTE.bat") {
    Copy-Item -Path ".\INSTALAR-MARTE.bat" -Destination "$OutputPath\" -Force
    Write-Host "  [OK] INSTALAR-MARTE.bat" -ForegroundColor Green
}

if (Test-Path ".\DESINSTALAR-MARTE.bat") {
    Copy-Item -Path ".\DESINSTALAR-MARTE.bat" -Destination "$OutputPath\" -Force
    Write-Host "  [OK] DESINSTALAR-MARTE.bat" -ForegroundColor Green
}

if (Test-Path ".\LEEME-PRIMERO.txt") {
    Copy-Item -Path ".\LEEME-PRIMERO.txt" -Destination "$OutputPath\" -Force
    Write-Host "  [OK] LEEME-PRIMERO.txt" -ForegroundColor Green
}

Write-Host ""

# PASO 5: Copiar documentación
Write-Host "PASO 5: Copiando documentacion..." -ForegroundColor Yellow

$docs = @(
    "README-INSTALACION.md",
    "VERSION.txt",
    "RELEASE-NOTES-v1.3.0.md"
)

foreach ($doc in $docs) {
    if (Test-Path ".\$doc") {
        Copy-Item -Path ".\$doc" -Destination "$OutputPath\" -Force
        Write-Host "  [OK] $doc" -ForegroundColor Green
    }
}

Write-Host ""

# PASO 6: Crear archivo VERSION.txt en el paquete
Write-Host "PASO 6: Generando archivo VERSION.txt..." -ForegroundColor Yellow

$versionContent = @"
MARTE - Sistema de Gestion de Asistencias
==========================================

Version: 1.3.0
Fecha: 18 de Octubre, 2025
Estado: Stable Release

Desarrollador: Andy Agurto Urcia
Organizacion: AU Developers (MVP)
Cliente: Nueva Acropolis

Framework: .NET 9.0
Base de Datos: SQL Server LocalDB 2022

Licencia: MIT License
Copyright 2025 Andy Agurto

Repositorio: https://github.com/AndyAgurto/Proyecto-Marte
Soporte: druagurto@hotmail.com

CONTENIDO DEL PAQUETE:
- Archivos/: Aplicacion compilada
- install-marte.ps1: Script de instalacion
- uninstall-marte.ps1: Script de desinstalacion
- README-INSTALACION.md: Guia de usuario
- VERSION.txt: Este archivo

INSTALACION RAPIDA:
1. Click derecho en install-marte.ps1
2. Ejecutar como administrador
3. Seguir las instrucciones en pantalla

Para mas informacion, consulte README-INSTALACION.md
"@

$versionContent | Out-File -FilePath "$OutputPath\VERSION.txt" -Encoding UTF8 -Force
Write-Host "[OK] VERSION.txt generado" -ForegroundColor Green
Write-Host ""

# PASO 7: Mostrar resumen
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "    PAQUETE CREADO EXITOSAMENTE        " -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Ubicacion: $OutputPath" -ForegroundColor White
Write-Host ""

# Listar contenido
Write-Host "CONTENIDO DEL PAQUETE:" -ForegroundColor Cyan
Write-Host ""
Get-ChildItem -Path $OutputPath -Recurse | Select-Object -First 20 | ForEach-Object {
    $relativePath = $_.FullName.Replace((Get-Location).Path, ".")
    if ($_.PSIsContainer) {
        Write-Host "  [DIR]  $relativePath" -ForegroundColor Blue
    } else {
        $size = "{0:N2} KB" -f ($_.Length / 1KB)
        Write-Host "  [FILE] $relativePath ($size)" -ForegroundColor Gray
    }
}

Write-Host ""
Write-Host "PROXIMOS PASOS:" -ForegroundColor Cyan
Write-Host "1. Comprimir la carpeta a ZIP:" -ForegroundColor White
Write-Host "   Compress-Archive -Path '$OutputPath\*' -DestinationPath 'MARTE-Installer-v1.3.0.zip'" -ForegroundColor Yellow
Write-Host ""
Write-Host "2. Subir el ZIP a GitHub Release:" -ForegroundColor White
Write-Host "   https://github.com/AndyAgurto/Proyecto-Marte/releases/tag/v1.3.0" -ForegroundColor Yellow
Write-Host ""
Write-Host "[EXITO] Paquete de distribucion listo!" -ForegroundColor Green
