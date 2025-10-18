# Guía de Usuario - Sistema MARTE

**Sistema**: MARTE - Sistema de Gestión de Asistencias para Nueva Acrópolis  
**Versión**: 1.3 BETA  
**Fecha**: 18 de octubre de 2025  
**Última actualización**: 18 de octubre de 2025  
**Audiencia**: Usuarios finales (Administradores y Guardias)

---

## Tabla de Contenidos

1. [Introducción al Sistema](#introducción-al-sistema)
2. [Inicio de Sesión](#inicio-de-sesión)
3. [Ventana Principal](#ventana-principal)
4. [Módulo: Gestión de Usuarios](#módulo-gestión-de-usuarios)
5. [Módulo: Configuración del Sistema](#módulo-configuración-del-sistema)
6. [Módulo: Gestión de Categorías](#módulo-gestión-de-categorías)
7. [Módulo: Gestión de Asistentes](#módulo-gestión-de-asistentes)
8. [Módulo: Control de Asistencia](#módulo-control-de-asistencia)
9. [Módulo: Reportes y Estadísticas](#módulo-reportes-y-estadísticas)
10. [Módulo: Bitácora de Auditoría](#módulo-bitácora-de-auditoría) ⭐ **NUEVO**
11. [Cerrar Sesión](#cerrar-sesión)
12. [Solución de Problemas Comunes](#solución-de-problemas-comunes)
13. [Buenas Prácticas](#buenas-prácticas)
14. [Glosario de Términos](#glosario-de-términos)

---

## Introducción al Sistema

### ¿Qué es MARTE?

**MARTE** (Sistema de Gestión de Asistencias) es una aplicación diseñada específicamente para Nueva Acrópolis que permite:

- 📋 Registrar y gestionar información de asistentes a actividades
- ✅ Controlar asistencias diarias (entradas y salidas)
- 📊 Generar reportes y estadísticas de participación
- 👥 Administrar usuarios del sistema con roles y permisos
- 🔧 Configurar parámetros del sistema (hora de cierre, categorías)
- 🔍 Auditar todas las operaciones realizadas

### Roles de Usuario

El sistema MARTE reconoce dos tipos principales de usuarios:

| Rol | Permisos | Funciones Principales |
|-----|----------|----------------------|
| **Administrador** | Acceso completo | Gestión de usuarios, gestión de asistentes, configuración del sistema, todos los módulos |
| **Guardia** | Acceso operativo | Registro de asistencia, consulta de asistentes, reportes básicos |

---

## Inicio de Sesión

### Acceder a MARTE

1. **Abrir la aplicación** MARTE desde el escritorio o menú de inicio
2. Aparecerá la ventana de **Login**

```
┌─────────────────────────────────────┐
│         🔐 ACCESO MARTE             │
├─────────────────────────────────────┤
│                                     │
│  Usuario:    [_______________]      │
│                                     │
│  Contraseña: [***************]      │
│                                     │
│         [INICIAR SESIÓN]           │
│                                     │
└─────────────────────────────────────┘
```

3. **Ingresar credenciales**
   - Usuario: Su nombre de usuario asignado
   - Contraseña: Su contraseña personal

4. **Hacer clic en "INICIAR SESIÓN"**

### Mensajes al Iniciar Sesión

✅ **Inicio exitoso**: Se abrirá la ventana principal automáticamente

❌ **Credenciales inválidas**:
```
╔══════════════════════════════════════╗
║          ⚠️  ERROR                   ║
╠══════════════════════════════════════╣
║ Usuario o contraseña incorrectos    ║
║                                      ║
║ Por favor, verifique sus            ║
║ credenciales e intente nuevamente.  ║
║                                      ║
║           [ACEPTAR]                  ║
╚══════════════════════════════════════╝
```

❌ **Usuario deshabilitado**:
```
╔══════════════════════════════════════╗
║          ⚠️  ACCESO DENEGADO         ║
╠══════════════════════════════════════╣
║ Su usuario está deshabilitado.      ║
║                                      ║
║ Contacte al administrador del       ║
║ sistema para más información.       ║
║                                      ║
║           [ACEPTAR]                  ║
╚══════════════════════════════════════╝
```

### Credenciales Iniciales (Administrador)

**Para desarrollo/primera instalación**:
- Usuario: `druagurto`
- Contraseña: `@Druagurto00`

⚠️ **Importante**: Cambie estas credenciales después del primer inicio de sesión en producción.

---

## Ventana Principal

### Descripción General

Al iniciar sesión exitosamente, verá la ventana principal de MARTE con la siguiente estructura:

```
┌───────────────────────────────────────────────────────────┐
│  MARTE    [Logo]                    Usuario: jperez       │
│                                     [Cerrar Sesión]       │
├─────────────────┬─────────────────────────────────────────┤
│                 │                                         │
│  📋 GESTIÓN     │       📊 DASHBOARD PRINCIPAL           │
│  - Asistentes  │                                         │
│                 │   ┌─────────────────────────────┐     │
│  ✅ CONTROL     │   │ Indicadores del Día         │     │
│  - Asistencia  │   │                              │     │
│                 │   │ Total Asistentes Hoy: 45    │     │
│  📊 REPORTES    │   │ Entradas: 45                │     │
│  - Estadísticas│   │ Salidas: 32                  │     │
│                 │   └─────────────────────────────┘     │
│  ⚙️ CONFIG      │                                         │
│  - Configurar  │   ┌─────────────────────────────┐     │
│  - Auditoría   │   │ Por Categoría               │     │
│                 │   │ - Miembros: 20              │     │
│                 │   │ - GG.FF: 10                 │     │
│                 │   │ - Visitas: 15               │     │
│                 │   └─────────────────────────────┘     │
└─────────────────┴─────────────────────────────────────────┘
│  Fecha: 18/10/2025 15:30:45        Sistema: MARTE v1.0  │
└───────────────────────────────────────────────────────────┘
```

### Componentes de la Ventana Principal

#### 1. Barra Superior
- **Logo y Título**: Identificación del sistema
- **Usuario Actual**: Muestra su nombre de usuario
- **Botón Cerrar Sesión**: Para salir del sistema de forma segura

#### 2. Sidebar (Panel Lateral Izquierdo)
Módulos disponibles según su rol:
- 📋 **Gestión de Asistentes** ✅ Implementado
- ✅ **Control de Asistencia** ✅ Implementado
- 📊 **Reportes y Estadísticas** ✅ Implementado
- 🔍 **Bitácora de Auditoría** ✅ Implementado
- ⚙️ **Configuración** ✅ Implementado (Administradores solamente)

#### 3. Dashboard Central

El dashboard muestra información en tiempo real con las siguientes secciones:

##### 📊 **Indicadores Principales**

1. **Asistentes Hoy**: Total de asistentes que ingresaron el día actual
2. **Hasta Cierre Ayer**: Asistentes que permanecieron hasta la hora de cierre el día anterior
3. **Puntualidad**: Porcentaje de asistentes puntuales
4. **Total Registrados**: Cantidad total de asistentes activos en el sistema

##### 📋 **Asistentes por Categoría (Selector Dinámico)** ⭐ NUEVO

Permite visualizar el total de asistentes filtrado por categoría:

**Controles**:
- **ComboBox de Categoría**: Seleccione la categoría deseada
  - Opción "Todas las categorías": Muestra el total general
  - Categorías específicas: Miembros, GG.FF, Visitas, etc.

- **ComboBox de Grupo** (solo visible para "Miembros"):
  - Aparece automáticamente al seleccionar la categoría "Miembros"
  - Muestra los grupos que realmente existen en la base de datos
  - Formato: "Grupo #01", "Grupo #05", etc.
  - Los grupos se cargan dinámicamente según los asistentes registrados

**Ejemplo de uso**:
```
Categoría: [Miembros ▼]        Grupo: [Grupo #03 ▼]
         
          Miembros Grupo #03
               12
            asistentes
```

**Ventajas**:
- ✅ Carga dinámica de grupos desde la base de datos
- ✅ Solo muestra grupos que tienen asistentes
- ✅ Actualización automática al cambiar categoría
- ✅ Interfaz intuitiva con colores del tema institucional

##### 🏆 **Top 5 Más Constantes**

Muestra los 5 asistentes con mejor porcentaje de asistencia:
- **Puesto**: Posición en el ranking (1° a 5°)
- **Nombre Completo**: Apellidos y nombres
- **Asistencias**: Días asistidos / Total de días disponibles
- **Porcentaje**: Porcentaje de asistencia

**Nota**: Los datos se cargan automáticamente desde la base de datos. Si no hay registros, se muestra el mensaje "No existen registros actualmente".

#### 4. Barra Inferior
- Fecha y hora actual
- Versión del sistema
- Estado de conexión

---

## Módulo: Gestión de Usuarios

### Acceso al Módulo

**Requisito**: Rol de **Administrador**

1. En la ventana principal, clic en **Configuración** en el sidebar
2. Seleccionar **"Gestión de Usuarios"**
3. Se abrirá una nueva ventana

### Interfaz de Gestión de Usuarios

```
┌────────────────────────────────────────────────────────────┐
│                  GESTIÓN DE USUARIOS                        │
├────────────────────────────────────────────────────────────┤
│                                                             │
│  ┌───────────────────────────────────────────────────┐    │
│  │ LISTA DE USUARIOS                                 │    │
│  ├─────────┬─────────────┬────────────┬─────────────┤    │
│  │ Usuario │ Nombre      │ Rol        │ Estado      │    │
│  ├─────────┼─────────────┼────────────┼─────────────┤    │
│  │jperez   │Juan Pérez   │Guardia     │✅ Activo   │    │
│  │mgarcia  │María García │Guardia     │✅ Activo   │    │
│  └─────────┴─────────────┴────────────┴─────────────┘    │
│                                                             │
│  ┌───────────────────────────────────────────────────┐    │
│  │ INFORMACIÓN DEL USUARIO (Solo lectura)           │    │
│  │ Nombre Usuario: jperez                            │    │
│  │ Nombre Completo: Juan Pérez García                │    │
│  │ Rol:             Guardia                          │    │
│  │ Estado:          ✅ Activo                        │    │
│  └───────────────────────────────────────────────────┘    │
│                                                             │
│  [NUEVO] [EDITAR] [CAMBIAR CONTRASEÑA]                    │
│  [DESHABILITAR] [HABILITAR] [CANCELAR]                    │
└────────────────────────────────────────────────────────────┘
```

### ⭐ NUEVO: Visualización Automática

**Cambio importante**: Ahora al hacer clic en un usuario de la lista, sus datos se cargan **automáticamente** en el formulario en modo **solo lectura**.

**Flujo de trabajo**:
1. **Seleccionar usuario** → Datos se muestran automáticamente (solo lectura)
2. **Hacer clic en "EDITAR"** → Campos se habilitan para modificación
3. **Modificar datos** → Los cambios se reflejan en tiempo real
4. **Hacer clic en "GUARDAR"** → Cambios se persisten en la base de datos
5. **Hacer clic en "CANCELAR"** → Se descarta la operación

###Operaciones de Usuario

#### 1. Visualizar Usuario

**Pasos**:
1. Seleccionar usuario de la lista (hacer clic en la fila)
2. Los datos del usuario aparecen automáticamente en el panel derecho
3. Todos los campos están en modo **solo lectura** (fondo gris claro)
4. Puede ver: Nombre de Usuario, Nombre Completo, Rol, Estado

**Beneficio**: Ya no necesita hacer clic en "Editar" solo para ver la información completa.

#### 2. Crear Nuevo Usuario

**Pasos**:
1. Clic en botón **[NUEVO]**
2. Se limpian todos los campos
3. Los campos se habilitan para edición
4. Completar formulario:
   - Nombre de Usuario (único, sin espacios)
   - Contraseña (mínimo 6 caracteres)
   - Confirmar Contraseña (debe coincidir)
   - Nombre Completo
   - Rol (seleccionar del dropdown)
5. Clic en **[GUARDAR]**

**Validaciones automáticas**:
- ✅ Usuario único
- ✅ Contraseñas coinciden
- ✅ Todos los campos completos

**Resultado**: Usuario creado y visible en la lista

#### 3. Editar Usuario Existente

**Pasos**:
1. Seleccionar usuario de la lista
2. **Clic en [EDITAR]** → Los campos se habilitan
3. Modificar campos necesarios:
   - ✅ Nombre Completo
   - ✅ Rol
   - ❌ Nombre de Usuario (no editable, es el identificador único)
   - ❌ Contraseña (usar "Cambiar Contraseña")
4. Clic en **[GUARDAR]**

**Importante**: El nombre de usuario NO puede modificarse. Para cambiar contraseña, use el botón específico.

#### 4. Cambiar Contraseña

**Uso**: Restablecer contraseña de un usuario

**Pasos**:
1. Seleccionar usuario de la lista
2. Clic en **[CAMBIAR CONTRASEÑA]**
3. Aparece diálogo:
   ```
   ╔════════════════════════════════╗
   ║   🔑 CAMBIAR CONTRASEÑA        ║
   ╠════════════════════════════════╣
   ║ Usuario: jperez                ║
   ║                                ║
   ║ Nueva Contraseña:              ║
   ║ [•••••••••]                    ║
   ║                                ║
   ║ Confirmar Contraseña:          ║
   ║ [•••••••••]                    ║
   ║                                ║
   ║   [ACEPTAR]    [CANCELAR]      ║
   ╚════════════════════════════════╝
   ```
4. Ingresar nueva contraseña (dos veces)
5. Clic en **[ACEPTAR]**

**Importante**: Informar al usuario su nueva contraseña de forma segura.

#### 5. Deshabilitar Usuario

**Uso**: Suspender temporalmente acceso sin eliminar datos

**Pasos**:
1. Seleccionar usuario
2. Clic en **[DESHABILITAR]**
3. Confirmar acción
4. Usuario queda con estado **❌ Inactivo**

**Resultado**: El usuario no podrá iniciar sesión hasta ser habilitado nuevamente.

#### 6. Habilitar Usuario

**Uso**: Reactivar usuario previamente deshabilitado

**Pasos**:
1. Seleccionar usuario inactivo
2. Clic en **[HABILITAR]**
3. Confirmar acción
4. Usuario queda con estado **✅ Activo**

### Validaciones y Mensajes

| Situación | Mensaje |
|-----------|---------|
| Usuario creado | "Usuario creado exitosamente" |
| Usuario actualizado | "Usuario actualizado exitosamente" |
| Contraseña cambiada | "Contraseña cambiada exitosamente" |
| Usuario deshabilitado | "Usuario deshabilitado exitosamente" |
| Usuario habilitado | "Usuario habilitado exitosamente" |
| Usuario ya existe | "El nombre de usuario ya existe" |
| Contraseñas no coinciden | "Las contraseñas no coinciden" |
| Campos incompletos | "Complete todos los campos obligatorios" |

---

## Módulo: Configuración del Sistema

### Acceso al Módulo

**Requisito**: Rol de **Administrador**

1. En la ventana principal, clic en **Configuración** en el sidebar
2. Se abre la ventana de **Configuración y Parámetros del Sistema**

### Interfaz de Configuración

La ventana de configuración está organizada en **3 módulos principales**:

```
┌────────────────────────────────────────────────────────────┐
│         CONFIGURACIÓN Y PARÁMETROS DEL SISTEMA             │
├────────────────────────────────────────────────────────────┤
│                                                             │
│  ┌─────────────────────────────────────────────────────┐  │
│  │  ⏰ CONFIGURACIÓN DE HORA DE CIERRE                  │  │
│  │                                                       │  │
│  │  Nombre de la Filial:  [N. A. Primavera_______]     │  │
│  │  Hora de Cierre:       Hora: [22] Minuto: [00]      │  │
│  │                        (Formato 24 horas: 0-23)      │  │
│  │                                                       │  │
│  │            [GUARDAR CONFIGURACIÓN]                   │  │
│  └─────────────────────────────────────────────────────┘  │
│                                                             │
│  ┌─────────────────────────────────────────────────────┐  │
│  │  👥 GESTIÓN DE USUARIOS                              │  │
│  │                                                       │  │
│  │  Administra usuarios del sistema, roles y permisos.  │  │
│  │                                                       │  │
│  │         [ABRIR GESTIÓN DE USUARIOS]                  │  │
│  └─────────────────────────────────────────────────────┘  │
│                                                             │
│  ┌─────────────────────────────────────────────────────┐  │
│  │  📋 GESTIÓN DE CATEGORÍAS                            │  │
│  │                                                       │  │
│  │  Crea, edita y elimina categorías para clasificar   │  │
│  │  asistentes.                                         │  │
│  │                                                       │  │
│  │         [ABRIR GESTIÓN DE CATEGORÍAS]                │  │
│  └─────────────────────────────────────────────────────┘  │
└────────────────────────────────────────────────────────────┘
```

### Módulo 1: Configuración de Hora de Cierre

**Propósito**: Establecer la hora oficial de cierre de la escuela/filial.

**Campos**:
- **Nombre de la Filial**: Nombre de la sede (ej: "N. A. Primavera")
- **Hora**: Hora de cierre en formato 24 horas (0-23)
- **Minuto**: Minutos de la hora de cierre (0-59)

**Pasos**:
1. Ingresar o modificar el nombre de la filial
2. Ajustar hora y minuto de cierre
3. Hacer clic en **[GUARDAR CONFIGURACIÓN]**

**Ejemplo**:
```
Filial: N. A. Primavera
Hora de cierre: 22:00 (10:00 PM)
```

**Resultado**: Se actualiza la configuración y se registra en auditoría.

### Módulo 2: Gestión de Usuarios

**Propósito**: Acceso rápido al módulo de gestión de usuarios.

**Acción**: Hacer clic en **[ABRIR GESTIÓN DE USUARIOS]**

**Resultado**: Se abre la ventana de Gestión de Usuarios (ver sección anterior).

### Módulo 3: Gestión de Categorías

**Propósito**: Administrar categorías para clasificar asistentes.

**Acción**: Hacer clic en **[ABRIR GESTIÓN DE CATEGORÍAS]**

**Resultado**: Se abre la ventana de Gestión de Categorías (ver siguiente sección).

---

## Módulo: Gestión de Categorías

### ¿Qué son las Categorías?

Las categorías permiten clasificar a los asistentes según su relación con la organización:
- Estructural / Jefe de Filial
- Secretarios
- GG.FF (Grupos Femeninos)
- GG.MM (Grupos Masculinos)
- Guardia de Seguridad
- Miembros
- Filosofía
- Visitas
- Otros

> **Nota (versión 1.2)**: El campo **Número de Grupo** ha sido removido de las categorías. Ahora los números de grupo se gestionan exclusivamente en el **Módulo de Gestión de Asistentes**, donde solo la categoría "Miembros" utiliza este dato.

### Interfaz de Gestión de Categorías

```
┌────────────────────────────────────────────────────────────┐
│                  GESTIÓN DE CATEGORÍAS                      │
├────────────────────────────────────────────────────────────┤
│                                                             │
│  ┌──────────────────────┐  ┌────────────────────────────┐ │
│  │ LISTA DE CATEGORÍAS  │  │ FORMULARIO DE CATEGORÍA    │ │
│  ├──────────────────────┤  ├────────────────────────────┤ │
│  │ Nombre               │  │ Nombre:  [____________]    │ │
│  ├──────────────────────┤  │                            │ │
│  │ Miembros             │  │                            │ │
│  │ GG.FF                │  │ [NUEVA] [GUARDAR]          │ │
│  │ Visitas              │  │ [EDITAR] [CANCELAR]        │ │
│  │ Otros                │  │                            │ │
│  └──────────────────────┘  │ [ELIMINAR CATEGORÍA]       │ │
│                             └────────────────────────────┘ │
└────────────────────────────────────────────────────────────┘
```

### Operaciones con Categorías

#### 1. Crear Nueva Categoría

**Pasos**:
1. Clic en **[NUEVA]**
2. Ingresar:
   - **Nombre**: Nombre descriptivo de la categoría (ej: "Colaboradores", "Invitados Especiales")
3. Clic en **[GUARDAR]**

**Validación**: El sistema verifica que el nombre no exista ya.

**✅ Mensaje de confirmación**: "Categoría creada exitosamente"

#### 2. Editar Categoría

**Pasos**:
1. Seleccionar categoría de la lista
2. Clic en **[EDITAR]**
3. Modificar el **Nombre**
4. Clic en **[GUARDAR]**

**✅ Mensaje de confirmación**: "Categoría actualizada exitosamente"

#### 3. Eliminar Categoría

**Pasos**:
1. Seleccionar categoría de la lista
2. Clic en **[ELIMINAR CATEGORÍA]**
3. Confirmar acción en el diálogo:
   > "¿Está seguro de eliminar la categoría [Nombre]?"

⚠️ **Advertencia**: 
- La eliminación es **permanente**
- Solo se pueden eliminar categorías que **no estén asignadas** a ningún asistente
- Si intentas eliminar una categoría en uso, el sistema mostrará error

**✅ Mensaje de confirmación**: "Categoría eliminada exitosamente"

### Validaciones

| Validación | Mensaje |
|------------|---------|
| Nombre vacío | El botón GUARDAR permanece deshabilitado |
| Nombre duplicado | "Ya existe una categoría con el nombre 'XXX'" |
| Categoría en uso | "No se puede eliminar la categoría porque está asignada a uno o más asistentes" |
| Eliminación exitosa | "Categoría eliminada exitosamente" |

### Relación con Módulo de Asistentes

- Las categorías creadas aquí estarán disponibles en la lista desplegable del **Módulo de Gestión de Asistentes**
- Cuando un asistente es clasificado como **"Miembros"**, se le asigna un **Número de Grupo** (01, 02, 03, etc.) en el módulo de asistentes
- Las demás categorías **no requieren** número de grupo

---

## Módulo: Gestión de Asistentes

⭐ **Estado**: **IMPLEMENTADO** - Versión 1.2

### Descripción General

El módulo de **Gestión de Asistentes** permite mantener el registro completo de todas las personas que asisten a la escuela filosófica. Incluye funcionalidades para crear, editar, activar, inactivar y eliminar registros de asistentes con validaciones y auditoría completa.

### Acceso al Módulo

1. En la **Ventana Principal**, localiza el panel **GESTIÓN DE ASISTENTES**
2. Haz clic en el botón **"Gestionar"**
3. Se abrirá la ventana de **Gestión de Asistentes**

> **Permisos**: Disponible para **Administradores** y **Guardias**

---

### Interfaz del Módulo

La ventana se divide en dos secciones principales:

#### 📋 **Sección Izquierda: Lista de Asistentes**
- **Tabla de datos** con las siguientes columnas:
  - **DNI/Código**: Identificador único del asistente
  - **Apellidos**: Apellidos del asistente
  - **Nombres**: Nombres del asistente
  - **Categoría**: Categoría asignada
  - **Grupo**: Número de grupo (solo para "Miembros")
  - **Estado**: Activo o Inactivo

- **CheckBox**: "Mostrar solo activos"
  - ✅ Marcado: Muestra solo asistentes activos
  - ☐ Desmarcado: Muestra todos (activos e inactivos)

#### 📝 **Sección Derecha: Formulario de Datos**
- **DNI/Código**: Campo de texto (único, obligatorio)
- **Nombres**: Campo de texto (obligatorio)
- **Apellidos**: Campo de texto (obligatorio)
- **Categoría**: Lista desplegable (obligatorio)
- **Número de Grupo**: Campo de texto (visible solo para categoría "Miembros")

**Botones de Acción**:
- **NUEVO**: Crear un nuevo asistente
- **GUARDAR**: Guardar cambios
- **EDITAR**: Editar asistente seleccionado
- **INACTIVAR**: Desactivar asistente (eliminación lógica)
- **ACTIVAR**: Reactivar asistente inactivo
- **ELIMINAR**: Eliminar permanentemente (⚠️ IRREVERSIBLE)
- **CANCELAR**: Cancelar operación actual

---

### 1️⃣ Registrar un Nuevo Asistente

#### Pasos:

1. **Clic en "NUEVO"**
   - Se limpia el formulario
   - Todos los campos quedan habilitados para edición

2. **Completar los Datos Obligatorios**:
   - **DNI/Código**: Ingresa el documento de identidad o código único
     - ⚠️ Debe ser único (el sistema validará que no exista)
     - Ejemplo: `12345678`, `DNI12345678`, `COD001`
   
   - **Nombres**: Ingresa el nombre completo
     - Ejemplo: `Juan Carlos`
   
   - **Apellidos**: Ingresa los apellidos
     - Ejemplo: `Pérez García`
   
   - **Categoría**: Selecciona de la lista desplegable
     - Opciones: Estructural / Jefe de Filial, Secretarios, GG.FF, GG.MM, Guardia de Seguridad, **Miembros**, Filosofía, Visitas, Otros

3. **Número de Grupo (Solo para "Miembros")**:
   - Si seleccionaste categoría **"Miembros"**, aparecerá el campo **Número de Grupo**
   - Formato: **2 dígitos** (01, 02, 03, 04, etc.)
   - Ejemplo: `01` para Grupo 1, `15` para Grupo 15
   - ⚠️ **Obligatorio** para miembros

4. **Clic en "GUARDAR"**
   - El sistema valida los datos
   - Si todo es correcto, se crea el registro
   - Mensaje de confirmación: "Asistente registrado exitosamente"
   - La lista se actualiza automáticamente

#### ✅ Validaciones al Crear:
- ❌ **DNI duplicado**: "Ya existe un asistente con este DNI/Código"
- ❌ **Campos vacíos**: "El campo [X] es obligatorio"
- ❌ **Número de Grupo para Miembros**: "El número de grupo es obligatorio para la categoría 'Miembros'"
- ❌ **Formato incorrecto**: "El número de grupo debe tener formato de 2 dígitos (ejemplo: 01, 02, 03)"

---

### 2️⃣ Editar un Asistente Existente

#### Pasos:

1. **Seleccionar un Asistente de la Lista**
   - Haz clic sobre la fila del asistente que deseas editar
   - Los datos se cargan automáticamente en el formulario
   - El botón **EDITAR** se habilita

2. **Clic en "EDITAR"**
   - Los campos se habilitan para edición
   - ⚠️ **IMPORTANTE**: El campo **DNI/Código** permanece bloqueado (no se puede modificar)

3. **Modificar los Datos Deseados**:
   - Puedes cambiar: Nombres, Apellidos, Categoría
   - Si cambias la categoría a **"Miembros"**, aparece el campo Número de Grupo
   - Si cambias de "Miembros" a otra categoría, el Número de Grupo se limpia automáticamente

4. **Clic en "GUARDAR"**
   - El sistema valida los cambios
   - Mensaje de confirmación: "Asistente actualizado exitosamente"
   - La lista se actualiza

5. **Clic en "CANCELAR"** (opcional)
   - Descarta los cambios sin guardar
   - Vuelve al estado de solo lectura

#### 📝 Nota sobre Auditoría:
Cada modificación queda registrada con detalle de los cambios realizados (DNI, Categoría, Grupo).

---

### 3️⃣ Inactivar un Asistente (Eliminación Lógica)

La **inactivación** es una eliminación reversible. El asistente permanece en la base de datos pero marcado como inactivo.

#### Pasos:

1. **Seleccionar un Asistente Activo**
   - Debe tener estado "Activo"
   - El botón **INACTIVAR** se habilita

2. **Clic en "INACTIVAR"**
   - Aparece diálogo de confirmación:
     > "¿Está seguro de inactivar al asistente [Nombre Apellidos]?"

3. **Confirmar con "Sí"**
   - El asistente cambia a estado "Inactivo"
   - Mensaje: "Asistente inactivado exitosamente"
   - Si está marcado "Mostrar solo activos", desaparece de la lista

#### ✅ Ventaja:
- Puedes **reactivar** el asistente en cualquier momento (ver siguiente sección)

---

### 4️⃣ Activar un Asistente Inactivo

#### Pasos:

1. **Desmarcar "Mostrar solo activos"**
   - Ahora verás todos los asistentes, incluidos los inactivos

2. **Seleccionar un Asistente Inactivo**
   - El botón **ACTIVAR** se habilita

3. **Clic en "ACTIVAR"**
   - Diálogo de confirmación:
     > "¿Está seguro de activar al asistente [Nombre Apellidos]?"

4. **Confirmar con "Sí"**
   - El asistente vuelve a estado "Activo"
   - Mensaje: "Asistente activado exitosamente"

---

### 5️⃣ Eliminar un Asistente Permanentemente

⚠️ **ATENCIÓN**: Esta acción es **IRREVERSIBLE**. El registro se elimina completamente de la base de datos.

#### Cuándo Usar:
- Solo para registros erróneos o duplicados
- **No recomendado** para asistentes que ya no asisten (mejor usar Inactivar)

#### Pasos:

1. **Seleccionar el Asistente**
   - El botón **ELIMINAR** se habilita

2. **Clic en "ELIMINAR"**
   - Diálogo de confirmación:
     > "¿Está seguro de eliminar permanentemente al asistente [Nombre Apellidos]?  
     > Esta acción NO se puede deshacer."

3. **Confirmar con "Sí"**
   - El registro se elimina de la base de datos
   - Mensaje: "Asistente eliminado exitosamente"
   - La lista se actualiza

---

### 🔍 Filtrar la Lista de Asistentes

#### Mostrar Solo Activos:
- ✅ **Marcado**: Lista muestra solo asistentes con estado "Activo"
- ☐ **Desmarcado**: Lista muestra todos los asistentes (activos e inactivos)

#### Seleccionar un Asistente:
- Haz clic sobre cualquier fila de la tabla
- Los datos se cargan automáticamente en el formulario
- Los botones se habilitan según el estado del asistente

---

### 📊 Categorías de Asistentes

El sistema incluye las siguientes categorías predefinidas:

| Categoría | Requiere Número de Grupo | Descripción |
|-----------|--------------------------|-------------|
| **Estructural / Jefe de Filial** | No | Personal directivo |
| **Secretarios** | No | Personal administrativo |
| **GG.FF (Grupos Femeninos)** | No | Grupos de damas |
| **GG.MM (Grupos Masculinos)** | No | Grupos de caballeros |
| **Guardia de Seguridad** | No | Personal de seguridad |
| **Miembros** | **Sí (obligatorio)** | Miembros numerados (01, 02, 03...) |
| **Filosofía** | No | Estudiantes de filosofía |
| **Visitas** | No | Visitantes ocasionales |
| **Otros** | No | Otras categorías |

> **Nota**: Las categorías se gestionan desde el **Módulo de Configuración** → **Gestión de Categorías**

---

### ⚙️ Comportamiento del Campo "Número de Grupo"

| Escenario | Comportamiento |
|-----------|----------------|
| Categoría ≠ "Miembros" | Campo **oculto** y valor automáticamente `null` |
| Categoría = "Miembros" + Modo NUEVO | Campo **visible y obligatorio** |
| Categoría = "Miembros" + Modo EDITAR | Campo **visible y editable** |
| Cambio de otra categoría a "Miembros" | Campo **aparece vacío**, debe llenarse |
| Cambio de "Miembros" a otra categoría | Campo **se oculta** y valor se limpia |

---

### 📝 Ejemplos Prácticos

#### Ejemplo 1: Registrar un Miembro del Grupo 03
```
1. Clic en NUEVO
2. DNI/Código: 12345678
3. Nombres: María Elena
4. Apellidos: López Sánchez
5. Categoría: Miembros
6. Número de Grupo: 03
7. Clic en GUARDAR
✅ Resultado: "Asistente registrado exitosamente"
```

#### Ejemplo 2: Registrar una Visita
```
1. Clic en NUEVO
2. DNI/Código: 87654321
3. Nombres: Carlos
4. Apellidos: Ramírez
5. Categoría: Visitas
   (Campo Número de Grupo NO aparece)
6. Clic en GUARDAR
✅ Resultado: "Asistente registrado exitosamente"
```

#### Ejemplo 3: Cambiar Categoría de Visita a Miembro
```
1. Seleccionar asistente (Carlos Ramírez - Visitas)
2. Clic en EDITAR
3. Categoría: Cambiar a "Miembros"
   → El campo Número de Grupo aparece vacío
4. Número de Grupo: 08
5. Clic en GUARDAR
✅ Resultado: "Asistente actualizado exitosamente"
   Auditoría: "Categoría: Visitas → Miembros, Grupo: N/A → 08"
```

---

### 🛡️ Auditoría y Trazabilidad

Todas las operaciones quedan registradas en el sistema de auditoría:

| Operación | Información Registrada |
|-----------|------------------------|
| **Crear** | Nombre completo, DNI, categoría, grupo |
| **Actualizar** | Cambios específicos (DNI, Categoría, Grupo) |
| **Eliminar** | Nombre completo, DNI (PERMANENTE) |
| **Inactivar** | Nombre completo, DNI, cambio a inactivo |
| **Activar** | Nombre completo, DNI, cambio a activo |

> Cada registro incluye: Usuario que realizó la acción, Fecha y hora, Detalles específicos

---

### ❓ Preguntas Frecuentes

**P: ¿Puedo cambiar el DNI de un asistente?**  
R: No. El DNI es un identificador único y no puede modificarse después de la creación. Si necesitas corregirlo, debes eliminar el registro y crear uno nuevo.

**P: ¿Qué diferencia hay entre INACTIVAR y ELIMINAR?**  
R: 
- **INACTIVAR**: Marca el asistente como inactivo pero conserva el registro. Es reversible (puedes reactivar).
- **ELIMINAR**: Borra permanentemente el registro de la base de datos. Es irreversible.

**P: ¿Puedo tener varios asistentes con el mismo nombre?**  
R: Sí, siempre que tengan DNI/Código diferente.

**P: ¿Qué pasa si intento guardar sin llenar el Número de Grupo para un Miembro?**  
R: El sistema mostrará el error: "El número de grupo es obligatorio para la categoría 'Miembros'".

**P: ¿Puedo cambiar un Miembro del Grupo 01 al Grupo 05?**  
R: Sí, en modo EDITAR puedes modificar el Número de Grupo. El cambio quedará auditado.

---

## Módulo: Control de Asistencia

⭐ **Estado**: **IMPLEMENTADO** - Versión 1.3

### Descripción General

El módulo de **Control de Asistencia** permite llevar el registro diario de entradas y salidas de todos los asistentes a la escuela filosófica. Incluye funcionalidades para:

- ✅ **Registrar Ingreso** mediante DNI/Código
- ✅ **Registrar Salida** con observaciones opcionales
- ✅ **Aplicar Cierre Automático** al final del día
- ✅ **Consultar Historial** por fecha
- ✅ **Actualización en Tiempo Real** cada 30 segundos
- ✅ **Validaciones Completas** para prevenir errores
- ✅ **Auditoría Completa** de todas las operaciones

### Acceso al Módulo

1. En la **Ventana Principal**, localiza el panel **CONTROL DE ASISTENCIA**
2. Haz clic en el botón **"Control"**
3. Se abrirá la ventana de **Control de Asistencia**

> **Permisos**: Disponible para **Administradores** y **Guardias**

---

### Interfaz del Módulo

La ventana se divide en tres secciones principales:

```
┌─────────────────────────────────────────────────────────────────────┐
│                   CONTROL DE ASISTENCIA                              │
├─────────────────────────────────────────────────────────────────────┤
│  🔍 REGISTRO RÁPIDO                                                 │
│  ┌───────────────────────────────────────────────────────────────┐ │
│  │ Ingresar DNI: [________________]  [REGISTRAR INGRESO]         │ │
│  └───────────────────────────────────────────────────────────────┘ │
├──────────────────────────────────┬──────────────────────────────────┤
│ 👥 ASISTENTES PRESENTES (Hoy)    │ 📋 HISTORIAL DEL DÍA            │
│ Total: 15                         │                                 │
│ ┌──────────────────────────────┐ │ Fecha: [____/____/____] [🔍]   │
│ │ DNI    │ Apellidos │ Nombres │ │ ┌────────────────────────────┐ │
│ │        │ Categoría │ Ingreso │ │ │ DNI    │ Apellidos │Ingreso│ │
│ ├────────┼───────────┼─────────┤ │ │        │ Nombres   │Salida │ │
│ │12345678│Pérez      │Juan     │ │ │        │ Categoría │Cierre │ │
│ │        │Miembros   │08:15    │ │ │        │ Observ.   │       │ │
│ │87654321│López      │María    │ │ ├────────┼───────────┼───────┤ │
│ │        │GG.FF      │09:30    │ │ │12345678│Pérez Juan │08:15  │ │
│ │...     │...        │...      │ │ │        │Miembros   │21:45  │ │
│ └──────────────────────────────┘ │ │        │           │☐      │ │
│                                   │ │        │Clase norm.│       │ │
│ Observación: [________________]   │ │...     │...        │...    │ │
│ [REGISTRAR SALIDA] [ACTUALIZAR]  │ └────────────────────────────┘ │
│ [APLICAR CIERRE AUTOMÁTICO]      │                                 │
└──────────────────────────────────┴──────────────────────────────────┘
  Actualización automática cada 30 seg.
```

#### 🔍 **Sección Superior: Registro Rápido**
- **Campo DNI**: Ingreso directo del DNI o código del asistente
- **Botón "REGISTRAR INGRESO"**: Registra la entrada inmediata

#### 👥 **Sección Izquierda: Asistentes Presentes**
- Muestra todos los asistentes que **ingresaron hoy** y **aún no han salido**
- Contador de **Total de Presentes**
- **Columnas**:
  - DNI/Código
  - Apellidos, Nombres
  - Categoría
  - Hora de Ingreso
- **Observación**: Campo para agregar notas al registrar salida
- **Botones**:
  - **REGISTRAR SALIDA**: Marca la salida del asistente seleccionado
  - **ACTUALIZAR**: Refresca la lista manualmente
  - **APLICAR CIERRE AUTOMÁTICO**: Registra salida automática para todos los presentes

#### 📋 **Sección Derecha: Historial del Día**
- Muestra **todos los registros** del día seleccionado (con y sin salida)
- **Selector de Fecha**: Permite consultar días anteriores
- **Columnas**:
  - DNI/Código
  - Apellidos, Nombres
  - Categoría
  - Hora de Ingreso
  - Hora de Salida (puede estar vacía si aún está presente)
  - Cierre: ✅ si fue cierre automático
  - Observación (si existe)
- **Botón "🔍 BUSCAR"**: Consulta historial de la fecha seleccionada

---

### 1️⃣ Registrar Ingreso de un Asistente

El registro de ingreso marca la hora exacta en que un asistente llega a la escuela.

#### Pasos:

1. **Localizar el Campo "Ingresar DNI"** (parte superior de la ventana)

2. **Ingresar el DNI o Código del Asistente**
   - Escribe el DNI completo (ej: `12345678`)
   - El DNI debe corresponder a un asistente registrado en el sistema

3. **Hacer Clic en "REGISTRAR INGRESO"**
   - El sistema valida:
     - ✅ DNI no vacío
     - ✅ Asistente existe en la base de datos
     - ✅ Asistente está activo (Estado = Activo)
     - ✅ No tiene una entrada abierta (sin salida) del día de hoy

4. **Confirmación**
   - ✅ Mensaje: "Ingreso registrado exitosamente para [Nombre Apellidos]"
   - La lista de **Asistentes Presentes** se actualiza automáticamente
   - El contador de **Total** incrementa en 1
   - Se registra la hora actual como **Hora de Ingreso**

#### 📝 Ejemplo Práctico:
```
Escenario: Juan Pérez (DNI 12345678) llega a las 8:15 AM

1. Escribir en campo: 12345678
2. Clic en [REGISTRAR INGRESO]
3. ✅ Mensaje: "Ingreso registrado exitosamente para Juan Pérez"
4. Juan aparece en "Asistentes Presentes":
   - DNI: 12345678
   - Nombre: Juan Pérez
   - Categoría: Miembros
   - Ingreso: 08:15:23
```

#### ⚠️ Validaciones y Errores Posibles:

| Error | Mensaje | Solución |
|-------|---------|----------|
| DNI vacío | "El DNI no puede estar vacío" | Ingresa un DNI válido |
| Asistente no existe | "No se encontró un asistente con el DNI [XXX]. Debe registrarse primero como asistente" | Registrar al asistente en el **Módulo de Gestión de Asistentes** |
| Asistente inactivo | "El asistente [Nombre] está inactivo y no puede registrar asistencia" | Activar al asistente en el módulo de Gestión |
| Entrada duplicada | "El asistente [Nombre] ya tiene una entrada registrada hoy sin salida" | Esperar a registrar salida primero |

---

### 2️⃣ Registrar Salida de un Asistente

El registro de salida marca la hora exacta en que un asistente abandona la escuela.

#### Pasos:

1. **Localizar la Lista "Asistentes Presentes"** (panel izquierdo)

2. **Seleccionar el Asistente**
   - Haz clic sobre la fila del asistente que se retira
   - Los datos se resaltan

3. **Agregar Observación** (Opcional)
   - En el campo **"Observación"**, puedes agregar notas:
     - "Salida por emergencia"
     - "Clase completada"
     - "Se retiró temprano"
   - Máximo recomendado: 200 caracteres

4. **Hacer Clic en "REGISTRAR SALIDA"**
   - El sistema valida:
     - ✅ Asistencia seleccionada
     - ✅ No tiene salida registrada previamente
     - ✅ Hora de salida >= Hora de ingreso (automático)

5. **Confirmación**
   - ✅ Mensaje: "Salida registrada exitosamente para [Nombre Apellidos]"
   - El asistente **desaparece** de "Asistentes Presentes"
   - El asistente **aparece** en "Historial del Día" con hora de salida
   - El contador de **Total** disminuye en 1
   - Se registra la hora actual como **Hora de Salida**

#### 📝 Ejemplo Práctico:
```
Escenario: Juan Pérez (que ingresó a las 8:15 AM) se retira a las 9:45 PM

1. Seleccionar a "Juan Pérez" de la lista de Presentes
2. (Opcional) Observación: "Clase completada"
3. Clic en [REGISTRAR SALIDA]
4. ✅ Mensaje: "Salida registrada exitosamente para Juan Pérez"
5. Juan desaparece de "Presentes" y aparece en "Historial del Día":
   - Ingreso: 08:15:23
   - Salida: 21:45:10
   - Cierre: ☐ (no es cierre automático)
   - Observación: "Clase completada"
```

#### ⚠️ Validaciones y Errores Posibles:

| Error | Mensaje | Solución |
|-------|---------|----------|
| Sin selección | "Debe seleccionar un asistente de la lista" | Selecciona una fila |
| Salida ya registrada | "Este asistente ya tiene salida registrada" | Verifica el historial |
| Hora inválida | "La hora de salida no puede ser anterior a la hora de ingreso" | Contacta al administrador |

---

### 3️⃣ Aplicar Cierre Automático

La función de **Cierre Automático** registra la salida de **todos los asistentes presentes** al finalizar el día, utilizando la **Hora de Cierre** configurada en el sistema (ej: 22:00).

#### ¿Cuándo Usar Esta Función?

- **Al finalizar el día**: Cuando se termina la jornada y algunos asistentes no registraron su salida
- **Para agilizar el proceso**: En lugar de registrar salida individual para cada persona
- **Cumplimiento de políticas**: Registrar automáticamente que todos salieron a la hora de cierre oficial

#### Pasos:

1. **Verificar Asistentes Presentes**
   - Revisa la lista de "Asistentes Presentes"
   - Identifica quiénes no han registrado su salida

2. **Hacer Clic en "APLICAR CIERRE AUTOMÁTICO"**
   - Se abre un diálogo de confirmación:
     > "¿Está seguro de aplicar el cierre automático?  
     > Se registrará la salida de TODOS los asistentes presentes  
     > con la hora de cierre configurada ([HH:MM])."

3. **Confirmar con "Sí"**
   - El sistema ejecuta:
     - Obtiene la **Hora de Cierre** desde **Configuración del Sistema**
     - Para cada asistente presente:
       - Registra **Hora de Salida** = Fecha de hoy + Hora de cierre
       - Marca **Hasta Cierre** = ✅
       - Agrega **Observación** = "Cierre automático"
     - Registra auditoría global: "Cierre automático aplicado, [N] asistentes"

4. **Resultado**
   - ✅ Mensaje: "Cierre automático aplicado exitosamente. [N] asistentes procesados"
   - La lista de **Asistentes Presentes** queda **vacía** (Total: 0)
   - Todos los registros aparecen en **Historial del Día** con:
     - Hora de Salida = Hora de cierre configurada
     - Cierre = ✅
     - Observación = "Cierre automático"

#### 📝 Ejemplo Práctico:
```
Escenario: Son las 10:30 PM y quedan 3 asistentes sin registrar salida.
La hora de cierre configurada es 22:00 (10:00 PM).

Estado Antes:
- Asistentes Presentes: 3 personas
  * María López - Ingreso: 14:30
  * Carlos Ruiz - Ingreso: 16:00
  * Ana Torres - Ingreso: 18:15

Pasos:
1. Clic en [APLICAR CIERRE AUTOMÁTICO]
2. Confirmación: "¿Está seguro...?" → Sí
3. ✅ Mensaje: "Cierre automático aplicado exitosamente. 3 asistentes procesados"

Estado Después:
- Asistentes Presentes: 0 (lista vacía)
- Historial del Día incluye:
  * María López
    - Ingreso: 14:30
    - Salida: 22:00:00
    - Cierre: ✅
    - Obs: "Cierre automático"
  
  * Carlos Ruiz
    - Ingreso: 16:00
    - Salida: 22:00:00
    - Cierre: ✅
    - Obs: "Cierre automático"
  
  * Ana Torres
    - Ingreso: 18:15
    - Salida: 22:00:00
    - Cierre: ✅
    - Obs: "Cierre automático"
```

#### ⚠️ Consideraciones Importantes:

- 📌 **No es reversible**: Una vez aplicado, las salidas quedan registradas permanentemente
- ⏰ **Hora de cierre**: Se obtiene de **Configuración del Sistema** (módulo Configuración)
- 🔍 **Auditoría**: Queda registrado quién aplicó el cierre y cuántos asistentes fueron procesados
- ✅ **Identificación**: Los registros con cierre automático tienen la marca "Cierre = ✅"

---

### 4️⃣ Consultar Historial por Fecha

El historial permite revisar los registros de asistencia de cualquier día.

#### Pasos:

1. **Localizar el Panel "Historial del Día"** (lado derecho)

2. **Seleccionar una Fecha**
   - Haz clic en el **DatePicker** (calendario)
   - Selecciona el día que deseas consultar
   - Por defecto muestra el **día actual**

3. **Hacer Clic en "🔍 BUSCAR"**
   - El sistema consulta la base de datos
   - Carga todos los registros de asistencia del día seleccionado

4. **Revisar Resultados**
   - La tabla muestra:
     - **Todos los ingresos** del día (con y sin salida)
     - **Hora de Ingreso**
     - **Hora de Salida** (vacío si aún está presente)
     - **Marca de Cierre** (✅ si fue cierre automático)
     - **Observaciones** (si existen)

#### 📝 Ejemplo Práctico:
```
Escenario: Deseas revisar la asistencia del lunes 14 de octubre de 2025

1. Clic en el DatePicker
2. Seleccionar: 14/10/2025
3. Clic en [🔍 BUSCAR]
4. Resultados mostrados:

┌──────────┬───────────────┬─────────┬─────────┬──────┬─────────────────┐
│ DNI      │ Nombre        │ Ingreso │ Salida  │Cierre│ Observación     │
├──────────┼───────────────┼─────────┼─────────┼──────┼─────────────────┤
│12345678  │Pérez, Juan    │08:15    │21:30    │☐     │Clase normal     │
│87654321  │López, María   │09:00    │22:00    │✅    │Cierre automático│
│11223344  │García, Luis   │10:30    │20:00    │☐     │Salió temprano   │
│55667788  │Torres, Ana    │14:00    │         │      │(Aún presente)   │
└──────────┴───────────────┴─────────┴─────────┴──────┴─────────────────┘

Total de registros: 4
```

#### 🔍 Interpretación de Resultados:

| Condición | Interpretación |
|-----------|----------------|
| **Salida vacía** | El asistente está actualmente presente (si es hoy) o no registró salida (días pasados) |
| **Cierre = ✅** | La salida fue registrada mediante cierre automático |
| **Cierre = ☐** | La salida fue registrada manualmente |
| **Observación** | Notas adicionales sobre la asistencia |

---

### ⏱️ Actualización Automática

El módulo incluye **actualización automática** para mantener los datos al día sin intervención manual.

#### Funcionamiento:
- ⏰ **Cada 30 segundos**, el sistema automáticamente:
  - Refresca la lista de **Asistentes Presentes**
  - Actualiza el contador de **Total de Presentes**
  - Sincroniza con la base de datos

#### Indicador Visual:
- En la parte inferior de la ventana se muestra:
  > "Actualización automática cada 30 seg."

#### Beneficios:
- ✅ **Multi-usuario**: Si otro guardia registra un ingreso/salida, lo verás automáticamente
- ✅ **Datos actualizados**: No necesitas refrescar manualmente
- ✅ **Sin interrupciones**: Trabaja mientras el sistema se actualiza en segundo plano

#### Actualización Manual:
- Si deseas refrescar inmediatamente, haz clic en **[ACTUALIZAR]**

---

### 📊 Validaciones del Sistema

El módulo implementa validaciones exhaustivas para garantizar la integridad de los datos:

| Validación | Descripción | Mensaje de Error |
|------------|-------------|------------------|
| **DNI vacío** | El campo DNI no puede estar en blanco | "El DNI no puede estar vacío" |
| **Asistente inexistente** | El DNI no corresponde a ningún asistente registrado | "No se encontró un asistente con el DNI [XXX]. Debe registrarse primero como asistente" |
| **Asistente inactivo** | El asistente tiene Estado = Inactivo | "El asistente [Nombre] está inactivo y no puede registrar asistencia" |
| **Entrada duplicada** | Ya existe una entrada sin salida para hoy | "El asistente [Nombre] ya tiene una entrada registrada hoy sin salida" |
| **Salida sin selección** | No se seleccionó ningún asistente | "Debe seleccionar un asistente de la lista" |
| **Salida duplicada** | Ya se registró la salida previamente | "Este asistente ya tiene salida registrada" |
| **Hora de salida inválida** | La salida sería anterior al ingreso | "La hora de salida no puede ser anterior a la hora de ingreso" |
| **Sin asistentes para cierre** | La lista de presentes está vacía | "No hay asistentes presentes para aplicar cierre automático" |

---

### 🛡️ Auditoría y Trazabilidad

Todas las operaciones del módulo quedan registradas en el sistema de auditoría:

| Operación | Información Registrada |
|-----------|------------------------|
| **Registrar Ingreso** | Acción: "Registrar Ingreso", Detalle: "Ingreso - [Nombre Apellidos] - DNI: [XXX] - Hora: [HH:MM:SS]" |
| **Registrar Salida** | Acción: "Registrar Salida", Detalle: "Salida - [Nombre Apellidos] - DNI: [XXX] - Hora: [HH:MM:SS] - Obs: [Texto]" |
| **Cierre Automático** | Acción: "Cierre Automático", Detalle: "Cierre automático aplicado, [N] asistentes procesados a las [HH:MM:SS]" |

> Cada registro incluye: **Usuario** que realizó la acción, **Fecha y Hora** exacta, **Detalles** específicos

---

### ❓ Preguntas Frecuentes

**P: ¿Puedo registrar la entrada de un asistente que no está en el sistema?**  
R: No. Primero debes registrar al asistente en el **Módulo de Gestión de Asistentes**. El sistema validará que el DNI exista y que el asistente esté activo.

**P: ¿Qué pasa si olvido registrar la salida de un asistente?**  
R: Tienes dos opciones:
1. Registrar la salida manualmente cuando lo recuerdes
2. Aplicar **Cierre Automático** al final del día (todos los presentes saldrán con la hora de cierre configurada)

**P: ¿Puedo modificar una hora de entrada o salida ya registrada?**  
R: No directamente desde este módulo. Las horas se registran automáticamente con la hora actual. Si necesitas correcciones, contacta al administrador del sistema.

**P: ¿Qué significa "Cierre = ✅" en el historial?**  
R: Indica que la salida fue registrada mediante la función **Aplicar Cierre Automático**, no manualmente.

**P: ¿Puedo ver el historial de días anteriores?**  
R: Sí, usa el **DatePicker** en el panel "Historial del Día" para seleccionar cualquier fecha y haz clic en "🔍 BUSCAR".

**P: ¿Cada cuánto tiempo se actualiza la lista de presentes?**  
R: Automáticamente cada **30 segundos**. También puedes refrescar manualmente con el botón **[ACTUALIZAR]**.

**P: ¿Puedo registrar la entrada de varios asistentes al mismo tiempo?**  
R: No simultáneamente, pero puedes registrar uno tras otro rápidamente usando el campo "Ingresar DNI" y el botón "REGISTRAR INGRESO".

**P: ¿Qué pasa si un asistente intenta ingresar dos veces el mismo día?**  
R: El sistema lo detectará y mostrará el error: "El asistente [Nombre] ya tiene una entrada registrada hoy sin salida". Primero debe registrar su salida.

**P: ¿La observación es obligatoria al registrar salida?**  
R: No, es opcional. Puedes dejar el campo vacío si no hay nada que agregar.

**P: ¿Qué hora se usa para el cierre automático?**  
R: La **Hora de Cierre** configurada en el **Módulo de Configuración del Sistema**. Consulta con el administrador si necesitas modificarla.

---

### 💡 Consejos y Buenas Prácticas

1. **Registro Inmediato**:
   - Registra los ingresos en el momento en que los asistentes llegan
   - Evita acumulación de registros pendientes

2. **Uso de Observaciones**:
   - Agrega observaciones útiles al registrar salidas:
     - "Salida temprana por motivo personal"
     - "Clase completada"
     - "Emergencia familiar"
   - Estas notas son valiosas para auditorías y estadísticas

3. **Cierre Automático**:
   - Úsalo al finalizar la jornada para agilizar el proceso
   - Verifica antes que no haya asistentes que aún deban permanecer

4. **Consulta de Historial**:
   - Revisa el historial diariamente para detectar anomalías
   - Útil para verificar asistencias de días específicos

5. **Actualización Manual**:
   - Si trabajas con otro usuario simultáneamente, usa **[ACTUALIZAR]** para ver sus cambios al instante

6. **Validación de DNI**:
   - Verifica que el DNI sea correcto antes de registrar
   - Si aparece error "Asistente no existe", verifica en Gestión de Asistentes
- Consultar asistencias del día

**Disponible en**: Próxima versión

---

## Módulo: Reportes y Estadísticas

⭐ **MEJORADO v1.5** - Visualización detallada y exportaciones completas

### Descripción General

El módulo de **Reportes y Estadísticas** permite consultar, analizar y exportar información detallada de asistencias. Ofrece más de 10 tipos de reportes diferentes organizados en 3 pestañas para facilitar el análisis de datos.

**Características principales:**
- �️ **Ventana maximizada** para mejor visualización (NUEVO v1.5)
- �📊 Consultas por rango de fechas o fecha específica
- � **Detalle expandible** en Totales Diarios (Master-Detail UI) (NUEVO v1.5)
- �📈 10+ tipos de reportes diferentes (totales, categorías, grupos, individuales, analíticos)
- 📄 **Exportación detallada a Excel** con información completa de cada asistente (MEJORADO v1.5)
- 📑 **Exportación detallada a PDF** formato apaisado con todos los datos (MEJORADO v1.5)
- 🔍 Búsqueda de historial individual por DNI o Nombre/Apellidos
- 📉 Paneles analíticos: Puntualidad, Permanencia hasta cierre, Top constantes
- 🎯 Identificación del día de mayor asistencia
- 👥 **Diferenciación de Miembros por número de grupo** (MEJORADO v1.5)
- ✨ **Métricas simplificadas**: Eliminadas "Sin Salida" y "% del Total" (MEJORADO v1.5)

**Roles con acceso**: Administrador, Guardia (solo lectura)

**Novedades v1.5:**
- ✅ Ventana se abre maximizada automáticamente
- ✅ Click en fila de Totales Diarios para ver detalle de asistentes
- ✅ Exportaciones muestran DNI, nombre, categoría, grupo, fecha, horarios y estado
- ✅ Formato de exportación mejorado (Excel con colores, PDF apaisado)
- ✅ Grupos de Miembros diferenciados en reportes por categoría
- ✅ Eliminadas métricas confusas

---

### Acceso al Módulo

1. Desde la **Ventana Principal**, hacer clic en el módulo **[Reportes]**:

```
╔═════════════════════════════════════════════╗
║   VENTANA PRINCIPAL - MARTE                 ║
╠═════════════════════════════════════════════╣
║  [Usuarios]  [Configuración]  [Categorías]  ║
║  [Asistentes]  [Asistencia]  [📊 Reportes] ║ ← Aquí
╚═════════════════════════════════════════════╝
```

2. Se abrirá una nueva ventana: **Reportes y Estadísticas**

---

### Interfaz del Módulo

La ventana está organizada en **3 pestañas** con filtros globales en la parte superior:

```
╔═══════════════════════════════════════════════════════════════════╗
║               REPORTES Y ESTADÍSTICAS                             ║
╠═══════════════════════════════════════════════════════════════════╣
║  Filtros Globales:                                                ║
║  Fecha Inicio: [📅 01/09/2025] → Fecha Fin: [📅 18/10/2025]      ║
║  Estado: [Se encontraron 150 registros]                          ║
╠═══════════════════════════════════════════════════════════════════╣
║  📊 Consultas Básicas  │  👤 Historial Individual  │  📈 Paneles ║
║  ═══════════════════════════════════════════════════════════════  ║
║                                                                   ║
║  [Contenido de la pestaña activa]                                ║
║                                                                   ║
╠═══════════════════════════════════════════════════════════════════╣
║  ⏳ Procesando...                                                 ║
╚═══════════════════════════════════════════════════════════════════╝
```

**Filtros Globales:**
- **Fecha Inicio**: Fecha de inicio del rango (default: hace 1 mes)
- **Fecha Fin**: Fecha de fin del rango (default: hoy)
- **Estado**: Muestra mensajes informativos ("Consultando...", "Se encontraron X registros", etc.)

---

### Pestaña 1: 📊 Consultas Básicas

Esta pestaña contiene 3 secciones con reportes agregados.

#### 1️⃣ Totales Diarios

Muestra el total de asistencias por cada día en el rango seleccionado.

**Columnas mostradas:**
- **Fecha**: Día de la asistencia
- **Total Asistencias**: Número total de ingresos ese día
- **Hasta Cierre**: Cuántos se quedaron hasta el cierre
- **Sin Salida**: Cuántos no registraron salida

**Pasos para consultar:**
1. Ajustar **Fecha Inicio** y **Fecha Fin** en filtros globales
2. Click en **[Consultar]**
3. Esperar a que se carguen los datos
4. Revisar tabla con resultados

**Ejemplo de resultado:**
```
╔═══════════════════════════════════════════════════════╗
║  Fecha        │ Total Asist. │ Hasta Cierre │ Sin Salida ║
╠═══════════════════════════════════════════════════════╣
║  01/10/2025   │     45       │      38      │     2      ║
║  02/10/2025   │     52       │      45      │     3      ║
║  03/10/2025   │     48       │      40      │     1      ║
║  ...          │     ...      │     ...      │    ...     ║
╚═══════════════════════════════════════════════════════╝
```

**Exportación:**
- **[Exportar Excel]**: Genera archivo `.xlsx` con formato profesional (headers rojos, auto-ajuste)
- **[Exportar PDF]**: Genera archivo `.pdf` formato A4 para impresión

**Nombre de archivo sugerido**: `TotalesDiarios_20251018_143500.xlsx`

---

#### 2️⃣ Por Categoría

Agrupa asistencias por categoría de asistentes (Estudiante, Miembro, Externo, Amigo).

**Columnas mostradas:**
- **Categoría**: Nombre de la categoría
- **Total**: Total de asistencias de esa categoría
- **Hasta Cierre**: Cuántos se quedaron hasta el cierre
- **% del Total**: Porcentaje respecto al total general

**Pasos para consultar:**
1. Ajustar rango de fechas
2. Click en **[Consultar]**
3. Revisar distribución por categoría

**Ejemplo de resultado:**
```
╔═══════════════════════════════════════════════════════╗
║  Categoría    │ Total │ Hasta Cierre │ % del Total   ║
╠═══════════════════════════════════════════════════════╣
║  Estudiante   │  120  │     95       │    40.00%     ║
║  Miembro      │   90  │     82       │    30.00%     ║
║  Amigo        │   60  │     50       │    20.00%     ║
║  Externo      │   30  │     20       │    10.00%     ║
╠═══════════════════════════════════════════════════════╣
║  TOTAL:       │  300  │    247       │   100.00%     ║
╚═══════════════════════════════════════════════════════╝
```

**Interpretación:**
- El **% del Total** ayuda a identificar qué categorías tienen mayor participación
- **Hasta Cierre** muestra el compromiso de cada categoría

**Exportación**: Excel y PDF disponibles

---

#### 3️⃣ Por Grupo (Miembros)

Agrupa asistencias por número de grupo (Miembros #).

**Columnas mostradas:**
- **Grupo**: Número de grupo (1, 2, 3, etc.)
- **Total Asistencias**: Total de registros de ese grupo
- **Asistentes Diferentes**: Cuántos asistentes únicos del grupo asistieron
- **Promedio**: Promedio de asistencias por miembro del grupo

**Pasos para consultar:**
1. Ajustar rango de fechas
2. Click en **[Consultar]**
3. Analizar participación por grupo

**Ejemplo de resultado:**
```
╔═════════════════════════════════════════════════════════════╗
║  Grupo │ Total Asist. │ Asist. Dif. │ Promedio             ║
╠═════════════════════════════════════════════════════════════╣
║    1   │     85       │     12      │      7.08            ║
║    2   │     72       │     10      │      7.20            ║
║    3   │     95       │     15      │      6.33            ║
║   ...  │    ...       │    ...      │     ...              ║
╚═════════════════════════════════════════════════════════════╝
```

**Interpretación:**
- **Promedio alto**: El grupo asiste con mayor frecuencia
- **Asistentes Diferentes** muestra cuántos miembros del grupo están activos

**Exportación**: Solo Excel disponible

---

### Pestaña 2: 👤 Historial Individual

Consulta el historial completo de asistencias de un asistente específico.

#### Métodos de Búsqueda

**Método 1: Por DNI/Código**

1. Ingresar DNI o Código en el campo **[DNI/Código]**
   - Ejemplo: `12345678`
2. Click en **[Consultar]**
3. Se mostrará tabla con todas las asistencias de ese asistente

**Método 2: Por Nombre y Apellidos**

1. Ingresar **Nombres** en el campo correspondiente
   - Ejemplo: `Juan Carlos`
2. Ingresar **Apellidos** en el campo correspondiente
   - Ejemplo: `García López`
3. Click en **[Consultar]**
4. Se mostrará tabla con todas las asistencias que coincidan

**Columnas mostradas:**
- **Fecha**: Día de la asistencia
- **Hora Ingreso**: Hora en que ingresó (formato 24h: 19:45)
- **Hora Salida**: Hora en que salió (vacío si no registró salida)
- **Hasta Cierre**: ✅ si se quedó hasta el cierre
- **Observación**: Notas adicionales (ej: "Cierre automático")

**Ejemplo de resultado:**
```
╔═════════════════════════════════════════════════════════════════════════╗
║ Fecha      │ Hora Ingreso │ Hora Salida │ Hasta Cierre │ Observación   ║
╠═════════════════════════════════════════════════════════════════════════╣
║ 01/10/2025 │   19:30      │   22:00     │      ✅      │               ║
║ 02/10/2025 │   19:45      │   22:00     │      ✅      │               ║
║ 03/10/2025 │   20:00      │   21:30     │      ❌      │               ║
║ 05/10/2025 │   19:25      │   22:00     │      ✅      │ Cierre auto.  ║
║ ...        │   ...        │   ...       │     ...      │     ...       ║
╚═════════════════════════════════════════════════════════════════════════╝
```

**Exportación (solo método DNI):**
- **[Exportar Excel]**: Crea archivo con historial completo
- **[Exportar PDF]**: Genera PDF para imprimir historial

**Nombre de archivo sugerido**: `HistorialIndividual_12345678_20251018_143500.xlsx`

---

### Pestaña 3: 📈 Paneles Analíticos

Contiene 5 análisis avanzados para tomar decisiones basadas en datos.

#### 1️⃣ Tasa de Puntualidad

Analiza qué asistentes llegan puntualmente según una hora límite configurable.

**Configuración:**
- **Hora Límite**: Ingresar hora de corte (formato `HH:mm`)
  - Ejemplo: `19:30` (7:30 PM)
  - Por defecto: 19:30

**Pasos:**
1. Configurar **Hora Límite** (ej: `19:30`)
2. Ajustar rango de fechas en filtros globales
3. Click en **[Consultar]**

**Columnas mostradas:**
- **DNI**: Documento del asistente
- **Nombre Completo**: Nombres y apellidos
- **Categoría**: Estudiante, Miembro, etc.
- **Total Asistencias**: Cuántas veces asistió en el período
- **Puntuales**: Cuántas veces llegó antes de la hora límite
- **% Puntual**: Porcentaje de puntualidad
- **Prom. Ingreso**: Hora promedio de llegada

**Ejemplo de resultado:**
```
╔══════════════════════════════════════════════════════════════════════════════════╗
║ DNI      │ Nombre Completo      │ Categ. │ Total │ Punt. │ % Punt. │ Prom. Ing. ║
╠══════════════════════════════════════════════════════════════════════════════════╣
║ 12345678 │ Juan García López    │ Estud. │  15   │  14   │ 93.33%  │   19:22    ║
║ 87654321 │ María Pérez Santos   │ Miemb. │  20   │  18   │ 90.00%  │   19:25    ║
║ 11223344 │ Pedro Díaz Ruiz      │ Amigo  │  10   │   7   │ 70.00%  │   19:45    ║
║ ...      │ ...                  │ ...    │  ...  │  ...  │  ...    │   ...      ║
╚══════════════════════════════════════════════════════════════════════════════════╝
```

**Interpretación:**
- **% Puntual alto (>80%)**: Asistente muy comprometido
- **% Puntual bajo (<50%)**: Podría necesitar recordatorios
- **Prom. Ingreso**: Indica hábito de llegada del asistente

**Orden**: Resultados ordenados por **% Puntual** descendente (mejores primero)

**Exportación**: Solo Excel disponible

---

#### 2️⃣ Asistentes Hasta Cierre

Muestra qué asistentes se quedan hasta el cierre con mayor frecuencia.

**Pasos:**
1. Ajustar rango de fechas
2. Click en **[Consultar]**

**Columnas mostradas:**
- **DNI**: Documento del asistente
- **Nombre Completo**: Nombres y apellidos
- **Categoría**: Estudiante, Miembro, etc.
- **Total Días**: Cuántas veces asistió
- **Días Hasta Cierre**: Cuántas veces se quedó hasta el cierre
- **% Hasta Cierre**: Porcentaje de permanencia completa

**Ejemplo de resultado:**
```
╔════════════════════════════════════════════════════════════════════════════╗
║ DNI      │ Nombre Completo      │ Categ. │ Total │ H.Cierre │ % H.Cierre ║
╠════════════════════════════════════════════════════════════════════════════╣
║ 12345678 │ Juan García López    │ Estud. │  15   │    14    │   93.33%   ║
║ 87654321 │ María Pérez Santos   │ Miemb. │  20   │    19    │   95.00%   ║
║ 11223344 │ Pedro Díaz Ruiz      │ Amigo  │  10   │     6    │   60.00%   ║
║ ...      │ ...                  │ ...    │  ...  │   ...    │    ...     ║
╚════════════════════════════════════════════════════════════════════════════╝
```

**Interpretación:**
- **% Hasta Cierre alto (>80%)**: Compromiso alto, asistente constante
- **% Hasta Cierre bajo (<50%)**: Podría tener compromisos o dificultades de horario

**Exportación**: Solo Excel disponible

---

#### 3️⃣ Asistencia Por Grupo (Detalle)

Analiza la asistencia de cada grupo día por día.

**Pasos:**
1. Ajustar rango de fechas
2. Click en **[Consultar]**

**Columnas mostradas:**
- **Grupo**: Número de grupo (Miembros #)
- **Fecha**: Día específico
- **Total Miembros**: Cuántos miembros tiene el grupo
- **Asistieron**: Cuántos miembros del grupo asistieron ese día
- **% Asistencia**: Porcentaje de asistencia del grupo ese día

**Ejemplo de resultado:**
```
╔══════════════════════════════════════════════════════════════╗
║ Grupo │ Fecha       │ Total Miemb. │ Asistieron │ % Asist. ║
╠══════════════════════════════════════════════════════════════╣
║   1   │ 01/10/2025  │     12       │     10     │  83.33%  ║
║   1   │ 02/10/2025  │     12       │      9     │  75.00%  ║
║   2   │ 01/10/2025  │     10       │      8     │  80.00%  ║
║   2   │ 02/10/2025  │     10       │     10     │ 100.00%  ║
║  ...  │    ...      │     ...      │    ...     │   ...    ║
╚══════════════════════════════════════════════════════════════╝
```

**Interpretación:**
- **100% Asistencia**: Todo el grupo asistió ese día (excelente)
- **<50% Asistencia**: Día con baja participación del grupo

**Uso práctico**: Identificar días donde grupos tienen baja asistencia para tomar acciones

**Exportación**: Solo Excel disponible

---

#### 4️⃣ Día de Mayor Asistencia

Identifica el día con más asistencias en el rango seleccionado.

**Pasos:**
1. Ajustar rango de fechas (se recomienda al menos 1 mes)
2. Click en **[Consultar]**

**Resultado mostrado (pantalla especial):**
```
╔═══════════════════════════════════════════════════════════════════════════╗
║                    DÍA DE MAYOR ASISTENCIA                                ║
╠═══════════════════════════════════════════════════════════════════════════╣
║                                                                           ║
║        FECHA                 TOTAL ASISTENCIAS      ASISTENTES DIFERENTES║
║                                                                           ║
║     15/10/2025                      75                       52          ║
║       Viernes                                                            ║
║                                                                           ║
╚═══════════════════════════════════════════════════════════════════════════╝
```

**Interpretación:**
- **Total Asistencias**: Número de ingresos registrados ese día
- **Asistentes Diferentes**: Cuántas personas únicas asistieron (sin contar reentradas)
- **Día de la semana**: Permite identificar patrones (ej: los viernes hay más asistencia)

**Uso práctico**: 
- Planificar eventos importantes en días de alta asistencia
- Identificar días menos concurridos para mantenimiento/actividades especiales

**Exportación**: No disponible (es un solo registro)

---

#### 5️⃣ Top Asistentes Más Constantes

Lista los N asistentes más constantes del período.

**Configuración:**
- **Top**: Ingresar número de asistentes a mostrar
  - Ejemplo: `10` (Top 10)
  - Por defecto: 10

**Pasos:**
1. Configurar **Top** (ej: `10` para Top 10)
2. Ajustar rango de fechas
3. Click en **[Consultar]**

**Columnas mostradas:**
- **Pos.**: Posición en el ranking (1, 2, 3, ...)
- **DNI**: Documento del asistente
- **Nombre Completo**: Nombres y apellidos
- **Categoría**: Estudiante, Miembro, etc.
- **Grupo**: Número de grupo (Miembros #) o vacío
- **Total Asist.**: Cuántas veces asistió
- **% Asist.**: Porcentaje de días asistidos vs días disponibles

**Ejemplo de resultado:**
```
╔═══════════════════════════════════════════════════════════════════════════════════╗
║ Pos. │ DNI      │ Nombre Completo      │ Categ. │ Grupo │ Total │ % Asist.      ║
╠═══════════════════════════════════════════════════════════════════════════════════╣
║  1   │ 12345678 │ Juan García López    │ Estud. │   1   │  25   │   83.33%      ║
║  2   │ 87654321 │ María Pérez Santos   │ Miemb. │   2   │  24   │   80.00%      ║
║  3   │ 11223344 │ Pedro Díaz Ruiz      │ Miemb. │   1   │  23   │   76.67%      ║
║  4   │ 99887766 │ Ana Martínez Villa   │ Amigo  │   -   │  22   │   73.33%      ║
║ ...  │ ...      │ ...                  │ ...    │  ...  │  ...  │    ...        ║
║ 10   │ 55443322 │ Luis Fernández Rey   │ Estud. │   3   │  18   │   60.00%      ║
╚═══════════════════════════════════════════════════════════════════════════════════╝
```

**Interpretación:**
- **Posición 1**: Asistente más constante
- **% Asist. alto (>70%)**: Compromiso excepcional
- **% Asist. moderado (50-70%)**: Buen nivel de participación

**Uso práctico**:
- Reconocer públicamente a los más constantes
- Identificar patrones de compromiso por categoría o grupo
- Motivar a otros asistentes

**Orden**: Resultados ordenados por **Total Asist.** descendente

**Exportación**: Solo Excel disponible

---

### Exportación de Reportes

#### Formatos Disponibles

**1. Excel (.xlsx)**
- **Software compatible**: Microsoft Excel, LibreOffice Calc, Google Sheets
- **Características**:
  - Headers con fondo rojo oscuro y texto blanco
  - Título en negrita tamaño 16
  - Período mostrado debajo del título
  - Columnas auto-ajustadas
  - Fila de totales en negrita (cuando aplique)
- **Ventajas**: Editable, permite análisis adicional, cálculos personalizados

**2. PDF (.pdf)**
- **Software compatible**: Cualquier visor de PDF (Adobe Reader, navegadores web)
- **Características**:
  - Formato A4 (21 x 29.7 cm)
  - Márgenes de 2 cm
  - Header con título, período y fecha de generación
  - Tablas con bordes
  - Footer con numeración de páginas
  - Color principal: Rojo (#8B0000 - tema MARTE)
- **Ventajas**: No editable, ideal para imprimir, compartir oficialmente

#### Proceso de Exportación

**Paso a paso:**

1. **Consultar datos primero**: Siempre hacer clic en **[Consultar]** antes de exportar
2. **Hacer clic en botón de exportación**: 
   - **[Exportar Excel]** o **[Exportar PDF]**
3. **Seleccionar ubicación**:
   - Aparecerá ventana de "Guardar Archivo"
   - Nombre sugerido: `NombreReporte_20251018_143500.xlsx`
   - Navegar a carpeta deseada (ej: Documentos, Escritorio)
   - Modificar nombre si se desea
   - Click en **[Guardar]**

4. **Esperar procesamiento**:
   - Aparecerá mensaje "Exportando..." en la parte inferior
   - Tiempo varía según cantidad de datos (1-10 segundos típicamente)

5. **Confirmación**:
   - Mensaje de éxito: "Reporte exportado exitosamente: [ruta completa]"
   - Click en **[Aceptar]**

6. **Abrir archivo**:
   - Navegar a la ubicación seleccionada
   - Abrir con Excel, visor PDF, etc.

**Ejemplo de nombres de archivo:**
```
TotalesDiarios_20251018_143522.xlsx
PorCategoria_20251018_143530.pdf
HistorialIndividual_12345678_20251018_143545.xlsx
Puntualidad_20251018_143600.xlsx
```

---

### Validaciones y Mensajes

#### Mensajes de Estado

**Durante consulta:**
- "Consultando..." - Procesando solicitud
- "Se encontraron X registros" - Consulta exitosa
- "No se encontraron registros para el período seleccionado" - Sin datos

**Durante exportación:**
- "Exportando..." - Generando archivo
- "Exportación completada" - Archivo guardado exitosamente

**Errores comunes:**
```
╔════════════════════════════════════════════╗
║         ❌ ERROR                           ║
╠════════════════════════════════════════════╣
║ Mensaje de error específico                ║
║                                            ║
║              [Aceptar]                     ║
╚════════════════════════════════════════════╝
```

Posibles errores:
- "Por favor ingrese un DNI válido"
- "Por favor ingrese Nombres y Apellidos"
- "La fecha de inicio debe ser menor o igual a la fecha de fin"
- "Error al consultar: [detalle técnico]"
- "Error al exportar: [detalle técnico]"

#### Validaciones Automáticas

**Rango de fechas:**
- ✅ Fecha Inicio ≤ Fecha Fin
- ❌ Si Fecha Inicio > Fecha Fin → Error al consultar

**DNI (Historial Individual):**
- ✅ DNI no vacío → Botones habilitados
- ❌ DNI vacío → Botones **[Consultar]** y **[Exportar]** deshabilitados (gris)

**Nombres y Apellidos:**
- ✅ Ambos campos llenos → Botón **[Consultar]** habilitado
- ❌ Algún campo vacío → Botón deshabilitado

**Hora Puntualidad:**
- ✅ Formato válido: `19:30`, `20:00`, `18:45`
- ❌ Formato inválido: `25:00`, `abc`, `19-30` → Error al consultar

**Top N:**
- ✅ Número entero positivo: `5`, `10`, `20`
- ❌ Número inválido: `-5`, `0`, `abc` → Error al consultar

---

### Preguntas Frecuentes (FAQ)

**P: ¿Puedo consultar datos de varios meses?**  
R: Sí, simplemente ajusta el rango de fechas. Por ejemplo, de 01/01/2025 a 31/12/2025 para todo el año.

**P: ¿Los reportes se actualizan en tiempo real?**  
R: No, debes hacer clic en **[Consultar]** cada vez que quieras ver datos actualizados.

**P: ¿Puedo exportar sin consultar primero?**  
R: No, siempre debes consultar los datos antes de exportar. De lo contrario, el reporte estará vacío.

**P: ¿Dónde se guardan los archivos exportados?**  
R: En la ubicación que selecciones en el diálogo "Guardar Archivo". Se recomienda crear una carpeta "Reportes MARTE" en Documentos.

**P: ¿Puedo modificar el archivo Excel exportado?**  
R: Sí, los archivos Excel son completamente editables. Puedes agregar fórmulas, gráficos, filtros, etc.

**P: ¿Puedo editar el PDF exportado?**  
R: No, los PDF son solo lectura. Si necesitas hacer cambios, usa la versión Excel.

**P: ¿Cuántos registros puedo exportar?**  
R: No hay límite definido. Sin embargo, con más de 10,000 registros, la exportación puede tardar varios segundos.

**P: ¿Por qué no aparecen datos en "Día de Mayor Asistencia"?**  
R: Asegúrate de tener al menos 1 asistencia registrada en el rango de fechas. Si no hay datos, se mostrará `--`.

**P: ¿Qué significa "% del Total" en Por Categoría?**  
R: Es el porcentaje de asistencias de esa categoría respecto al total de todas las categorías. Ejemplo: Si hay 100 asistencias totales y Estudiante tiene 40, su % es 40%.

**P: ¿Cómo interpreto el "Promedio" en Por Grupo?**  
R: Es el promedio de asistencias por miembro del grupo. Si el grupo tiene 10 miembros y hubo 80 asistencias, el promedio es 8 (cada miembro asistió 8 veces en promedio).

**P: ¿Puedo buscar por nombre parcial?**  
R: La búsqueda por nombre busca coincidencias exactas. Si no recuerdas el nombre completo, es mejor buscar por DNI.

**P: ¿El "Top 10" siempre muestra 10?**  
R: Muestra hasta 10 (o el número que configures). Si solo hay 7 asistentes en el período, mostrará solo 7.

**P: ¿Cómo se calcula el "% Puntual"?**  
R: Se divide el número de llegadas puntuales (antes de la hora límite) entre el total de asistencias y se multiplica por 100.

**P: ¿Qué pasa si cambio la "Hora Límite" después de consultar?**  
R: Debes hacer clic en **[Consultar]** nuevamente para recalcular con la nueva hora límite.

---

### Ejemplos Prácticos

#### Ejemplo 1: Reportar asistencia del mes pasado

**Objetivo**: Generar un reporte de totales diarios del mes de septiembre para presentar en reunión.

**Pasos:**
1. Abrir **Reportes y Estadísticas**
2. Configurar filtros:
   - Fecha Inicio: `01/09/2025`
   - Fecha Fin: `30/09/2025`
3. Tab **📊 Consultas Básicas** → Sección **Totales Diarios**
4. Click **[Consultar]**
5. Revisar tabla con 30 días
6. Click **[Exportar Excel]**
7. Guardar como: `ReporteSeptiembre2025.xlsx`
8. Abrir archivo y agregar gráfico de barras (si se desea)
9. Presentar en reunión

---

#### Ejemplo 2: Identificar asistentes más puntuales

**Objetivo**: Reconocer a los 5 asistentes más puntuales del trimestre.

**Pasos:**
1. Abrir **Reportes y Estadísticas**
2. Configurar filtros:
   - Fecha Inicio: `01/07/2025` (inicio del trimestre)
   - Fecha Fin: `30/09/2025` (fin del trimestre)
3. Tab **📈 Paneles Analíticos** → Sección **Tasa de Puntualidad**
4. Configurar Hora Límite: `19:30`
5. Click **[Consultar]**
6. Tabla se ordena automáticamente por % Puntual
7. Los primeros 5 son los más puntuales
8. Click **[Exportar Excel]**
9. Guardar y compartir con área de reconocimientos

---

#### Ejemplo 3: Analizar asistencia por categoría

**Objetivo**: Determinar qué categoría tiene mayor participación.

**Pasos:**
1. Abrir **Reportes y Estadísticas**
2. Configurar filtros:
   - Fecha Inicio: `01/01/2025` (inicio del año)
   - Fecha Fin: Fecha actual
3. Tab **📊 Consultas Básicas** → Sección **Por Categoría**
4. Click **[Consultar]**
5. Observar columna **% del Total**:
   - Estudiante: 45%
   - Miembro: 30%
   - Amigo: 15%
   - Externo: 10%
6. **Conclusión**: Los estudiantes son la categoría más activa
7. Exportar a PDF para presentar en informe anual

---

#### Ejemplo 4: Revisar historial de un asistente específico

**Objetivo**: Ver todas las asistencias de Juan García (DNI: 12345678) del último mes.

**Pasos:**
1. Abrir **Reportes y Estadísticas**
2. Configurar filtros:
   - Fecha Inicio: Hace 1 mes
   - Fecha Fin: Hoy
3. Tab **👤 Historial Individual**
4. Ingresar DNI: `12345678`
5. Click **[Consultar]**
6. Revisar tabla:
   - Asistió 15 días
   - Promedio de llegada: 19:35
   - Se quedó hasta el cierre 12 veces
7. Click **[Exportar PDF]**
8. Imprimir y archivar en expediente del asistente

---

### Consejos y Mejores Prácticas

**✅ HACER:**
- Consultar datos antes de exportar
- Usar rangos de fechas razonables (evitar rangos de varios años para reportes individuales)
- Nombrar archivos exportados con fechas descriptivas
- Crear carpeta dedicada para reportes (ej: `C:\Reportes MARTE\2025\`)
- Revisar la columna **Estado** para confirmar que la consulta fue exitosa
- Exportar a Excel si necesitas hacer cálculos adicionales
- Exportar a PDF si necesitas compartir oficialmente o imprimir
- Ajustar hora de puntualidad según necesidad (19:00, 19:30, 20:00, etc.)

**❌ EVITAR:**
- Intentar exportar sin consultar primero
- Dejar campos obligatorios vacíos (DNI, Nombres+Apellidos)
- Usar rangos de fechas inválidos (Fecha Inicio > Fecha Fin)
- Sobrescribir archivos importantes (verificar nombre antes de guardar)
- Hacer consultas muy amplias innecesariamente (enlentece el sistema)
- Compartir archivos con datos sensibles sin autorización

**💡 TIPS:**
- **Para reportes mensuales**: Usa siempre el día 1 y último día del mes
- **Para comparar períodos**: Exporta dos reportes (ej: enero y febrero) y compara en Excel
- **Para identificar tendencias**: Usa "Totales Diarios" con rango de 3-6 meses
- **Para evaluaciones individuales**: Combina "Historial Individual" + "Tasa de Puntualidad" + "Hasta Cierre"
- **Para planificación**: Usa "Día de Mayor Asistencia" para identificar mejores días para eventos
- **Para reconocimientos**: Usa "Top Constantes" trimestral o anual

---

## Módulo: Bitácora de Auditoría

### Descripción General

El módulo de Bitácora de Auditoría permite consultar y exportar el historial completo de todas las acciones realizadas en el sistema, proporcionando trazabilidad y transparencia en las operaciones.

### Acceso al Módulo

**Ubicación**: Ventana Principal → Sección "BITÁCORA DE AUDITORÍA" → Botón [Auditoría]

**Requisitos**:
- Usuario autenticado (cualquier rol)
- Sin permisos especiales requeridos

### Pantalla Principal

Al abrir el módulo, verás:

```
╔═══════════════════════════════════════════════════════════════╗
║            ⚔ BITÁCORA DE AUDITORÍA ⚔                          ║
╠═══════════════════════════════════════════════════════════════╣
║                                                               ║
║  FILTROS DE BÚSQUEDA                                         ║
║  ┌─────────────┬─────────────┬──────────────────────────┐   ║
║  │ Fecha Inicio│ Fecha Fin   │ Usuario                  │   ║
║  ├─────────────┼─────────────┼──────────────────────────┤   ║
║  │  [________] │ [________]  │ [_____________________]  │   ║
║  └─────────────┴─────────────┴──────────────────────────┘   ║
║                                                               ║
║  [🔍 Buscar]  [🗑 Limpiar Filtros]  [📋 Ver Todos]          ║
║                                                               ║
║  ┌──────────────────────────────────────────────────────┐   ║
║  │ Fecha/Hora    │ Usuario │ Acción      │ Entidad │...│   ║
║  ├──────────────────────────────────────────────────────┤   ║
║  │ 18/10/2025... │ admin   │ Crear Cat...│ Categoría │  │   ║
║  │ 18/10/2025... │ guardia │ Registrar...│ Asistencia│  │   ║
║  │ ...                                                   │   ║
║  └──────────────────────────────────────────────────────┘   ║
║                                                               ║
║  N registro(s)     [📄 Exportar a TXT]    [❌ Cerrar]       ║
╚═══════════════════════════════════════════════════════════════╝
```

### Funciones de Consulta

#### 1. Búsqueda por Rango de Fechas

**Objetivo**: Filtrar registros entre dos fechas específicas

**Pasos**:
1. Click en el campo **Fecha Inicio**
2. Seleccionar fecha de inicio en el calendario
3. Click en el campo **Fecha Fin**
4. Seleccionar fecha final
5. Click en botón **[🔍 Buscar]**

**Valores por Defecto**:
- Fecha Inicio: Hace 1 mes desde hoy
- Fecha Fin: Fecha actual

**Ejemplo**:
```
Fecha Inicio: 01/10/2025
Fecha Fin: 18/10/2025
→ Muestra todos los registros del 1 al 18 de octubre
```

#### 2. Búsqueda por Usuario

**Objetivo**: Ver solo las acciones de un usuario específico

**Pasos**:
1. En el campo **Usuario**, escribir el nombre del usuario
2. Click en botón **[🔍 Buscar]**

**Características**:
- Búsqueda parcial (no requiere nombre completo)
- No distingue mayúsculas/minúsculas
- Se puede combinar con filtro de fechas

**Ejemplo**:
```
Usuario: "admin"
→ Muestra todas las acciones del usuario "admin"
```

#### 3. Búsqueda Combinada

**Pasos**:
1. Establecer **Fecha Inicio** y **Fecha Fin**
2. Escribir nombre en campo **Usuario**
3. Click en **[🔍 Buscar]**

**Ejemplo**:
```
Fecha Inicio: 15/10/2025
Fecha Fin: 18/10/2025
Usuario: "guardia"
→ Muestra solo acciones del usuario "guardia" entre esas fechas
```

#### 4. Ver Todos los Registros

**Objetivo**: Cargar todo el historial sin filtros

**Pasos**:
1. Click en botón **[📋 Ver Todos]**

⚠️ **Advertencia**: Si hay muchos registros, puede tardar en cargar

#### 5. Limpiar Filtros

**Objetivo**: Resetear los criterios de búsqueda

**Pasos**:
1. Click en botón **[🗑 Limpiar Filtros]**

**Efecto**:
- Fecha Inicio: Se resetea a hace 1 mes
- Fecha Fin: Se resetea a hoy
- Usuario: Campo se limpia

### Columnas de Información

| Columna | Descripción | Ejemplo |
|---------|-------------|---------|
| **Fecha/Hora** | Timestamp exacto de la acción | 18/10/2025 14:30:45 |
| **Usuario** | Nombre del usuario que ejecutó | admin |
| **Acción** | Descripción de la operación | Crear Categoría |
| **Entidad** | Tipo de dato afectado | Categoría |
| **Detalle** | Información adicional | Nombre: Miembros |

### Exportación a TXT

#### Generar Archivo de Auditoría

**Objetivo**: Crear archivo de texto con el historial actual

**Pasos**:
1. Realizar búsqueda con los filtros deseados
2. Verificar que la tabla muestre los registros correctos
3. Click en botón **[📄 Exportar a TXT]**
4. En el diálogo:
   - Elegir ubicación para guardar
   - (Opcional) Modificar nombre del archivo
   - Formato sugerido: `AuditoriaMarte_YYYYMMDD_HHmmss.txt`
5. Click en **[Guardar]**

**Resultado**: Aparece mensaje de confirmación con la ruta del archivo

#### Estructura del Archivo TXT

```
╔═══════════════════════════════════════════════════════════════════╗
║           SISTEMA MARTE - BITÁCORA DE AUDITORÍA                   ║
╚═══════════════════════════════════════════════════════════════════╝

Fecha de Generación: 18/10/2025 14:35:20
Total de Registros: 125

═══════════════════════════════════════════════════════════════════

Fecha/Hora: 18/10/2025 14:30:00
Usuario: admin
Acción: Crear Categoría
Entidad: Categoría
Detalle: Nombre: Miembros, Descripción: Miembros activos
───────────────────────────────────────────────────────────────────

Fecha/Hora: 18/10/2025 14:25:00
Usuario: guardia
Acción: Registrar Asistencia
Entidad: Asistencia
Detalle: DNI: 12345678, Hora: 14:25
───────────────────────────────────────────────────────────────────

[Más registros...]

╔═══════════════════════════════════════════════════════════════════╗
║                         FIN DEL REPORTE                           ║
╚═══════════════════════════════════════════════════════════════════╝
```

### Casos de Uso Comunes

#### 1. Revisar Actividad Diaria

**Escenario**: Ver todas las acciones de hoy

**Pasos**:
1. Establecer ambas fechas a hoy
2. Click en **[🔍 Buscar]**

#### 2. Auditoría de Usuario Específico

**Escenario**: Revisar qué hizo un usuario en un periodo

**Pasos**:
1. Seleccionar rango de fechas
2. Escribir nombre del usuario
3. Click en **[🔍 Buscar]**
4. Click en **[📄 Exportar a TXT]** para guardar evidencia

#### 3. Investigación de Cambios

**Escenario**: Encontrar cuándo se modificó un dato

**Pasos**:
1. Establecer rango amplio de fechas
2. Click en **[🔍 Buscar]**
3. Buscar visualmente en la columna "Acción" y "Detalle"

#### 4. Reporte Mensual

**Escenario**: Generar reporte del mes pasado

**Pasos**:
1. Fecha Inicio: Primer día del mes anterior
2. Fecha Fin: Último día del mes anterior
3. Click en **[🔍 Buscar]**
4. Click en **[📄 Exportar a TXT]**

#### 5. Cumplimiento Normativo

**Escenario**: Documentar operaciones para auditoría externa

**Pasos**:
1. Click en **[📋 Ver Todos]** para cargar historial completo
2. Click en **[📄 Exportar a TXT]**
3. Entregar archivo generado

### Tipos de Acciones Registradas

El sistema registra automáticamente:

**Autenticación**:
- Inicio de sesión (Login)
- Cierre de sesión (Logout)

**Usuarios**:
- Crear Usuario
- Actualizar Usuario
- Eliminar Usuario
- Cambiar Contraseña

**Categorías**:
- Crear Categoría
- Actualizar Categoría
- Eliminar Categoría

**Asistentes**:
- Crear Asistente
- Actualizar Asistente
- Eliminar Asistente
- Cambiar Estado

**Asistencias**:
- Registrar Asistencia
- Cierre Automático de Día

**Configuración**:
- Actualizar Hora de Cierre

### Consejos y Buenas Prácticas

✅ **Recomendaciones**:
- Exportar reportes mensuales para respaldo
- Usar filtros específicos en lugar de "Ver Todos"
- Revisar regularmente la actividad de usuarios
- Guardar archivos TXT con nombres descriptivos
- Mantener un archivo de auditorías importantes

⚠️ **Advertencias**:
- No borrar registros de auditoría
- Los registros son de solo lectura
- Las consultas NO generan nuevos registros
- Archivos TXT grandes pueden tardar en generarse

### Solución de Problemas

#### No Aparecen Registros

**Causas Posibles**:
1. Rango de fechas muy restrictivo
2. Usuario escrito incorrectamente
3. No hay actividad en ese periodo

**Solución**:
- Click en **[🗑 Limpiar Filtros]**
- Click en **[📋 Ver Todos]**
- Verificar si existen registros

#### Error al Exportar

**Causa**: Falta de permisos en la carpeta de destino

**Solución**:
- Elegir otra ubicación (ej: Escritorio)
- Verificar permisos de escritura
- Cerrar archivo TXT si está abierto

#### Carga Lenta

**Causa**: Demasiados registros

**Solución**:
- Usar filtros más específicos
- Dividir búsqueda por meses
- Exportar por periodos pequeños

---

## Cerrar Sesión

### Cómo Cerrar Sesión Correctamente

1. En la ventana principal, hacer clic en **[Cerrar Sesión]** (esquina superior derecha)
2. Aparecerá un mensaje de confirmación:

```
╔════════════════════════════════════╗
║      ❓ CERRAR SESIÓN              ║
╠════════════════════════════════════╣
║ ¿Está seguro que desea cerrar      ║
║ sesión?                            ║
║                                    ║
║     [SÍ]          [NO]             ║
╚════════════════════════════════════╝
```

3. Hacer clic en **[SÍ]**
4. Se registra el cierre de sesión en la auditoría
5. Regresa a la ventana de Login

⚠️ **Importante**: Siempre cierre sesión al terminar de usar el sistema, especialmente en computadoras compartidas.

---

## Solución de Problemas Comunes

### Problema 1: No puedo editar los datos del usuario

**Síntoma**: Al seleccionar un usuario, veo sus datos pero no puedo modificarlos.

**Solución**: 
1. Los datos se muestran en modo **solo lectura** por defecto
2. Debe hacer clic en el botón **[EDITAR]** para habilitar la edición
3. Una vez habilitado, los campos cambiarán de color y podrán modificarse

### Problema 2: El botón GUARDAR está deshabilitado

**Posibles causas**:
- ❌ Faltan campos obligatorios por completar
- ❌ Las contraseñas no coinciden (en modo nuevo usuario)
- ❌ No está en modo edición/nuevo (debe hacer clic en NUEVO o EDITAR primero)

**Solución**: Complete todos los campos requeridos y verifique que las contraseñas coincidan.

### Problema 3: "El nombre de usuario ya existe"

**Causa**: Está intentando crear un usuario con un nombre que ya está en uso.

**Solución**: Elija un nombre de usuario diferente y único.

### Problema 4: No veo el módulo de Configuración

**Causa**: Su usuario no tiene permisos de Administrador.

**Solución**: Contacte a un administrador para que le asigne el rol correcto.

### Problema 5: Error al conectar con la base de datos

**Síntoma**: Mensaje "Error al cargar datos"

**Soluciones**:
1. Verificar que SQL Server esté en ejecución
2. Comprobar conexión de red
3. Contactar al administrador del sistema

---

## Buenas Prácticas

### Seguridad

✅ **DO (Hacer)**:
- Cerrar sesión al terminar de usar el sistema
- Cambiar contraseñas periódicamente
- Usar contraseñas fuertes (mínimo 8 caracteres, combinando mayúsculas, minúsculas, números)
- Mantener credenciales confidenciales
- Verificar dos veces antes de deshabilitar o eliminar registros

❌ **DON'T (No hacer)**:
- Compartir sus credenciales con otros usuarios
- Dejar la sesión abierta en computadoras compartidas
- Usar contraseñas simples o predecibles
- Anotar contraseñas en lugares visibles

### Gestión de Usuarios

✅ **Recomendaciones**:
- Deshabilitar usuarios en lugar de eliminarlos (permite auditoría histórica)
- Asignar roles según las funciones reales del usuario
- Documentar cambios importantes (cambios de rol, habilitaciones)
- Revisar periódicamente la lista de usuarios activos
- Comunicar cambios de contraseña de forma segura

### Gestión de Categorías

✅ **Recomendaciones**:
- Usar nombres descriptivos y consistentes
- Mantener la numeración de grupos organizada (#01, #02, etc.)
- Verificar que la categoría no esté en uso antes de eliminarla
- Documentar el propósito de cada categoría nueva

---

## Glosario de Términos

**Administrador**: Usuario con permisos completos en el sistema.

**Asistencia**: Registro de entrada y salida de asistentes en una fecha específica.

**Asistente**: Persona registrada que participa en actividades de la organización.

**Auditoría**: Registro automático de todas las operaciones realizadas en el sistema.

**Categoría**: Clasificación de asistentes según su relación con la organización.

**Cierre Automático**: Función que registra la salida de todos los asistentes presentes usando la hora de cierre configurada.

**Dashboard**: Panel principal con indicadores y métricas del sistema.

**Deshabilitar**: Suspender temporalmente el acceso de un usuario sin eliminar sus datos.

**DNI**: Documento Nacional de Identidad o código único que identifica a cada asistente.

**Guardia**: Usuario con permisos operativos limitados (registro de asistencia, consultas).

**Hora de Cierre**: Hora oficial en que finaliza la jornada de actividades.

**Hora de Ingreso**: Momento exacto en que un asistente registra su llegada.

**Hora de Salida**: Momento exacto en que un asistente registra su partida.

**RBAC**: Control de acceso basado en roles (Role-Based Access Control).

**Soft Delete**: Eliminación lógica que marca registros como inactivos sin borrarlos físicamente.

**Bitácora de Auditoría**: Registro cronológico de todas las acciones realizadas en el sistema.

**Trazabilidad**: Capacidad de rastrear y documentar el historial de cambios y operaciones.

---

**Última actualización**: 18 de octubre de 2025  
**Versión del documento**: BETA  
**Para soporte técnico**: Contacte al administrador del sistema Andy Agurto 
