using Marte.Domain.Entities;

namespace Marte.Infrastructure.Interfaces
{
    public interface IAuditLogRepository
    {
        Task AddAsync(AuditLog auditLog);
        Task<IEnumerable<AuditLog>> GetAllAsync();
        Task<IEnumerable<AuditLog>> GetByUsuarioAsync(string usuario);
        Task<IEnumerable<AuditLog>> GetByEntidadAsync(string entidad, Guid entidadId);
        Task<IEnumerable<AuditLog>> GetByFechaRangoAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<IEnumerable<AuditLog>> GetByFiltrosAsync(DateTime? fechaInicio, DateTime? fechaFin, string? usuario);
        Task SaveChangesAsync();
    }
}
