using Marte.Domain.Entities;

namespace Marte.Application.Interfaces
{
    public interface IAuditLogService
    {
        Task<IEnumerable<AuditLog>> GetAllAsync();
        Task<IEnumerable<AuditLog>> GetByUsuarioAsync(string usuario);
        Task<IEnumerable<AuditLog>> GetByFechaRangoAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<IEnumerable<AuditLog>> GetByFiltrosAsync(DateTime? fechaInicio, DateTime? fechaFin, string? usuario);
        Task ExportarATxtAsync(IEnumerable<AuditLog> logs, string rutaArchivo);
    }
}
