# PROYECTO MARTE
## Sistema de Gestión de Asistencias para Nueva Acrópolis

[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/download)
[![WPF](https://img.shields.io/badge/WPF-Windows-blue.svg)](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/)
[![Entity Framework](https://img.shields.io/badge/Entity%20Framework-Core-orange.svg)](https://docs.microsoft.com/en-us/ef/)
[![Version](https://img.shields.io/badge/Version-1.3.0-blue.svg)](README.md)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

**MARTE** es un sistema de gestión de asistencias desarrollado para Nueva Acrópolis, con instalación automatizada, LocalDB embebido y arquitectura Clean Architecture.

> **Versión 1.3.0** - Instalación Zero-Config • Dashboard en tiempo real • Sistema de Auditoría • Reportes avanzados

---

## Tabla de Contenidos

- [Características Principales](#características-principales)
- [Instalación Rápida](#instalación-rápida)
- [Arquitectura](#arquitectura)
- [Tecnologías](#tecnologías)
- [Documentación](#documentación)
- [Testing](#testing)
- [Licencia](#licencia)

---

## Características Principales

### 🚀 Instalación Zero-Config
- **Instalación Automatizada**: Script PowerShell con detección de dependencias (.NET 9 + LocalDB)
- **SQL Server LocalDB Embebido**: Sin configuración manual de SQL Server
- **Accesos directos automáticos**: Desktop y Menú Inicio
- **Desinstalador con Backup**: Respaldo opcional de datos
- **Actualizaciones Simples**: Preserva datos automáticamente

### 🎯 Funcionalidades Core
- **Gestión de Asistentes**: CRUD completo con categorías y grupos
- **Control de Asistencia**: Registro de entrada/salida con timestamps
- **Dashboard en Tiempo Real**: Indicadores actualizados desde BD
- **Sistema de Reportes**: 10 tipos con exportación PDF/Excel/TXT
- **Módulo de Auditoría**: Trazabilidad completa con filtros avanzados

### 🔒 Seguridad
- Autenticación BCrypt con hash de contraseñas
- Sistema de roles: Administrador, Coordinador, Usuario
- Control de acceso basado en roles (RBAC)
- Logs de auditoría completos

### 📊 Dashboard Destacado
- Asistentes del día actual
- Tasa de puntualidad (< 7:40 PM)
- Top 5 más constantes (últimos 30 días)
- Distribución por grupos de miembros

---

## Arquitectura

### Clean Architecture - 4 Capas

```
┌─────────────────────────────────────────┐
│  MARTE.WPF (Presentación)               │
│  Views • ViewModels • Styles            │
├─────────────────────────────────────────┤
│  MARTE.APPLICATION (Aplicación)         │
│  Services • DTOs • Interfaces           │
├─────────────────────────────────────────┤
│  MARTE.INFRASTRUCTURE (Infraestructura) │
│  Repositories • DbContext • Migrations  │
├─────────────────────────────────────────┤
│  MARTE.DOMAIN (Dominio - Núcleo)        │
│  Entities • DTOs • Sin dependencias     │
└─────────────────────────────────────────┘
```

**Estructura del Proyecto:**
```
MARTE/
├── Marte.Domain/         # 7 Entidades + 10 DTOs
├── Marte.Application/    # Services + ViewModels
├── Marte.Infrastructure/ # Repositories + Migrations
├── Marte.WPF/           # UI XAML + ViewModels
└── Marte.Tests/         # 63 Tests (100% passing)
```

---

## Instalación Rápida

### Para Usuarios Finales

**Requisitos**: Windows 10/11 (64-bit) • 500 MB espacio • Internet (instalación inicial)

1. Descargar `MARTE-Installer-v1.3.0.zip` desde [Releases](https://github.com/AndyAgurto/Proyecto-Marte/releases)
2. Ejecutar `install-marte.ps1` como Administrador
3. Iniciar MARTE (acceso directo en Desktop)
4. Login por defecto: `druagurto` / `@Druagurto00`

**El instalador automáticamente:**
- ✅ Instala .NET 9 Runtime + SQL LocalDB
- ✅ Crea base de datos en `%LocalAppData%\MARTE\Data\`
- ✅ Configura accesos directos

📖 **Guía completa**: [`README-INSTALACION.md`](./README-INSTALACION.md)

### Para Desarrolladores

**Requisitos**: .NET 9 SDK • SQL Server • Visual Studio 2022 / VS Code

```bash
# Clonar repositorio
git clone https://github.com/AndyAgurto/Proyecto-Marte.git
cd Proyecto-Marte

# Restaurar dependencias
dotnet restore

# Crear base de datos (primera vez)
dotnet ef database update --project Marte.Infrastructure --startup-project Marte.WPF

# Ejecutar aplicación
dotnet run --project Marte.WPF

# O con Hot Reload
dotnet watch --project Marte.WPF run
```

**Compilación Condicional:**
- **Debug**: Usa SQL Server (`Server=.\DRUAGURTO`)
- **Release**: Usa LocalDB embebido (`%LocalAppData%\MARTE\Data\`)

**Comandos útiles:**
```bash
# Crear migración
dotnet ef migrations add NombreMigracion --project Marte.Infrastructure --startup-project Marte.WPF

# Compilar para producción
dotnet build -c Release

# Crear paquete de distribución
.\crear-paquete-distribucion.ps1
```

---

## Tecnologías

### Stack Principal
- **.NET 9.0** - Framework moderno de Microsoft
- **WPF + XAML** - Interfaz de usuario con patrón MVVM
- **Entity Framework Core 9.0.10** - ORM para acceso a datos
- **SQL Server LocalDB 2022** - Base de datos embebida (producción)
- **BCrypt.Net-Next 4.0.3** - Encriptación de contraseñas

### Librerías Destacadas
- **ClosedXML 0.105.0** - Exportación a Excel
- **QuestPDF 2025.7.3** - Generación de PDFs
- **xUnit 2.9.2** - Testing (63 tests)

### Herramientas
- **PowerShell** - Scripts de instalación automatizada
- **Visual Studio 2022** - IDE principal
- **Git** - Control de versiones



---

## Testing

**Suite Completa**: 63 tests • 100% passing • xUnit 2.9.2 • SQLite in-memory

### Módulos Cubiertos

| Módulo | Tests | Estado |
|--------|-------|--------|
| Autenticación | 1 | ✅ |
| Database Seeding | 1 | ✅ |
| Gestión de Usuarios | 8 | ✅ |
| Gestión de Asistentes | 15 | ✅ |
| Control de Asistencias | 12 | ✅ |
| Dashboard & Estadísticas | 8 | ✅ |
| Sistema de Auditoría | 6 | ✅ |
| Generación de Reportes | 8 | ✅ |
| Configuración | 6 | ✅ |
| **TOTAL** | **63** | **✅** |

### Ejecutar Tests

```bash
dotnet test                              # Todos los tests
dotnet test --verbosity detailed         # Con detalles
dotnet test --collect:"XPlat Code Coverage"  # Con cobertura
```

📖 **Documentación completa**: [`DOCUMENTACION_TESTS.md`](./DOCUMENTACION_TESTS.md)



---

## Documentación

### 📚 Guías Disponibles

**Para Usuarios Finales:**
- [`README-INSTALACION.md`](./README-INSTALACION.md) - Instalación paso a paso con troubleshooting
- [`GUIA_USUARIO_MARTE.md`](./GUIA_USUARIO_MARTE.md) - Manual de usuario completo con capturas

**Para Desarrolladores:**
- [`DOCUMENTACION_TECNICA_GENERAL.md`](./DOCUMENTACION_TECNICA_GENERAL.md) - Arquitectura completa del sistema
- [`CONEXIONES.md`](./CONEXIONES.md) - SQL Server (Desarrollo)
- [`CONEXIONES-LOCALDB.md`](./CONEXIONES-LOCALDB.md) - LocalDB (Producción)
- [`DOCUMENTACION_TESTS.md`](./DOCUMENTACION_TESTS.md) - Suite de pruebas automatizadas

### � Recursos Externos

- [WPF Documentation](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [Clean Architecture Guide](https://docs.microsoft.com/en-us/dotnet/architecture/)

---

## Licencia

MIT License - Ver [LICENSE](LICENSE) para más detalles

Copyright © 2025 Andy Agurto - Proyecto MARTE

---

## Autor

**Andy Agurto** - Ingeniero de Sistemas  
🔗 [GitHub](https://github.com/AndyAgurto) • 📧 druagurto@hotmail.com

---

## Información del Proyecto

**Versión**: 1.3.0 • **Estado**: ✅ Producción • **Actualizado**: Oct 2025

**Métricas**: 15,000 LOC • 63 Tests (100%) • 4 Capas • Clean Architecture

---

## Changelog

### v1.3.0 (2025-10-18) - Actual
- ✅ LocalDB embebido + Instalación Zero-Config
- ✅ Scripts PowerShell automatizados
- ✅ Compilación condicional (Debug/Release)
- ✅ Dashboard dinámico con grupos

### v1.2.0 (2025-10-17)
- ✅ Dashboard en tiempo real
- ✅ Módulo de auditoría completo
- ✅ Sistema de reportes (10 tipos)

### v1.1.0 (2025-10-16)
- ✅ Control de asistencia avanzado
- ✅ Exportación Excel/PDF

### v1.0.0 (2025-10-15)
- ✅ Lanzamiento inicial
- ✅ Clean Architecture
- ✅ Autenticación BCrypt

---

<p align="center">
  <strong>Desarrollado con ❤️ usando .NET 9.0 para Nueva Acrópolis</strong><br>
  <sub>© 2025 Andy Agurto - Licencia MIT</sub>
</p>
