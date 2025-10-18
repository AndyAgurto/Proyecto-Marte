using Marte.Domain.Entities;
using Marte.Infrastructure.Data;
using Marte.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Marte.Infrastructure.Repositories
{
    public class RolRepository : IRolRepository
    {
        private readonly MarteDbContext _context;

        public RolRepository(MarteDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Rol>> GetAllAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<Rol?> GetByIdAsync(Guid id)
        {
            return await _context.Roles.FindAsync(id);
        }

        public async Task<Rol?> GetByNombreAsync(string nombre)
        {
            return await _context.Roles.FirstOrDefaultAsync(r => r.Nombre == nombre);
        }
    }
}
