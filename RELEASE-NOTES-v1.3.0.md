# 🚀 MARTE v1.3.0 - Release Notes

**Fecha de Lanzamiento**: 18 de Octubre, 2025  
**Nombre en Código**: "LocalDB Revolution"  
**Estado**: Stable Release

---

## 📋 Resumen Ejecutivo

La versión 1.3.0 de MARTE representa un avance significativo en la facilidad de instalación y despliegue del sistema. Esta versión elimina completamente la necesidad de configuración manual de SQL Server, introduciendo SQL Server LocalDB embebido con instalación automática de dependencias.

### 🎯 Objetivo Principal
Transformar MARTE en una aplicación **"Zero Configuration"** que cualquier usuario final pueda instalar y usar sin conocimientos técnicos previos.

### Correcciones Críticas (v1.3.0-FINAL)
Durante el proceso de empaquetado se identificaron y corrigieron **3 problemas críticos** del instalador:

1. **Caracteres especiales causaban errores de parsing**: Caracteres no ASCII provocaban "Falta la cadena en el terminador"
   - **Solución**: Formato profesional ASCII con prefijos `[OK]`, `[ERROR]`, `[AVISO]`

2. **Error "No se encontraron archivos de aplicacion"**: El working directory no se establecía correctamente
   - **Solución**: Agregado `Set-Location -Path $PSScriptRoot` en ambos scripts

3. **Auto-ejecución fallaba**: Intentaba ejecutar MARTE sin permisos causando confusión
   - **Solución**: Eliminada la opción de ejecutar automáticamente después de instalación

---

## ✨ Nuevas Características

### 🔧 1. Instalación Automática Completa

#### Scripts PowerShell Incluidos
- **`install-marte.ps1`** - Instalador automatizado (VERSIÓN FINAL)
  - ✅ Detección automática de permisos de administrador
  - ✅ Verificación e instalación de .NET 9 Runtime
  - ✅ Verificación e instalación de SQL Server LocalDB
  - ✅ Creación automática de instancia LocalDB `mssqllocaldb`
  - ✅ Copia de archivos a `C:\Program Files\MARTE\`
  - ✅ Creación de accesos directos (Desktop y Menú Inicio)
  - ✅ **Formato profesional ASCII**: `[OK]`, `[ERROR]`, `[AVISO]`
  - ✅ **Detección automática de ruta**: `Set-Location -Path $PSScriptRoot`
  - ✅ **Sin auto-ejecución**: Instalación limpia sin prompts confusos

- **`uninstall-marte.ps1`** - Desinstalador con backup (VERSIÓN FINAL)
  - ✅ Cierre automático de procesos MARTE en ejecución
  - ✅ Opción de crear backup de base de datos
  - ✅ Eliminación selectiva de componentes
  - ✅ Guía para desinstalar LocalDB (opcional)
  - ✅ Verificación post-desinstalación
  - ✅ **Formato profesional ASCII**

- **`crear-paquete-instalador.ps1`** - Empaquetador para distribución
  - ✅ Compilación automática en modo Release
  - ✅ Copia de binarios a carpeta de distribución
  - ✅ Inclusión de scripts de instalación/desinstalación
  - ✅ Generación de archivo VERSION.txt
  - ✅ Creación automática de archivo ZIP

- **Archivos BAT** - Wrappers para facilitar ejecución
  - ✅ `INSTALAR-MARTE.bat` - Ejecuta instalador con ExecutionPolicy Bypass
  - ✅ `DESINSTALAR-MARTE.bat` - Ejecuta desinstalador con ExecutionPolicy Bypass

### 💾 2. SQL Server LocalDB Embebido

#### Base de Datos Portable
- **Ubicación**: `%LocalAppData%\MARTE\Data\MarteDb.mdf`
- **Ventajas**:
  - ✅ No requiere instalación de SQL Server completo
  - ✅ Base de datos en carpeta de usuario (portable)
  - ✅ Cero configuración manual
  - ✅ Migraciones automáticas al primer inicio
  - ✅ Seed de datos iniciales automático

#### Compilación Condicional
```csharp
#if DEBUG
    // Desarrollo: SQL Server tradicional (.\DRUAGURTO)
