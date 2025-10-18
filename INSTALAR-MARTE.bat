@echo off
REM ========================================
REM Instalador MARTE v1.3.0
REM Este archivo ejecuta el script PowerShell con los permisos necesarios
REM ========================================

echo.
echo ========================================
echo    INSTALADOR DE MARTE v1.3.0
echo ========================================
echo.
echo Iniciando instalacion...
echo.

REM Ejecutar PowerShell con bypass de politicas de ejecucion
powershell.exe -ExecutionPolicy Bypass -NoProfile -File "%~dp0install-marte.ps1"

if %ERRORLEVEL% EQU 0 (
    echo.
    echo [OK] Instalacion completada
) else (
    echo.
    echo [ERROR] Error durante la instalacion
    echo Codigo de error: %ERRORLEVEL%
)

echo.
pause
