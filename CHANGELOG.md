# Changelog - MARTE

Todos los cambios notables de este proyecto serán documentados en este archivo.

El formato está basado en [Keep a Changelog](https://keepachangelog.com/es-ES/1.0.0/),
y este proyecto adhiere a [Semantic Versioning](https://semver.org/lang/es/).

---

## [1.3.0] - 2025-10-18

### 🎯 "LocalDB Revolution" - Instalación Zero Configuration

### Agregado
- **Instalación Automática Completa**
  - Script `install-marte.ps1` con detección de dependencias (versión FINAL sin emojis)
  - Script `uninstall-marte.ps1` con opción de backup (versión FINAL sin emojis)
  - Script `crear-paquete-instalador.ps1` para empaquetado
  - Wrappers BAT (`INSTALAR-MARTE.bat`, `DESINSTALAR-MARTE.bat`) con ExecutionPolicy Bypass
  - Instalación automática de .NET 9 Runtime
  - Instalación automática de SQL Server LocalDB
  - Creación de accesos directos en Desktop y Menú Inicio
  - Detección automática de ruta con `$PSScriptRoot`

- **SQL Server LocalDB Embebido**
  - Base de datos portable en `%LocalAppData%\MARTE\Data\`
  - Compilación condicional (Debug: SQL Server / Release: LocalDB)
  - Migraciones automáticas al primer inicio
  - Seed de datos iniciales automático
  - Zero configuration para usuarios finales

- **Ventana "Acerca de"**
  - Modal con información completa del sistema
  - Versión, desarrollador, tecnologías, licencia
  - Links clickeables (email y GitHub)
  - Botón en header de MainWindow
  - Diseño con colores institucionales

- **Primera Ejecución Automática**
  - Detección automática de primera ejecución
  - Mensaje de confirmación al usuario
  - Creación automática de base de datos
  - Aplicación de migraciones de EF Core
  - Mensaje con credenciales por defecto

- **Documentación**
  - `INSTALADOR-FINAL-README.md` - Guía completa del instalador final
  - `README-COMPLETO.md` - Documentación técnica extendida (1,652 líneas)
  - `CONEXIONES-LOCALDB.md` - Guía técnica de LocalDB
  - `RELEASE-NOTES-v1.3.0.md` - Notas detalladas de la versión
  - `VERSION.txt` - Metadatos de versión
  - `CHANGELOG.md` - Este archivo

### Cambiado
- **README.md** - Reducido a versión concisa (~300 líneas)
- **MainWindow** - Footer actualizado a v1.3.0
- **Documentación** - Eliminadas opciones manuales, enfoque en automatización

### Mejorado
- Proceso de instalación 10x más rápido y simple
- Experiencia de usuario para instalación sin conocimientos técnicos
- Separación clara entre entornos desarrollo/producción
- Documentación más organizada y accesible

### Corregido
- **Errores de parsing en PowerShell**: Emojis Unicode causaban "Falta la cadena en el terminador"
  - Solución: Formato profesional ASCII con prefijos `[OK]`, `[ERROR]`, `[AVISO]`
- **Error "No se encontraron archivos de aplicacion"**: Working directory incorrecto
  - Solución: Agregado `Set-Location -Path $PSScriptRoot` en scripts
- **Auto-ejecución fallaba sin permisos**: Intentaba ejecutar MARTE causando confusión
  - Solución: Eliminada opción de ejecutar automáticamente después de instalación
- **Políticas de ejecución bloqueaban scripts**: PowerShell restrictivo
  - Solución: Wrappers BAT con `-ExecutionPolicy Bypass`

### Técnico
- Compilación condicional `#if DEBUG` / `#else` para entornos
- Rutas dinámicas con `Environment.SpecialFolder.LocalApplicationData`
- Migraciones automáticas con `Database.MigrateAsync()`
- Manejo de errores mejorado en inicialización

---

## [1.2.0] - 2025-10-17

### Agregado
- **Dashboard en Tiempo Real**
  - Indicadores con datos reales desde base de datos
  - Asistentes del día actual
  - Miembros hasta el cierre (día anterior)
  - Porcentaje de puntualidad (<19:40)
  - Top 5 asistentes más constantes
  - Selector dinámico de categorías
  - Grupos de miembros con carga dinámica

- **Módulo de Auditoría Completo**
  - Tabla `AuditLog` en base de datos
  - Registro automático de todas las acciones
  - Filtros por fecha (inicio/fin) y usuario
  - Búsqueda combinada
  - Exportación a TXT con formato estructurado
  - Vista en DataGrid ordenable
  - Contador de registros totales

- **Sistema de Reportes Avanzado**
  - 10 tipos de reportes diferentes
  - Exportación a Excel (ClosedXML)
  - Exportación a PDF (QuestPDF)
  - Filtros por fecha, categoría, grupo
  - Vista previa en DataGrid
  - Reportes analíticos de puntualidad y constancia

### Cambiado
- Dashboard migrado de datos estáticos a consultas SQL reales
- Mejoras en rendimiento de consultas con índices

---

## [1.1.0] - 2025-10-16

### Agregado
- **Gestión de Asistentes**
  - CRUD completo (Create, Read, Update, Delete)
  - Validación de DNI único
  - Asignación de categorías y grupos
  - Estado activo/inactivo (soft delete)
  - Filtros por estado

- **Control de Asistencias**
  - Registro de ingreso por DNI
  - Registro de salida con hora
  - Opción "hasta cierre"
  - Cierre automático masivo
  - Vista de presentes en tiempo real
  - Historial por fecha

- **Exportación de Datos**
  - Exportación a Excel (.xlsx)
  - Exportación a PDF con diseño profesional
  - Formato de auditoría en TXT

### Mejorado
- Validaciones de integridad en formularios
- Mensajes de error más descriptivos
- Performance en consultas de asistencias

---

## [1.0.0] - 2025-10-15

### 🎉 Lanzamiento Inicial

### Agregado
- **Arquitectura Clean Architecture**
  - 4 capas: Domain, Application, Infrastructure, WPF
  - Separación de responsabilidades
  - Inyección de dependencias con Microsoft.Extensions.DependencyInjection
  - Patrón Repository
  - MVVM en capa de presentación

- **Autenticación y Seguridad**
  - Sistema de login con validación
  - Encriptación BCrypt para contraseñas
  - Gestión de roles (Administrador, Coordinador, Usuario)
  - Sesiones seguras con SessionService
  - Control de acceso basado en roles (RBAC)

- **Gestión de Usuarios**
  - CRUD completo de usuarios
  - Asignación de roles
  - Cambio de contraseña
  - Activación/inactivación de cuentas
  - Auditoría básica de acciones

- **Base de Datos**
  - SQL Server con Entity Framework Core 9.0.10
  - Migraciones de EF Core
  - Database Seeder con datos iniciales:
    - Roles predefinidos
    - Categorías de asistentes
    - Usuario administrador por defecto
    - Configuración de escuela

- **Entidades del Dominio**
  - `Usuario` - Usuarios del sistema
  - `Rol` - Roles de seguridad
  - `Asistente` - Miembros de Nueva Acrópolis
  - `Asistencia` - Registro de asistencias
  - `Categoria` - Categorías de asistentes
  - `ConfiguracionEscuela` - Configuración general
  - `AuditLog` - Bitácora de auditoría

- **Interfaz de Usuario**
  - Tema oscuro institucional
  - Paleta de colores: Rojo (#8B0000) y Dorado (#FFD700)
  - Fuente Cinzel (Times New Roman fallback)
  - Efectos hover y transiciones
  - LoginWindow con validación
  - MainWindow con sidebar de navegación
  - Dashboard con paneles informativos

- **Testing**
  - Suite de pruebas con xUnit 2.9.2
  - 63 tests unitarios (100% passing)
  - SQLite in-memory para tests aislados
  - Cobertura 100% en módulos críticos

- **Documentación Inicial**
  - README.md con información completa
  - DOCUMENTACION_TECNICA_GENERAL.md
  - CONEXIONES.md para configuración SQL Server
  - Comentarios XML en código C#

---

## [Unreleased]

### Planeado para v1.4.0
- [ ] Gráficos interactivos en Dashboard (ChartJS/LiveCharts)
- [ ] Modo offline con cache local
- [ ] Tema claro/oscuro personalizable
- [ ] Notificaciones del sistema
- [ ] Backup automático programado

### Planeado para v2.0.0
- [ ] API REST para integración externa
- [ ] Aplicación móvil con .NET MAUI
- [ ] Sincronización cloud (Azure/AWS)
- [ ] Reconocimiento facial para asistencias
- [ ] Multi-idioma (español/inglés/portugués)

---

## Tipos de Cambios

- **Agregado** - para nuevas funcionalidades
- **Cambiado** - para cambios en funcionalidades existentes
- **Deprecado** - para funcionalidades que serán eliminadas
- **Eliminado** - para funcionalidades eliminadas
- **Corregido** - para corrección de bugs
- **Seguridad** - para vulnerabilidades de seguridad
- **Mejorado** - para mejoras de rendimiento o UX
- **Técnico** - para cambios técnicos internos

---

[1.3.0]: https://github.com/AndyAgurto/Proyecto-Marte/compare/v1.2.0...v1.3.0
[1.2.0]: https://github.com/AndyAgurto/Proyecto-Marte/compare/v1.1.0...v1.2.0
[1.1.0]: https://github.com/AndyAgurto/Proyecto-Marte/compare/v1.0.0...v1.1.0
[1.0.0]: https://github.com/AndyAgurto/Proyecto-Marte/releases/tag/v1.0.0
[Unreleased]: https://github.com/AndyAgurto/Proyecto-Marte/compare/v1.3.0...HEAD
