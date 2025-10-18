# 📦 Guía para Crear Release en GitHub

## Pasos para Publicar MARTE v1.3.0 en GitHub

### 1. Acceder a la Página de Releases

1. Ir a: https://github.com/AndyAgurto/Proyecto-Marte
2. Click en **"Releases"** (en sidebar derecho)
3. Click en **"Draft a new release"** o **"Create a new release"**

---

### 2. Configurar el Release

#### Tag Version
- **Choose a tag**: `v1.3.0` (ya creado y pusheado)
- **Target**: `main` branch

#### Release Title
```
🚀 MARTE v1.3.0 - LocalDB Revolution
```

#### Release Description

Copiar el siguiente contenido en la descripción:

```markdown
# 🚀 MARTE v1.3.0 - "LocalDB Revolution"

**Fecha de Lanzamiento**: 18 de Octubre, 2025  
**Estado**: Stable Release

---

## 📋 Resumen

Esta versión transforma MARTE en una aplicación **Zero Configuration** con instalación automática completa. Los usuarios finales ya no necesitan conocimientos técnicos para instalar y usar el sistema.

## ✨ Características Principales

### 🔧 Instalación Automática
- ✅ Script PowerShell inteligente (`install-marte.ps1`)
- ✅ Detección e instalación automática de .NET 9 Runtime
- ✅ Detección e instalación automática de SQL Server LocalDB
- ✅ Creación de accesos directos (Desktop y Menú Inicio)
- ✅ Proceso completamente guiado

### 💾 Base de Datos Portable
- ✅ SQL Server LocalDB embebido
- ✅ Ubicación: `%LocalAppData%\MARTE\Data\`
- ✅ Primera ejecución con setup automático
- ✅ Migraciones auto-aplicadas
- ✅ Seed de datos iniciales incluido

### 🎨 Ventana "Acerca de" (NUEVO)
- ✅ Modal profesional con información del sistema
- ✅ Versión, desarrollador, tecnologías, licencia
- ✅ Links clickeables (email y GitHub)
- ✅ Diseño con colores institucionales

### 📚 Documentación Completa
- ✅ `README-INSTALACION.md` - Guía de usuario
- ✅ `README-COMPLETO.md` - Documentación técnica (1,652 líneas)
- ✅ `RELEASE-NOTES-v1.3.0.md` - Notas detalladas
- ✅ `CHANGELOG.md` - Historial completo

## 📊 Estadísticas del Release

- **Archivos modificados**: 8
- **Archivos nuevos**: 9
- **Líneas agregadas**: ~2,500
- **Commits**: 6
- **Tests pasando**: 63/63 (100%)
- **Documentación**: ~3,000 líneas

## 🔄 Actualización desde v1.2.0

```bash
# 1. Backup
Copy-Item "%LocalAppData%\MARTE\Data" "Desktop\Backup"

# 2. Descargar y ejecutar instalador
.\install-marte.ps1

# 3. Responder "S" para sobrescribir

# 4. ¡Listo! Base de datos se migra automáticamente
```

## 📦 Archivos del Release

Ver sección **Assets** abajo para descargar:
- `MARTE-Installer-v1.3.0.zip` - Instalador completo
- `Source code (zip)` - Código fuente
- `Source code (tar.gz)` - Código fuente

## 🛠️ Requisitos del Sistema

- **OS**: Windows 10/11 (64-bit)
- **RAM**: 2 GB mínimo (4 GB recomendado)
- **Disco**: 500 MB libres
- **Resolución**: 1024x768 mínimo

### Dependencias (instaladas automáticamente)
- .NET 9.0 Desktop Runtime
- SQL Server LocalDB 2022

## 🎯 Breaking Changes

**Ninguno** - 100% compatible hacia atrás con v1.2.0

## 📚 Documentación

- [README-INSTALACION.md](README-INSTALACION.md) - Guía de instalación
- [README-COMPLETO.md](README-COMPLETO.md) - Documentación técnica
- [RELEASE-NOTES-v1.3.0.md](RELEASE-NOTES-v1.3.0.md) - Notas completas
- [CHANGELOG.md](CHANGELOG.md) - Historial de versiones

## 🔒 Seguridad

- ✅ Base de datos en carpeta de usuario (segura)
- ✅ BCrypt para encriptación de contraseñas
- ✅ Auditoría completa de acciones
- ⚠️ **IMPORTANTE**: Cambiar contraseña por defecto después del primer inicio

## 📞 Soporte

- **Email**: druagurto@hotmail.com
- **Issues**: [GitHub Issues](https://github.com/AndyAgurto/Proyecto-Marte/issues)
- **Repositorio**: [GitHub](https://github.com/AndyAgurto/Proyecto-Marte)

## 👨‍💻 Créditos

**Desarrollador**: Andy Agurto Urcia - Ingeniero de Sistemas  
**Organización**: AU Developers (MVP)  
**Cliente**: Nueva Acrópolis  
**Licencia**: MIT License

---

## 🎉 ¡Gracias por usar MARTE!

Sistema desarrollado con ❤️ usando .NET 9.0

---

**Nota**: Este es un release estable y listo para producción. Para reportar bugs o sugerir mejoras, por favor abre un [Issue](https://github.com/AndyAgurto/Proyecto-Marte/issues).
```

