using Marte.Domain.Entities;
using Marte.Infrastructure.Data;
using Marte.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Marte.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly MarteDbContext _context;

        public UsuarioRepository(MarteDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> GetByUsernameAsync(string username)
        {
            return await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.NombreUsuario == username);
        }

        public async Task<Usuario?> GetByIdAsync(Guid id)
        {
            return await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            return await _context.Usuarios
                .Include(u => u.Rol)
                .ToListAsync();
        }

        public async Task AddAsync(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
        }

        public async Task UpdateAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await _context.Usuarios.FindAsync(id);
            if (user != null)
                _context.Usuarios.Remove(user);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(string username)
        {
            return await _context.Usuarios.AnyAsync(u => u.NombreUsuario == username);
        }
    }
}
