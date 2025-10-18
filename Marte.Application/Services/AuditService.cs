using Marte.Domain.Entities;
using Marte.Infrastructure.Interfaces;

namespace Marte.Application.Services
{
    public interface IAuditService
    {
        Task LogActionAsync(string entidad, Guid entidadId, string accion, string usuario, string? detalle = null);
        Task<IEnumerable<AuditLog>> GetAllLogsAsync();
        Task<IEnumerable<AuditLog>> GetUserLogsAsync(string usuario);
    }

    public class AuditService : IAuditService
    {
        private readonly IAuditLogRepository _auditLogRepository;

        public AuditService(IAuditLogRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        public async Task LogActionAsync(string entidad, Guid entidadId, string accion, string usuario, string? detalle = null)
        {
            var auditLog = new AuditLog
            {
                Entidad = entidad,
                EntidadId = entidadId,
                Accion = accion,
                Usuario = usuario,
                Fecha = DateTime.Now,
                Detalle = detalle
            };

            await _auditLogRepository.AddAsync(auditLog);
            await _auditLogRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetAllLogsAsync()
        {
            return await _auditLogRepository.GetAllAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetUserLogsAsync(string usuario)
        {
            return await _auditLogRepository.GetByUsuarioAsync(usuario);
        }
    }
}
