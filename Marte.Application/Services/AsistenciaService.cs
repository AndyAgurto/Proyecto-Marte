using Marte.Application.Interfaces;
using Marte.Domain.Entities;
using Marte.Infrastructure.Interfaces;

namespace Marte.Application.Services
{
    public class AsistenciaService : IAsistenciaService
    {
        private readonly IAsistenciaRepository _asistenciaRepository;
        private readonly IAsistenteRepository _asistenteRepository;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IConfiguracionEscuelaRepository _configuracionRepository;
        private readonly ICategoriaRepository _categoriaRepository;

        public AsistenciaService(
            IAsistenciaRepository asistenciaRepository,
            IAsistenteRepository asistenteRepository,
            IAuditLogRepository auditLogRepository,
            IConfiguracionEscuelaRepository configuracionRepository,
            ICategoriaRepository categoriaRepository)
        {
            _asistenciaRepository = asistenciaRepository;
            _asistenteRepository = asistenteRepository;
            _auditLogRepository = auditLogRepository;
            _configuracionRepository = configuracionRepository;
            _categoriaRepository = categoriaRepository;
        }

        public async Task<(bool Success, string Message)> RegistrarIngresoAsync(string dni, string registradoPor)
        {
            try
            {
                // Validar que el DNI no esté vacío
                if (string.IsNullOrWhiteSpace(dni))
                {
                    return (false, "El DNI/Código es obligatorio.");
                }

                // Buscar al asistente por DNI
                var asistente = await _asistenteRepository.GetByDNIAsync(dni);
                if (asistente == null)
                {
                    return (false, $"No existe un asistente registrado con DNI/Código: {dni}. Debe registrarse primero en el módulo de Gestión de Asistentes.");
                }

                // Validar que el asistente esté activo
                if (!asistente.Estado)
                {
                    return (false, $"El asistente {asistente.Nombres} {asistente.Apellidos} está inactivo. No puede registrar asistencia.");
                }

                // Verificar si ya tiene alguna asistencia hoy (con o sin salida)
                var tieneAsistenciaHoy = await _asistenciaRepository.ExisteAsistenciaHoyAsync(asistente.Id);
                if (tieneAsistenciaHoy)
                {
                    return (false, $"El asistente {asistente.Nombres} {asistente.Apellidos} ya registró su ingreso hoy. Solo se permite un ingreso por día.");
                }

                // Crear el registro de asistencia
                var asistencia = new Asistencia
                {
                    AsistenteId = asistente.Id,
                    Fecha = DateTime.Today,
                    HoraIngreso = DateTime.Now,
                    HoraSalida = null,
                    HastaCierre = false
                };

                await _asistenciaRepository.AddAsync(asistencia);
                await _asistenciaRepository.SaveChangesAsync();

                // Auditoría
                var auditLog = new AuditLog
                {
                    Accion = "Registrar Ingreso",
                    Entidad = "Asistencia",
                    Detalle = $"Ingreso registrado para {asistente.Nombres} {asistente.Apellidos} (DNI: {dni}) a las {asistencia.HoraIngreso:HH:mm:ss}",
                    Usuario = registradoPor,
                    Fecha = DateTime.Now
                };
                await _auditLogRepository.AddAsync(auditLog);
                await _auditLogRepository.SaveChangesAsync();

                return (true, $"Ingreso registrado exitosamente para {asistente.Nombres} {asistente.Apellidos} a las {asistencia.HoraIngreso:HH:mm:ss}");
            }
            catch (Exception ex)
            {
                return (false, $"Error al registrar ingreso: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> RegistrarSalidaAsync(Guid asistenciaId, string registradoPor, string? observacion = null)
        {
            try
            {
                var asistencia = await _asistenciaRepository.GetByIdAsync(asistenciaId);
                if (asistencia == null)
                {
                    return (false, "No se encontró el registro de asistencia.");
                }

                // Validar que no tenga ya una salida registrada
                if (asistencia.HoraSalida != null)
                {
                    return (false, "Este registro ya tiene una salida registrada.");
                }

                var horaSalida = DateTime.Now;

                // Validar que la hora de salida no sea menor que la hora de ingreso
                if (horaSalida < asistencia.HoraIngreso)
                {
                    return (false, "La hora de salida no puede ser menor que la hora de ingreso.");
                }

                // Registrar salida
                asistencia.HoraSalida = horaSalida;
                asistencia.Observacion = observacion;

                await _asistenciaRepository.UpdateAsync(asistencia);
                await _asistenciaRepository.SaveChangesAsync();

                // Auditoría
                var auditLog = new AuditLog
                {
                    Accion = "Registrar Salida",
                    Entidad = "Asistencia",
                    Detalle = $"Salida registrada para {asistencia.Asistente.Nombres} {asistencia.Asistente.Apellidos} a las {horaSalida:HH:mm:ss}. Observación: {observacion ?? "Ninguna"}",
                    Usuario = registradoPor,
                    Fecha = DateTime.Now
                };
                await _auditLogRepository.AddAsync(auditLog);
                await _auditLogRepository.SaveChangesAsync();

                return (true, $"Salida registrada exitosamente para {asistencia.Asistente.Nombres} {asistencia.Asistente.Apellidos} a las {horaSalida:HH:mm:ss}");
            }
            catch (Exception ex)
            {
                return (false, $"Error al registrar salida: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message, int Actualizados)> AplicarCierreAutomaticoAsync(string aplicadoPor)
        {
            try
            {
                // Obtener la hora de cierre de la configuración
                var configuracion = await _configuracionRepository.GetConfiguracionAsync();
                if (configuracion == null)
                {
                    return (false, "No se encontró la configuración de hora de cierre.", 0);
                }

                // Obtener todas las asistencias abiertas de hoy
                var asistenciasAbiertas = await _asistenciaRepository.GetAsistenciasPresentesAsync();
                var asistenciasParaCerrar = asistenciasAbiertas.ToList();

                if (!asistenciasParaCerrar.Any())
                {
                    return (true, "No hay asistencias abiertas para cerrar.", 0);
                }

                // Crear la hora de cierre completa (fecha de hoy + hora configurada)
                var hoy = DateTime.Today;
                var horaCierre = hoy.Add(configuracion.HoraCierre);

                int contador = 0;
                foreach (var asistencia in asistenciasParaCerrar)
                {
                    asistencia.HoraSalida = horaCierre;
                    asistencia.HastaCierre = true;
                    asistencia.Observacion = "Cierre automático";
                    
                    await _asistenciaRepository.UpdateAsync(asistencia);
                    contador++;
                }

                await _asistenciaRepository.SaveChangesAsync();

                // Auditoría
                var auditLog = new AuditLog
                {
                    Accion = "Cierre Automático",
                    Entidad = "Asistencia",
                    Detalle = $"Se aplicó cierre automático a {contador} asistencias abiertas con hora de cierre: {horaCierre:HH:mm:ss}",
                    Usuario = aplicadoPor,
                    Fecha = DateTime.Now
                };
                await _auditLogRepository.AddAsync(auditLog);
                await _auditLogRepository.SaveChangesAsync();

                return (true, $"Cierre automático aplicado exitosamente a {contador} asistencia(s) con hora {horaCierre:HH:mm}.", contador);
            }
            catch (Exception ex)
            {
                return (false, $"Error al aplicar cierre automático: {ex.Message}", 0);
            }
        }

        public async Task<IEnumerable<Asistencia>> GetAsistenciasPresentesAsync()
        {
            return await _asistenciaRepository.GetAsistenciasPresentesAsync();
        }

        public async Task<IEnumerable<Asistencia>> GetAsistenciasByFechaAsync(DateTime fecha)
        {
            return await _asistenciaRepository.GetByFechaAsync(fecha);
        }

        public async Task<IEnumerable<Asistencia>> GetAllAsistenciasAsync()
        {
            return await _asistenciaRepository.GetAllAsync();
        }

        public async Task<(bool Success, string Message)> RegistrarIngresoDinamicoAsync(string busqueda, string registradoPor)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(busqueda))
                {
                    return (false, "Debe ingresar un DNI, código o nombre para buscar.");
                }

                // Normalizar búsqueda
                var busquedaNormalizada = RemoverTildes(busqueda.ToLower().Trim());

                // Buscar asistente por DNI primero (búsqueda exacta)
                var asistente = await _asistenteRepository.GetByDNIAsync(busqueda.Trim());

                // Si no se encuentra por DNI, buscar por nombre
                if (asistente == null)
                {
                    var asistentes = await _asistenteRepository.GetActivosAsync();
                    var asistentesFiltrados = asistentes.Where(a =>
                        RemoverTildes(a.Nombres.ToLower()).Contains(busquedaNormalizada) ||
                        RemoverTildes(a.Apellidos.ToLower()).Contains(busquedaNormalizada) ||
                        RemoverTildes($"{a.Nombres} {a.Apellidos}".ToLower()).Contains(busquedaNormalizada) ||
                        RemoverTildes($"{a.Apellidos} {a.Nombres}".ToLower()).Contains(busquedaNormalizada)
                    ).ToList();

                    if (!asistentesFiltrados.Any())
                    {
                        return (false, $"No se encontró ningún asistente con: '{busqueda}'. Verifique o use 'Registro Temporal' para visitantes.");
                    }

                    if (asistentesFiltrados.Count > 1)
                    {
                        var nombres = string.Join(", ", asistentesFiltrados.Take(5).Select(a => $"{a.Apellidos} {a.Nombres}"));
                        return (false, $"Se encontraron {asistentesFiltrados.Count} asistentes. Sea más específico. Ejemplos: {nombres}");
                    }

                    asistente = asistentesFiltrados.First();
                }

                // Registrar ingreso del asistente encontrado
                return await RegistrarIngresoAsync(asistente.DNI, registradoPor);
            }
            catch (Exception ex)
            {
                return (false, $"Error en búsqueda dinámica: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> RegistrarIngresoTemporalAsync(string nombres, string apellidos, string registradoPor)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombres) || string.IsNullOrWhiteSpace(apellidos))
                {
                    return (false, "Nombres y apellidos son obligatorios para registro temporal.");
                }

                // Buscar o crear categoría "Temporales" para asistentes temporales
                var categorias = await _categoriaRepository.GetAllAsync();
                var categoriaTemporales = categorias.FirstOrDefault(c => c.Nombre == "Temporales");

                if (categoriaTemporales == null)
                {
                    // Crear automáticamente la categoría "Temporales"
                    categoriaTemporales = new Categoria
                    {
                        Nombre = "Temporales"
                    };
                    categoriaTemporales = await _categoriaRepository.CreateAsync(categoriaTemporales);
                }

                // Generar DNI temporal único (TEMP-YYYYMMDDHHMMSS)
                var dniTemporal = $"TEMP-{DateTime.Now:yyyyMMddHHmmss}";

                // Crear asistente temporal
                var asistenteTemp = new Asistente
                {
                    Nombres = nombres.Trim(),
                    Apellidos = apellidos.Trim(),
                    DNI = dniTemporal,
                    CategoriaId = categoriaTemporales.Id,
                    NumeroGrupo = null,
                    Estado = true
                };

                await _asistenteRepository.CreateAsync(asistenteTemp);

                // Crear asistencia
                var asistencia = new Asistencia
                {
                    AsistenteId = asistenteTemp.Id,
                    Fecha = DateTime.Today,
                    HoraIngreso = DateTime.Now,
                    HoraSalida = null,
                    HastaCierre = false,
                    Observacion = "Visitante temporal"
                };

                await _asistenciaRepository.AddAsync(asistencia);
                await _asistenciaRepository.SaveChangesAsync();

                // Auditoría
                var auditLog = new AuditLog
                {
                    Accion = "Registro Temporal",
                    Entidad = "Asistencia",
                    Detalle = $"Visitante temporal registrado: {nombres} {apellidos} (ID: {dniTemporal}) a las {asistencia.HoraIngreso:HH:mm:ss}",
                    Usuario = registradoPor,
                    Fecha = DateTime.Now
                };
                await _auditLogRepository.AddAsync(auditLog);
                await _auditLogRepository.SaveChangesAsync();

                return (true, $"Visitante temporal '{nombres} {apellidos}' registrado exitosamente a las {asistencia.HoraIngreso:HH:mm:ss}");
            }
            catch (Exception ex)
            {
                return (false, $"Error al registrar visitante temporal: {ex.Message}");
            }
        }

        public async Task<IEnumerable<Asistencia>> BuscarAsistenciasPorNombreAsync(string busqueda, DateTime fecha)
        {
            if (string.IsNullOrWhiteSpace(busqueda))
                return new List<Asistencia>();

            var asistencias = await _asistenciaRepository.GetByFechaAsync(fecha);
            var busquedaNormalizada = RemoverTildes(busqueda.ToLower().Trim());

            return asistencias.Where(a =>
                RemoverTildes(a.Asistente.Nombres.ToLower()).Contains(busquedaNormalizada) ||
                RemoverTildes(a.Asistente.Apellidos.ToLower()).Contains(busquedaNormalizada) ||
                RemoverTildes($"{a.Asistente.Nombres} {a.Asistente.Apellidos}".ToLower()).Contains(busquedaNormalizada) ||
                a.Asistente.DNI.Contains(busqueda, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        private string RemoverTildes(string texto)
        {
            var textoNormalizado = texto.Normalize(System.Text.NormalizationForm.FormD);
            var chars = textoNormalizado.Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark).ToArray();
            return new string(chars).Normalize(System.Text.NormalizationForm.FormC);
        }
    }
}
