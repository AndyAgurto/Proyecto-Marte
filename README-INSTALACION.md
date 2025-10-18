# Sistema MARTE - Guía de Instalación

**Versión**: 1.3.0  
**Fecha**: Octubre 2025

---

## 📋 Requisitos del Sistema

- **Sistema Operativo**: Windows 10 o Windows 11 (64 bits)
- **Espacio en Disco**: Mínimo 500 MB libres
- **RAM**: Mínimo 2 GB (recomendado 4 GB)
- **Resolución de Pantalla**: Mínimo 1024x768

⚠️ **NOTA**: No se requiere tener SQL Server instalado previamente. El instalador se encarga de todo.

---

## 🚀 Instalación Automática

1. **Descargar** el paquete `MARTE-Installer.zip`

2. **Descomprimir** en cualquier carpeta temporal

3. **Click derecho** en `install-marte.ps1`

4. Seleccionar **"Ejecutar con PowerShell"** o **"Ejecutar como administrador"**

5. **Seguir las instrucciones** en pantalla

6. **Esperar** aproximadamente 2-3 minutos (dependiendo de la conexión a internet)

7. Al finalizar, encontrará el icono de **MARTE** en su escritorio

⚠️ **NOTA**: El instalador se encarga automáticamente de:
- Instalar .NET 9 Runtime (si no está instalado)
- Instalar SQL Server LocalDB (si no está instalado)
- Copiar archivos de aplicación
- Crear accesos directos

---

## 🔧 Primera Ejecución

1. **Ejecutar MARTE** desde el icono del escritorio

2. En la **primera ejecución**, verá un mensaje:
   ```
   Esta es la primera ejecución de MARTE.
   Se creará la base de datos y se cargarán los datos iniciales.
   ```

3. Hacer clic en **"Sí"** para continuar

4. Esperar mientras se crea la base de datos (30-60 segundos)

5. Verá un mensaje de confirmación:
   ```
   ✅ Base de datos inicializada correctamente.
   
   Usuario por defecto:
   Usuario: druagurto
   Contraseña: @Druagurto00
   ```

6. Hacer clic en **"Aceptar"**

7. Aparecerá la pantalla de **Login**

8. Ingresar las credenciales:
   - **Usuario**: `druagurto`
   - **Contraseña**: `@Druagurto00`

9. **¡Listo!** Ya puede comenzar a usar MARTE

---

## 🔐 Seguridad Importante

⚠️ **CAMBIAR LA CONTRASEÑA INMEDIATAMENTE**

Después del primer inicio de sesión:

1. Ir a **Configuración** → **Gestión de Usuarios**
2. Seleccionar el usuario `druagurto`
3. Hacer clic en **"Cambiar Contraseña"**
4. Ingresar una contraseña segura nueva
5. Guardar los cambios

