using Marte.Domain.Entities;
using Marte.Infrastructure.Data;
using Marte.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Marte.Infrastructure.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly MarteDbContext _context;

        public AuditLogRepository(MarteDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(AuditLog auditLog)
        {
            await _context.AuditLogs.AddAsync(auditLog);
        }

        public async Task<IEnumerable<AuditLog>> GetAllAsync()
        {
            return await _context.AuditLogs
                .OrderByDescending(a => a.Fecha)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetByUsuarioAsync(string usuario)
        {
            return await _context.AuditLogs
                .Where(a => a.Usuario == usuario)
                .OrderByDescending(a => a.Fecha)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetByEntidadAsync(string entidad, Guid entidadId)
        {
            return await _context.AuditLogs
                .Where(a => a.Entidad == entidad && a.EntidadId == entidadId)
                .OrderByDescending(a => a.Fecha)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetByFechaRangoAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _context.AuditLogs
                .Where(a => a.Fecha >= fechaInicio && a.Fecha <= fechaFin)
                .OrderByDescending(a => a.Fecha)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetByFiltrosAsync(DateTime? fechaInicio, DateTime? fechaFin, string? usuario)
        {
            var query = _context.AuditLogs.AsQueryable();

            if (fechaInicio.HasValue)
                query = query.Where(a => a.Fecha >= fechaInicio.Value);

            if (fechaFin.HasValue)
                query = query.Where(a => a.Fecha <= fechaFin.Value);

            if (!string.IsNullOrWhiteSpace(usuario))
                query = query.Where(a => a.Usuario.Contains(usuario));

            return await query.OrderByDescending(a => a.Fecha).ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
