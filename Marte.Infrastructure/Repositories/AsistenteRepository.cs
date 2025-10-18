using Microsoft.EntityFrameworkCore;
using Marte.Domain.Entities;
using Marte.Infrastructure.Data;
using Marte.Infrastructure.Interfaces;

namespace Marte.Infrastructure.Repositories
{
    public class AsistenteRepository : IAsistenteRepository
    {
        private readonly MarteDbContext _context;

        public AsistenteRepository(MarteDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Asistente>> GetAllAsync()
        {
            return await _context.Asistentes
                .Include(a => a.Categoria)
                .OrderBy(a => a.Apellidos)
                .ThenBy(a => a.Nombres)
                .ToListAsync();
        }

        public async Task<IEnumerable<Asistente>> GetActivosAsync()
        {
            return await _context.Asistentes
                .Include(a => a.Categoria)
                .Where(a => a.Estado)
                .OrderBy(a => a.Apellidos)
                .ThenBy(a => a.Nombres)
                .ToListAsync();
        }

        public async Task<Asistente?> GetByIdAsync(Guid id)
        {
            return await _context.Asistentes
                .Include(a => a.Categoria)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Asistente?> GetByDNIAsync(string dni)
        {
            return await _context.Asistentes
                .Include(a => a.Categoria)
                .FirstOrDefaultAsync(a => a.DNI == dni);
        }

        public async Task<bool> ExisteDNIAsync(string dni, Guid? excludeId = null)
        {
            var query = _context.Asistentes.Where(a => a.DNI == dni);
            
            if (excludeId.HasValue)
            {
                query = query.Where(a => a.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<Asistente> CreateAsync(Asistente asistente)
        {
            asistente.FechaCreacion = DateTime.Now;
            _context.Asistentes.Add(asistente);
            await _context.SaveChangesAsync();
            
            // Recargar con la categoría
            await _context.Entry(asistente)
                .Reference(a => a.Categoria)
                .LoadAsync();
                
            return asistente;
        }

        public async Task<Asistente> UpdateAsync(Asistente asistente)
        {
            asistente.FechaModificacion = DateTime.Now;
            _context.Asistentes.Update(asistente);
            await _context.SaveChangesAsync();
            
            // Recargar con la categoría
            await _context.Entry(asistente)
                .Reference(a => a.Categoria)
                .LoadAsync();
                
            return asistente;
        }

        public async Task DeleteAsync(Guid id)
        {
            var asistente = await _context.Asistentes.FindAsync(id);
            if (asistente != null)
            {
                _context.Asistentes.Remove(asistente);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> InactivarAsync(Guid id)
        {
            var asistente = await _context.Asistentes.FindAsync(id);
            if (asistente != null)
            {
                asistente.Estado = false;
                asistente.FechaModificacion = DateTime.Now;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> ActivarAsync(Guid id)
        {
            var asistente = await _context.Asistentes.FindAsync(id);
            if (asistente != null)
            {
                asistente.Estado = true;
                asistente.FechaModificacion = DateTime.Now;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