**Requisitos de contraseña segura**:
- Mínimo 8 caracteres
- Al menos 1 mayúscula
- Al menos 1 minúscula
- Al menos 1 número
- Al menos 1 carácter especial (@, #, $, %, etc.)

---

## 📁 Ubicación de Archivos

### Archivos de Aplicación
```
C:\Program Files\MARTE\
├── Marte.WPF.exe           (Ejecutable principal)
├── Marte.Application.dll
├── Marte.Infrastructure.dll
├── Marte.Domain.dll
└── ... (otros archivos)
```

### Base de Datos
```
C:\Users\[TuUsuario]\AppData\Local\MARTE\Data\
└── MarteDb.mdf             (Base de datos SQLite)
└── MarteDb_log.ldf         (Archivo de log)
```

⚠️ **IMPORTANTE**: Hacer backup periódico de la carpeta `Data`

---

## 🔄 Actualización

Para actualizar MARTE a una nueva versión:

1. **Cerrar** la aplicación MARTE si está abierta

2. **Hacer backup** de la base de datos:
   - Copiar la carpeta: `C:\Users\[TuUsuario]\AppData\Local\MARTE\Data`
   - Guardar en un lugar seguro

3. **Ejecutar** el nuevo instalador (install-marte.ps1)

4. Cuando pregunte si desea sobrescribir, responder **"S"** (Sí)

5. La base de datos NO se perderá (se mantiene en AppData)

6. **Ejecutar MARTE** y verificar que todo funciona

---

## 🆘 Solución de Problemas

### Problema: "Error al iniciar la aplicación"

**Solución**:
1. Verificar que .NET 9 Runtime está instalado:
   ```powershell
   dotnet --version
   ```
   Debe mostrar: `9.0.x`

2. Si no está instalado, descargar de: https://dotnet.microsoft.com/download/dotnet/9.0

---

### Problema: "No se puede conectar a la base de datos"

**Solución**:
1. Verificar que SQL Server LocalDB está instalado:
   ```powershell
   sqllocaldb info
   ```
   Debe mostrar: `mssqllocaldb`

2. Si no aparece, crear la instancia:
   ```powershell
   sqllocaldb create mssqllocaldb
   sqllocaldb start mssqllocaldb
   ```

---

### Problema: "El script está deshabilitado en este sistema"

**Solución**:
Ejecutar PowerShell como Administrador y escribir:
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

Luego intentar ejecutar `install-marte.ps1` nuevamente.

---

### Problema: "Credenciales inválidas" al hacer login

**Verificar**:
- Usuario: `druagurto` (todo en minúsculas)
- Contraseña: `@Druagurto00` (exactamente como se muestra, respetando mayúsculas)

---

## 📞 Soporte Técnico

Si experimenta problemas no listados aquí:

1. **Revisar** el archivo de log en:
   ```
   C:\Users\[TuUsuario]\AppData\Local\MARTE\Logs\
   ```

2. **Contactar** a soporte técnico con:
   - Descripción del problema
   - Mensaje de error (captura de pantalla)
   - Contenido del archivo de log más reciente

---

## 🗑️ Desinstalación Automática

1. **Localizar** el script `uninstall-marte.ps1` (incluido en el paquete de instalación)

2. **Click derecho** en `uninstall-marte.ps1`

3. Seleccionar **"Ejecutar con PowerShell"** o **"Ejecutar como administrador"**

4. **Seguir las instrucciones** en pantalla:
   - Confirmación de desinstalación
   - Opción de crear backup de base de datos
   - Eliminación de archivos de aplicación
   - Eliminación de accesos directos
   - Opción de eliminar datos de aplicación (base de datos)
   - Opción de desinstalar SQL Server LocalDB

5. **Completar** el proceso siguiendo las indicaciones

**El desinstalador incluye**:
- ✅ Cierre automático de MARTE si está en ejecución
- ✅ Backup opcional de base de datos antes de desinstalar
- ✅ Eliminación de archivos de aplicación
- ✅ Eliminación de accesos directos
- ✅ Opción de conservar o eliminar datos
- ✅ Instrucciones para desinstalar LocalDB (si se desea)

---

### Reinstalación Después de Desinstalar

Si desea reinstalar MARTE después de desinstalar:

#### Con Datos Conservados

Si conservó los datos de aplicación (`%LocalAppData%\MARTE`):

1. Ejecutar `install-marte.ps1` nuevamente
2. Los datos se mantendrán automáticamente
3. Login con las credenciales existentes

#### Con Datos Eliminados

Si eliminó los datos de aplicación:

1. Ejecutar `install-marte.ps1`
2. Se creará una nueva base de datos
3. Login con credenciales por defecto: `druagurto / @Druagurto00`

#### Restaurar desde Backup

Si tiene un backup y desea restaurarlo:

1. Instalar MARTE ejecutando `install-marte.ps1`
2. **Cerrar** MARTE si está abierto
3. Abrir el Explorador de Windows
4. Navegar a `%LocalAppData%\MARTE\Data`
5. **Eliminar** el contenido de la carpeta `Data`
6. **Copiar** los archivos del backup a la carpeta `Data`:
   - `MarteDb.mdf`
   - `MarteDb_log.ldf`
7. Ejecutar MARTE
8. Login con las credenciales que tenía antes

---

## 🔄 Actualización

### Actualizar a una Nueva Versión

Para actualizar MARTE a una versión más reciente:

#### Paso 1: Verificar Versión Actual

1. Abrir MARTE
2. Ver la versión en la pantalla principal o en "Acerca de"
3. Cerrar MARTE completamente

---

#### Paso 2: Backup de Seguridad (IMPORTANTE)

Antes de actualizar, siempre crear un backup:

1. Presionar **Win + R**
2. Escribir: `%LocalAppData%\MARTE\Data`
3. Copiar la carpeta `Data` completa al escritorio
4. Renombrar como: `MARTE_Backup_Antes_Actualizacion`

---

#### Paso 3: Actualización Automática

1. Descargar el nuevo paquete `MARTE-Installer-vX.X.X.zip`
2. Descomprimir en una carpeta temporal
3. **Click derecho** en `install-marte.ps1`
4. Ejecutar como administrador
5. Cuando pregunte **"¿Desea sobrescribir?"**, responder **"S"** (Sí)
6. Esperar a que complete la actualización
7. Ejecutar MARTE
8. Verificar que funciona correctamente
9. **La base de datos se mantiene automáticamente** ✅

**Ventajas**:
- ✅ Proceso automático
- ✅ Base de datos se conserva
- ✅ Configuraciones se mantienen
- ✅ No requiere desinstalar primero

---

#### Paso 4: Aplicar Migraciones de Base de Datos (Si es Necesario)

Algunas actualizaciones pueden requerir cambios en la base de datos:

1. **Ejecutar MARTE** por primera vez después de actualizar
2. Si hay migraciones pendientes, verá un mensaje:
   ```
   Se detectaron actualizaciones de base de datos.
   ¿Desea aplicar las actualizaciones?
   ```
3. Click en **"Sí"** para aplicar migraciones automáticamente
4. Esperar a que complete (normalmente 5-30 segundos)
5. Ver mensaje de confirmación
6. Continuar usando MARTE normalmente

**NOTA**: Las migraciones son seguras y no eliminan datos. Solo actualizan la estructura de la base de datos.

---

#### Paso 5: Verificación Post-Actualización

Después de actualizar, verificar:

- [ ] MARTE inicia correctamente
- [ ] Login funciona con credenciales existentes
- [ ] Asistentes registrados siguen presentes
- [ ] Historial de asistencias está intacto
- [ ] Nuevas funciones están disponibles (si aplica)
- [ ] Reportes se generan correctamente

Si algo no funciona:
1. Cerrar MARTE
2. Restaurar desde backup (ver sección de Restauración)
3. Contactar soporte técnico

---

### Actualización de Emergencia (Rollback)

Si después de actualizar MARTE presenta problemas:

#### Restaurar Base de Datos desde Backup

1. Cerrar MARTE completamente
2. Presionar **Win + R** → `%LocalAppData%\MARTE\Data`
3. Eliminar:
   - `MarteDb.mdf`
   - `MarteDb_log.ldf`
4. Copiar los archivos del backup `MARTE_Backup_Antes_Actualizacion`:
   - `MarteDb.mdf` → Copiar a `Data`
   - `MarteDb_log.ldf` → Copiar a `Data`
5. Ejecutar MARTE

---

#### Reinstalar Versión Anterior

1. Ejecutar `uninstall-marte.ps1` (con opción de backup)
2. Instalar la versión anterior con `install-marte.ps1`
3. Restaurar base de datos desde backup (si es necesario)

---

### Historial de Versiones

| Versión | Fecha | Cambios Principales |
|---------|-------|---------------------|
| **1.3.0** | Oct 2025 | • LocalDB implementation<br>• Instalación automática<br>• Mejoras en dashboard |
| **1.2.0** | Oct 2025 | • Sistema de reportes<br>• Exportación Excel/PDF |
| **1.1.0** | Oct 2025 | • Gestión de asistencias<br>• Control de asistencias |
| **1.0.0** | Oct 2025 | • Versión inicial<br>• Login y usuarios |

---

### Notas de Actualización por Versión

#### Actualizar de 1.2.0 a 1.3.0

**Cambios importantes**:
- ✅ Migración de SQL Server a LocalDB
- ✅ No requiere cambios de configuración
- ✅ Base de datos se migra automáticamente
- ⚠️ **IMPORTANTE**: Crear backup antes de actualizar

**Pasos específicos**:
1. Backup de base de datos (obligatorio)
2. Ejecutar nuevo instalador
3. En primera ejecución, se migrará la BD automáticamente
4. Verificar que todos los datos estén presentes

---

#### Actualizar de 1.1.0 a 1.2.0

**Cambios importantes**:
- ✅ Nuevas tablas para reportes
- ✅ Migraciones automáticas aplicadas
- ✅ Sin pérdida de datos

**Pasos específicos**:
1. Actualizar archivos de aplicación
2. Ejecutar MARTE
3. Aceptar aplicación de migraciones
4. Verificar módulo de reportes

---

### Frecuencia de Actualizaciones

- **Actualizaciones de Seguridad**: Aplicar inmediatamente cuando estén disponibles
- **Actualizaciones de Funcionalidad**: Opcional, según necesidades
- **Actualizaciones Menores (bugs)**: Recomendado aplicar mensualmente

---

### Descargas de Versiones

Para descargar versiones de MARTE:

1. **Última versión estable**: Contactar con soporte técnico
2. **Versiones anteriores**: Disponibles bajo solicitud para rollback
3. **Beta versions**: Solo para testing, no usar en producción

---

## ℹ️ Información Adicional

**Desarrollador**: Andy Agurto Urcia - Ing. de Sistemas  
**Versión**: 1.3.0 BETA  
**Fecha de Lanzamiento**: Octubre 2025  
**Framework**: .NET 9.0  
**Base de Datos**: SQL Server LocalDB  

---

## 📄 Licencia

Este software es propiedad de AU Developers (MVP).  
Todos los derechos reservados.

---

**¡Gracias por usar MARTE!** 🚀
