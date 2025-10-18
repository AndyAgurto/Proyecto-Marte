@echo off
REM ========================================
REM Desinstalador MARTE v1.3.0
REM Este archivo ejecuta el script PowerShell con los permisos necesarios
REM ========================================

echo.
echo ╔═══════════════════════════════════════════════════════════╗
echo ║                                                           ║
echo ║            DESINSTALADOR DE MARTE v1.3.0                  ║
echo ║                                                           ║
echo ╚═══════════════════════════════════════════════════════════╝
echo.
echo Iniciando desinstalacion...
echo.

REM Ejecutar PowerShell con bypass de politicas de ejecucion
powershell.exe -ExecutionPolicy Bypass -NoProfile -File "%~dp0uninstall-marte.ps1"

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ✅ Desinstalacion completada
) else (
    echo.
    echo ❌ Error durante la desinstalacion
    echo Codigo de error: %ERRORLEVEL%
)

echo.
pause