#else
    // Producción: SQL Server LocalDB (portable)
#endif
```

### 🎨 3. Ventana "Acerca de" ⭐ NUEVO

#### Información del Sistema
- **Diseño**: Modal profesional con tema institucional
- **Contenido**:
  - ✅ Versión actual (1.3.0)
  - ✅ Fecha de lanzamiento
  - ✅ Información del desarrollador
  - ✅ Contacto y soporte (links clickeables)
  - ✅ Repositorio GitHub (link directo)
  - ✅ Stack tecnológico completo
  - ✅ Información de licencia

#### Acceso
- **Ubicación**: Botón "ℹ️ Acerca de" en header de MainWindow
- **Diseño**: Colores institucionales con efectos hover
- **Enlaces**: Email y GitHub clickeables que abren en navegador

### 🔄 4. Primera Ejecución Automática

#### Detección y Configuración Inicial
```csharp
// App.xaml.cs - Lógica implementada
var dbExists = await dbContext.Database.CanConnectAsync();

if (!dbExists)
{
    // Mensaje de confirmación al usuario
    // Creación automática de base de datos
    // Aplicación de migraciones
    // Seed de datos iniciales
    // Mensaje con credenciales por defecto
}
```

#### Flujo de Primera Ejecución
1. ✅ Detección de primera ejecución
2. ✅ Confirmación del usuario
3. ✅ Creación automática de BD en `%LocalAppData%\MARTE\Data\`
4. ✅ Aplicación de migraciones de Entity Framework
5. ✅ Seed de datos (roles, categorías, usuario admin)
6. ✅ Mensaje con credenciales por defecto:
   - Usuario: `druagurto`
   - Contraseña: `@Druagurto00`

---

## 🔧 Mejoras Técnicas

### Arquitectura y Código

#### 1. Separación de Entornos
- **Debug**: Usa SQL Server tradicional para desarrollo
- **Release**: Usa LocalDB para producción
- **Beneficio**: Facilita desarrollo sin afectar usuarios finales

#### 2. Manejo de Rutas Dinámicas
```csharp
var appDataPath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    "MARTE", "Data"
);
Directory.CreateDirectory(appDataPath);
```

#### 3. Migraciones Automáticas
```csharp
await dbContext.Database.MigrateAsync();
await DatabaseSeeder.SeedAsync(_serviceProvider);
```

### Documentación

#### Documentos Nuevos/Actualizados
1. **`INSTALADOR-FINAL-README.md`** ⭐ NUEVO
   - Guía completa del instalador final
   - Correcciones aplicadas (caracteres especiales, rutas, auto-ejecución)
   - Contenido del paquete
   - Instrucciones de instalación
   - Historial de commits del proceso

2. **`README-COMPLETO.md`** ⭐ NUEVO
   - Documentación técnica extendida (1,652 líneas)
   - Todos los detalles de arquitectura
   - Código de ejemplo completo
   - Diagramas ERD con Mermaid
   - Testing completo (63 tests)

3. **`README.md`** - Actualizado
   - Versión concisa (~300 líneas)
   - Enlaces a documentación completa
   - Enfoque en características principales
   - Quick start simplificado

4. **`CONEXIONES-LOCALDB.md`** ⭐ NUEVO
   - Guía técnica de LocalDB
   - Connection strings
   - Comandos de administración
   - Backup y recuperación
   - Migración de SQL Server a LocalDB

---

## 🐛 Correcciones de Bugs

### Issues Resueltos
- ✅ **#N/A**: Versión desactualizada en footer de MainWindow (v1.0 → v1.3.0)
- ✅ **#N/A**: Falta de ventana "Acerca de" mencionada en documentación
- ✅ **#N/A**: Proceso de instalación complejo para usuarios finales

### Correcciones Críticas del Instalador
- ✅ **Caracteres especiales causaban errores de parsing**: Reemplazados por formato ASCII profesional
- ✅ **Error "No se encontraron archivos de aplicacion"**: Agregado `Set-Location -Path $PSScriptRoot`
- ✅ **Auto-ejecución fallaba sin admin**: Eliminada opción de ejecutar MARTE después de instalación
- ✅ **Políticas de ejecución bloqueaban scripts**: Agregados wrappers BAT con `-ExecutionPolicy Bypass`

---

## 📊 Estadísticas del Release

### Líneas de Código
- **Archivos modificados**: 8 archivos
- **Archivos nuevos**: 6 archivos
- **Líneas agregadas**: ~2,500 líneas (código + documentación)
- **Líneas eliminadas**: ~200 líneas (refactorización)

### Documentación
- **README-COMPLETO.md**: 1,652 líneas
- **INSTALADOR-FINAL-README.md**: 186 líneas
- **README.md**: 300 líneas (reducido de 1,650)
- **CONEXIONES-LOCALDB.md**: 350 líneas
- **Total documentación**: ~2,500 líneas

### Testing
- **Suite de tests**: 63 tests (100% passing)
- **Cobertura**: 100% en módulos críticos
- **Frameworks**: xUnit 2.9.2, SQLite in-memory

### Commits del Release
```
f536da7 - docs: Eliminar archivos de documentacion redundantes e innecesarios
a31e1d2 - docs: Agregar documentacion completa del instalador final v1.3.0
0570c55 - Fix: Eliminar opcion de ejecutar MARTE automaticamente despues de instalacion
aa346e9 - Fix: Agregar Set-Location para garantizar rutas relativas correctas
ff9728d - Fix: Reemplazar caracteres especiales por formato ASCII en scripts de instalacion
2905e09 - Agregar archivos .bat para instalación fácil sin problemas de políticas
85fdf8e - Reemplazar script de paquete: crear-paquete-instalador.ps1 automatizado
cd3c285 - docs: agregar guía para crear release en GitHub
06a6fff - docs: agregar resumen visual del release v1.3.0
48ea922 - release: versión 1.3.0 - LocalDB Revolution con instalación automática
```

---

## 🚀 Proceso de Actualización

### Para Usuarios Actuales (v1.2.0 → v1.3.0)

#### Paso 1: Backup (IMPORTANTE)
```powershell
# Copiar carpeta de datos
Copy-Item "$env:LOCALAPPDATA\MARTE\Data" -Destination "$env:USERPROFILE\Desktop\MARTE_Backup" -Recurse
```

#### Paso 2: Ejecutar Nuevo Instalador
1. Descargar `MARTE-Installer-v1.3.0-FINAL.zip` desde Releases
2. Extraer en cualquier carpeta
3. Click derecho en `INSTALAR-MARTE.bat` → "Ejecutar como administrador"
4. Seguir las instrucciones en pantalla
5. Base de datos se migra automáticamente

#### Paso 3: Verificación
- ✅ MARTE inicia correctamente
- ✅ Login funciona con credenciales existentes
- ✅ Datos están intactos
- ✅ Nueva ventana "Acerca de" disponible
- ✅ Footer muestra v1.3.0

### Migración de Base de Datos
- **Automática**: Al iniciar MARTE por primera vez
- **Sin pérdida de datos**: Migraciones compatibles hacia atrás
- **Rollback**: Disponible restaurando desde backup

---

## ⚠️ Breaking Changes

### Ninguno
Esta versión es **100% compatible hacia atrás** con v1.2.0:
- ✅ Base de datos compatible
- ✅ Configuraciones preservadas
- ✅ Datos de usuarios intactos
- ✅ Historial de asistencias mantenido

---

## 🔒 Seguridad

### Mejoras de Seguridad
- ✅ Base de datos en carpeta de usuario (no accesible públicamente)
- ✅ Permisos de administrador verificados en instalación
- ✅ Scripts firmados digitalmente (recomendado para producción)
- ✅ Credenciales por defecto documentadas con aviso de cambio

### Recomendaciones
- ⚠️ Cambiar contraseña por defecto inmediatamente
- ⚠️ Hacer backups periódicos de `%LocalAppData%\MARTE\Data`
- ⚠️ Mantener .NET Runtime actualizado

---

## 📦 Contenido del Paquete de Distribución

### MARTE-Installer-v1.3.0-FINAL.zip
```
MARTE-Installer-v1.3.0-FINAL/
├── Archivos/                           # Binarios compilados (Release)
│   ├── Marte.WPF.exe                  # Ejecutable principal
│   ├── Marte.Application.dll
│   ├── Marte.Domain.dll
│   ├── Marte.Infrastructure.dll
│   ├── Images/                         # Recursos visuales
│   │   └── Marte-Ico.ico
│   └── [dependencias...]               # DLLs de NuGet
│
├── INSTALAR-MARTE.bat                  # Wrapper de instalación
├── DESINSTALAR-MARTE.bat               # Wrapper de desinstalación
├── install-marte.ps1                   # Instalador automático (formato ASCII)
├── uninstall-marte.ps1                 # Desinstalador con backup (formato ASCII)
├── RELEASE-NOTES-v1.3.0.md             # Notas del release
└── VERSION.txt                         # Metadatos de versión
```

### Tamaño del Paquete
- **MARTE-Installer-v1.3.0-FINAL.zip**: 52.41 MB (incluye todos los binarios y dependencias)

---

## 🛠️ Requisitos del Sistema

### Mínimos
- **OS**: Windows 10 (64-bit)
- **RAM**: 2 GB
- **Disco**: 500 MB libres
- **Resolución**: 1024x768

### Recomendados
- **OS**: Windows 11 (64-bit)
- **RAM**: 4 GB o más
- **Disco**: 1 GB libres
- **Resolución**: 1920x1080

### Dependencias (instaladas automáticamente)
- ✅ .NET 9.0 Desktop Runtime
- ✅ SQL Server LocalDB 2022

---

## 🔮 Roadmap Futuro

### v1.4.0 (Próxima versión - Q4 2025)
- [ ] Gráficos interactivos en Dashboard (ChartJS/LiveCharts)
- [ ] Modo offline con sincronización
- [ ] Tema claro/oscuro personalizable
- [ ] Exportación automática de backups programados

### v2.0.0 (2026)
- [ ] API REST para integración externa
- [ ] Aplicación móvil (.NET MAUI)
- [ ] Sincronización cloud (Azure/AWS)
- [ ] Reconocimiento facial para asistencias

---

## 👥 Créditos

### Equipo de Desarrollo
- **Desarrollador Principal**: Andy Agurto Urcia - Ingeniero de Sistemas
- **Organización**: AU Developers (MVP)
- **Cliente**: Nueva Acrópolis

### Tecnologías Open Source
- .NET 9.0 - Microsoft
- Entity Framework Core 9.0.10 - Microsoft
- BCrypt.Net-Next 4.0.3
- ClosedXML 0.105.0
- QuestPDF 2025.7.3
- xUnit 2.9.2

---

## 📞 Soporte

### Canales de Soporte
- **Email**: druagurto@hotmail.com
- **GitHub Issues**: https://github.com/AndyAgurto/Proyecto-Marte/issues
- **Repositorio**: https://github.com/AndyAgurto/Proyecto-Marte

### Documentación
- **README.md**: Guía rápida de inicio
- **INSTALADOR-FINAL-README.md**: Guía completa del instalador final
- **README-COMPLETO.md**: Documentación técnica extendida
- **DOCUMENTACION_TECNICA_GENERAL.md**: Arquitectura y módulos
- **GUIA_USUARIO_MARTE.md**: Manual de usuario completo

---

## 📄 Licencia

MIT License

Copyright (c) 2025 Andy Agurto - AU Developers (MVP)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.

---

## 🎉 Agradecimientos

Gracias a:
- **Nueva Acrópolis** por la confianza y el apoyo al proyecto
- **Comunidad .NET** por las excelentes herramientas y librerías
- **Microsoft** por .NET 9.0 y el ecosistema de desarrollo moderno

---

**¡Disfruta de MARTE v1.3.0!** 🚀

Para reportar bugs o sugerir mejoras, por favor abre un issue en GitHub.

---

*Generado el 18 de Octubre, 2025*  
*Sistema MARTE - Gestión de Asistencias para Nueva Acrópolis*
