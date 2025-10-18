using Marte.Application.Interfaces;
using Marte.Domain.Entities;
using Marte.Infrastructure.Interfaces;
using System.IO;
using System.Text;

namespace Marte.Application.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _auditLogRepository;

        public AuditLogService(IAuditLogRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        public async Task<IEnumerable<AuditLog>> GetAllAsync()
        {
            return await _auditLogRepository.GetAllAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetByUsuarioAsync(string usuario)
        {
            return await _auditLogRepository.GetByUsuarioAsync(usuario);
        }

        public async Task<IEnumerable<AuditLog>> GetByFechaRangoAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _auditLogRepository.GetByFechaRangoAsync(fechaInicio, fechaFin);
        }

        public async Task<IEnumerable<AuditLog>> GetByFiltrosAsync(DateTime? fechaInicio, DateTime? fechaFin, string? usuario)
        {
            return await _auditLogRepository.GetByFiltrosAsync(fechaInicio, fechaFin, usuario);
        }

        public async Task ExportarATxtAsync(IEnumerable<AuditLog> logs, string rutaArchivo)
        {
            var sb = new StringBuilder();
            
            // Encabezado
            sb.AppendLine("╔═══════════════════════════════════════════════════════════════════════════════╗");
            sb.AppendLine("║                     SISTEMA MARTE - BITÁCORA DE AUDITORÍA                    ║");
            sb.AppendLine("╚═══════════════════════════════════════════════════════════════════════════════╝");
            sb.AppendLine();
            sb.AppendLine($"Fecha de Generación: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            sb.AppendLine($"Total de Registros: {logs.Count()}");
            sb.AppendLine();
            sb.AppendLine("═══════════════════════════════════════════════════════════════════════════════");
            sb.AppendLine();

            foreach (var log in logs.OrderByDescending(l => l.Fecha))
            {
                sb.AppendLine($"Fecha/Hora: {log.Fecha:dd/MM/yyyy HH:mm:ss}");
                sb.AppendLine($"Usuario: {log.Usuario}");
                sb.AppendLine($"Acción: {log.Accion}");
                sb.AppendLine($"Entidad: {log.Entidad}");
                
                if (!string.IsNullOrWhiteSpace(log.Detalle))
                {
                    sb.AppendLine($"Detalle: {log.Detalle}");
                }
                
                sb.AppendLine("───────────────────────────────────────────────────────────────────────────────");
                sb.AppendLine();
            }

            sb.AppendLine();
            sb.AppendLine("╔═══════════════════════════════════════════════════════════════════════════════╗");
            sb.AppendLine("║                            FIN DEL REPORTE                                    ║");
            sb.AppendLine("╚═══════════════════════════════════════════════════════════════════════════════╝");

            await File.WriteAllTextAsync(rutaArchivo, sb.ToString(), Encoding.UTF8);
        }
    }
}
