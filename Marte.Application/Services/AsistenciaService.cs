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

        public AsistenciaService(
            IAsistenciaRepository asistenciaRepository,
            IAsistenteRepository asistenteRepository,
            IAuditLogRepository auditLogRepository,
            IConfiguracionEscuelaRepository configuracionRepository)
        {
            _asistenciaRepository = asistenciaRepository;
            _asistenteRepository = asistenteRepository;
            _auditLogRepository = auditLogRepository;
            _configuracionRepository = configuracionRepository;
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
    }
}