---

### 3. Subir Assets (Archivos del Release)

#### Crear Paquete de Distribución

Antes de publicar el release, necesitas crear el paquete:

```powershell
# Ejecutar desde la raíz del proyecto
.\crear-paquete-distribucion.ps1
```

Esto creará la carpeta `MARTE-Installer/` con todos los archivos.

#### Comprimir el Paquete

```powershell
# Comprimir carpeta a ZIP
Compress-Archive -Path "MARTE-Installer\*" -DestinationPath "MARTE-Installer-v1.3.0.zip"
```

#### Subir a GitHub

En la página de creación del release, en la sección **"Attach binaries"**:

1. Click en **"Attach binaries by dropping them here or selecting them"**
2. Seleccionar `MARTE-Installer-v1.3.0.zip`
3. Esperar a que se suba completamente

---

### 4. Configuración Adicional

#### Marcar como Release
- ✅ Check **"Set as the latest release"**
- ❌ No marcar como pre-release (es stable)

#### Generar Release Notes Automáticas
- ✅ Puedes hacer click en **"Generate release notes"** para incluir lista de commits
- Luego editar para mantener solo el formato personalizado arriba

---

### 5. Publicar

1. Revisar toda la información
2. Click en **"Publish release"**
3. ¡Listo! El release estará público en:
   ```
   https://github.com/AndyAgurto/Proyecto-Marte/releases/tag/v1.3.0
   ```

---

## 📋 Checklist Pre-Publicación

Antes de publicar, verificar:

- [ ] Tag `v1.3.0` existe y está pusheado
- [ ] Código compilado sin errores
- [ ] Todos los tests pasando (63/63)
- [ ] Documentación actualizada
- [ ] `VERSION.txt` tiene la versión correcta
- [ ] `CHANGELOG.md` incluye v1.3.0
- [ ] `RELEASE-NOTES-v1.3.0.md` completo
- [ ] Paquete `MARTE-Installer-v1.3.0.zip` creado
- [ ] Release notes copiados en GitHub

---

## 📝 Notas Adicionales

### Después de Publicar

1. **Anunciar el release**:
   - Enviar email a stakeholders
   - Notificar a Nueva Acrópolis
   - Actualizar documentación interna

2. **Monitorear issues**:
   - Revisar GitHub Issues por reportes de bugs
   - Responder preguntas de instalación

3. **Preparar hotfix si necesario**:
   - Si hay bugs críticos, preparar v1.3.1

### Promoción del Release

#### Badge para README.md
```markdown
[![Release](https://img.shields.io/github/v/release/AndyAgurto/Proyecto-Marte)](https://github.com/AndyAgurto/Proyecto-Marte/releases/latest)
```

#### Link directo de descarga
```
https://github.com/AndyAgurto/Proyecto-Marte/releases/download/v1.3.0/MARTE-Installer-v1.3.0.zip
```

---

## 🎯 Próximos Pasos

Después de publicar v1.3.0:

1. **Monitoreo** (1-2 semanas):
   - Feedback de usuarios
   - Reportes de bugs
   - Métricas de instalación

2. **Hotfixes** (si necesario):
   - Versión v1.3.1 para bugs críticos

3. **Planificación v1.4.0**:
   - Gráficos interactivos
   - Modo offline
   - Tema personalizable

---

**¡Éxito con el release! 🚀**
