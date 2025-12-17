# Notas de Versión - MARTE v1.4.0

**Fecha de lanzamiento:** 17 de Diciembre, 2025  
**Tipo de versión:** Actualización Mayor

---

## 🎉 Nuevas Características

### Reporte Jerárquico Mejorado
- **Detalle completo por asistente**: El reporte jerárquico ahora muestra todos los asistentes individuales dentro de cada categoría, similar al formato de "Totales Diarios"
- **Información detallada**: Incluye DNI, Nombre Completo, Grupo, Fecha, Hora Ingreso, Hora Salida, Hasta Cierre y Observación
- **12 niveles jerárquicos**: Organización completa desde Jefe de Filial hasta Otros
- **Identificación Fuerzas Vivas**: Agrupación especial para G de S, GG.FF, GG.MM

### Exportación PDF Unificada
- **Estilo consistente**: Todos los reportes PDF ahora siguen el mismo formato profesional
- **Header mejorado**: Título, período, fecha de generación y datos adicionales
- **Footer con paginación**: Indicador de "Página X de Y" en todos los reportes
- **Formato landscape**: Reporte jerárquico en formato horizontal para mejor visualización

---

## 🔧 Mejoras

### Sistema de Instalación
- **Detección automática de actualizaciones**: El instalador detecta versiones previas instaladas
- **Preservación de datos**: La base de datos se respalda y restaura automáticamente durante actualizaciones
- **Backup automático**: Copia de seguridad temporal antes de actualizar
- **Mensaje diferenciado**: Información específica para instalación nueva vs actualización

### Optimización de Código
- **Métodos no utilizados eliminados**: Se removieron 4 métodos PDF que no se estaban usando:
  - `ExportarPuntualidadPDFAsync`
  - `ExportarHastaCierrePDFAsync`
  - `ExportarAsistenciaPorGrupoPDFAsync`
  - `ExportarTopConstantesPDFAsync`
- **~240 líneas de código eliminadas**: Mejora en mantenibilidad y rendimiento
- **Repositorio optimizado**: Método `GetByFechaRangoAsync` agregado para consultas eficientes

---

## 📊 Cambios Técnicos

### Base de Datos
- Nuevo método en `IAsistenciaRepository`: `GetByFechaRangoAsync`
- Implementación en `AsistenciaRepository` con ordenamiento optimizado
- Include optimizado de relaciones (Asistente → Categoria)

### Servicios
- **ReporteService**: Lógica mejorada para reportes jerárquicos detallados
- **Método auxiliar**: `ObtenerCategoriaJerarquica` para mapeo de categorías
- **Diccionario de jerarquía**: Soporte para múltiples variantes de nombres por categoría

### Documentos PDF
- Framework: QuestPDF 2025.7.3
- Fuente: Arial
- Tamaño de página: A4 Landscape para reportes detallados
- Márgenes: 1 cm consistente
- Tamaño de fuente: 8pt (datos), 7pt (tablas), 16pt (títulos)

---

## 📝 Archivos Modificados

### Aplicación
- `Marte.Application/Services/ReporteService.cs`
- `Marte.Application/Interfaces/IReporteService.cs`
- `Marte.Infrastructure/Repositories/AsistenciaRepository.cs`
- `Marte.Infrastructure/Interfaces/IAsistenciaRepository.cs`

### Instalador
- `install-marte.ps1` (v1.4.0)
- `MARTE-Installer/install-marte.ps1`
- `MARTE-Installer/VERSION.txt`

---

## 🚀 Instrucciones de Actualización

### Para Usuarios con Instalación Previa

1. **Cerrar la aplicación** si está en ejecución
2. **Ejecutar como administrador**: `install-marte.ps1`
3. El instalador detectará automáticamente la versión anterior
4. La base de datos se preservará automáticamente
5. Los archivos de programa se actualizarán
6. Lanzar la aplicación desde el acceso directo

**Nota**: No es necesario desinstalar la versión anterior. El instalador se encarga de todo.

### Para Nuevos Usuarios

1. Ejecutar como administrador: `install-marte.ps1`
2. El instalador verificará e instalará:
   - .NET 9 Runtime
   - SQL Server LocalDB 2022
3. Credenciales por defecto:
   - **Usuario**: druagurto
   - **Contraseña**: @Druagurto00

---

## ⚠️ Notas Importantes

- **Backup de seguridad**: Durante la actualización, se crea un backup temporal en `%TEMP%\MarteDB_backup_[fecha]`
- **Compatibilidad**: Compatible con bases de datos de v1.3.0
- **Requisitos**: Windows 10/11, .NET 9 Runtime, SQL Server LocalDB

---

## 🐛 Correcciones de Errores

- Corregido: Error de sintaxis en exportación PDF de categorías jerárquicas
- Corregido: Verificación de nulidad en navegación de entidades
- Corregido: Ordenamiento inconsistente en reportes detallados

---

## 📞 Soporte

**Desarrollador:** Andy Agurto Urcia  
**Organización:** AU Developers (MVP)  
**Email:** druagurto@hotmail.com  
**Repositorio:** https://github.com/AndyAgurto/Proyecto-Marte

---

## 📜 Licencia

MIT License - Copyright © 2025 Andy Agurto
