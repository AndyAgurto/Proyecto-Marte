# ========================================
# Script para crear paquete de distribución
# Sistema MARTE v1.3.0
# ========================================

param(
    [string]$OutputPath = ".\MARTE-Installer"
)

Clear-Host
Write-Host "╔═══════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║                                                           ║" -ForegroundColor Cyan
Write-Host "║         CREADOR DE PAQUETE DE DISTRIBUCIÓN MARTE          ║" -ForegroundColor Cyan
Write-Host "║                                                           ║" -ForegroundColor Cyan
Write-Host "╚═══════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host ""

# Compilar en modo Release
Write-Host "PASO 1: Compilando proyecto en modo Release..." -ForegroundColor Cyan
dotnet publish .\Marte.WPF\Marte.WPF.csproj -c Release -o .\Marte.WPF\bin\Publish --self-contained false

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Error en compilación" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Compilación exitosa" -ForegroundColor Green
Write-Host ""

# Crear carpeta de distribución
Write-Host "PASO 2: Creando carpeta de distribución..." -ForegroundColor Cyan

if (Test-Path $OutputPath) {
    Remove-Item -Path $OutputPath -Recurse -Force
}

New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null
New-Item -ItemType Directory -Path "$OutputPath\Archivos" -Force | Out-Null

Write-Host "✅ Carpeta creada: $OutputPath" -ForegroundColor Green
Write-Host ""

# Copiar archivos compilados
Write-Host "PASO 3: Copiando archivos de aplicación..." -ForegroundColor Cyan
Copy-Item -Path ".\Marte.WPF\bin\Publish\*" -Destination "$OutputPath\Archivos" -Recurse -Force
Write-Host "✅ Archivos copiados" -ForegroundColor Green
Write-Host ""

# Copiar script de instalación
Write-Host "PASO 4: Copiando scripts de instalación y desinstalación..." -ForegroundColor Cyan
Copy-Item -Path ".\install-marte.ps1" -Destination "$OutputPath\" -Force
Copy-Item -Path ".\uninstall-marte.ps1" -Destination "$OutputPath\" -Force
Write-Host "✅ Scripts copiados" -ForegroundColor Green
Write-Host ""

# Copiar README
Write-Host "PASO 5: Copiando documentación..." -ForegroundColor Cyan
Copy-Item -Path ".\README-INSTALACION.md" -Destination "$OutputPath\README.md" -Force
Write-Host "✅ Documentación copiada" -ForegroundColor Green
Write-Host ""

# Crear archivo de versión
Write-Host "PASO 6: Creando archivo de versión..." -ForegroundColor Cyan
$versionInfo = @"
Sistema MARTE
Versión: 1.3.0
Fecha de compilación: $(Get-Date -Format "dd/MM/yyyy HH:mm:ss")
Framework: .NET 9.0
Base de Datos: SQL Server LocalDB

Contenido del paquete:
- Archivos de aplicación (carpeta Archivos)
- Script de instalación (install-marte.ps1)
- Script de desinstalación (uninstall-marte.ps1)
- Documentación (README.md)

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
"@

$versionInfo | Out-File "$OutputPath\VERSION.txt" -Encoding UTF8
Write-Host "✅ Archivo de versión creado" -ForegroundColor Green
Write-Host ""

# Opcional: Descargar SqlLocalDB.msi
Write-Host "PASO 7: ¿Desea incluir SqlLocalDB.msi en el paquete?" -ForegroundColor Cyan
Write-Host "   (Aumentará el tamaño del paquete en ~50 MB pero permitirá instalación offline)" -ForegroundColor Yellow
$includeLocalDB = Read-Host "   Incluir SqlLocalDB.msi? (S/N)"

if ($includeLocalDB -eq "S" -or $includeLocalDB -eq "s") {
    if (Test-Path ".\SqlLocalDB.msi") {
        Write-Host "   Copiando SqlLocalDB.msi existente..." -ForegroundColor Cyan
        Copy-Item -Path ".\SqlLocalDB.msi" -Destination "$OutputPath\" -Force
        Write-Host "   ✅ SqlLocalDB.msi incluido" -ForegroundColor Green
    } else {
        Write-Host "   ⚠️  SqlLocalDB.msi no encontrado en la carpeta actual" -ForegroundColor Yellow
        Write-Host "   Puede descargarlo manualmente desde:" -ForegroundColor Yellow
        Write-Host "   https://download.microsoft.com/download/3/8/d/38de7036-2433-4207-8eae-06e247e17b25/SqlLocalDB.msi" -ForegroundColor White
    }
} else {
    Write-Host "   ⏭️  SqlLocalDB.msi no incluido (se descargará durante instalación)" -ForegroundColor Yellow
}

Write-Host ""

# Mostrar resumen
Write-Host "╔═══════════════════════════════════════════════════════════╗" -ForegroundColor Green
Write-Host "║                                                           ║" -ForegroundColor Green
Write-Host "║         ✅ PAQUETE DE DISTRIBUCIÓN CREADO                  ║" -ForegroundColor Green
Write-Host "║                                                           ║" -ForegroundColor Green
Write-Host "╚═══════════════════════════════════════════════════════════╝" -ForegroundColor Green
Write-Host ""
Write-Host "📦 Ubicación del paquete:" -ForegroundColor Cyan
Write-Host "   $((Get-Item $OutputPath).FullName)" -ForegroundColor White
Write-Host ""
Write-Host "📁 Contenido:" -ForegroundColor Cyan
Get-ChildItem $OutputPath -Recurse | Where-Object { !$_.PSIsContainer } | ForEach-Object {
    $relativePath = $_.FullName.Replace((Get-Item $OutputPath).FullName, "")
    Write-Host "   .$relativePath" -ForegroundColor White
}
Write-Host ""
Write-Host "📊 Tamaño total:" -ForegroundColor Cyan
$totalSize = (Get-ChildItem $OutputPath -Recurse | Measure-Object -Property Length -Sum).Sum / 1MB
Write-Host "   $([math]::Round($totalSize, 2)) MB" -ForegroundColor White
Write-Host ""
Write-Host "🎯 Próximos pasos:" -ForegroundColor Cyan
Write-Host "   1. Comprimir la carpeta $OutputPath en un archivo ZIP" -ForegroundColor White
Write-Host "   2. Distribuir el archivo ZIP a los usuarios finales" -ForegroundColor White
Write-Host "   3. Los usuarios deben descomprimir y ejecutar install-marte.ps1" -ForegroundColor White
Write-Host ""

# Opcional: Crear archivo ZIP
$createZip = Read-Host "¿Desea crear un archivo ZIP ahora? (S/N)"
if ($createZip -eq "S" -or $createZip -eq "s") {
    $zipPath = "$OutputPath.zip"
    
    if (Test-Path $zipPath) {
        Remove-Item $zipPath -Force
    }
    
    Write-Host "Creando archivo ZIP..." -ForegroundColor Cyan
    Compress-Archive -Path "$OutputPath\*" -DestinationPath $zipPath -Force
    Write-Host "✅ Archivo ZIP creado: $zipPath" -ForegroundColor Green
    Write-Host ""
    Write-Host "📦 Tamaño del ZIP:" -ForegroundColor Cyan
    $zipSize = (Get-Item $zipPath).Length / 1MB
    Write-Host "   $([math]::Round($zipSize, 2)) MB" -ForegroundColor White
}

Write-Host ""
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkGray
Write-Host "✅ Proceso completado" -ForegroundColor Green
Read-Host "Presione ENTER para salir"
