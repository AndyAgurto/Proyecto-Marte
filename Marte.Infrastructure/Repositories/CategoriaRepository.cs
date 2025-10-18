using Microsoft.EntityFrameworkCore;
using Marte.Domain.Entities;
using Marte.Infrastructure.Data;
using Marte.Infrastructure.Interfaces;

namespace Marte.Infrastructure.Repositories
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly MarteDbContext _context;

        public CategoriaRepository(MarteDbContext context)
        {
            _context = context;
        }

        public async Task<List<Categoria>> GetAllAsync()
        {
            return await _context.Categorias
                .OrderBy(c => c.Nombre)
                .ToListAsync();
        }

        public async Task<Categoria?> GetByIdAsync(Guid id)
        {
            return await _context.Categorias
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Categoria> CreateAsync(Categoria categoria)
        {
            categoria.Id = Guid.NewGuid();
            await _context.Categorias.AddAsync(categoria);
            await _context.SaveChangesAsync();
            return categoria;
        }

        public async Task<Categoria> UpdateAsync(Categoria categoria)
        {
            _context.Categorias.Update(categoria);
            await _context.SaveChangesAsync();
            return categoria;
        }

        public async Task DeleteAsync(Guid id)
        {
            var categoria = await GetByIdAsync(id);
            if (categoria != null)
            {
                _context.Categorias.Remove(categoria);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExisteNombreAsync(string nombre, Guid? excludeId = null)
        {
            var query = _context.Categorias.Where(c => c.Nombre.ToLower() == nombre.ToLower());
            
            if (excludeId.HasValue)
            {
                query = query.Where(c => c.Id != excludeId.Value);
            }
            
            return await query.AnyAsync();
        }
    }
}
