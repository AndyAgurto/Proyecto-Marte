using Microsoft.EntityFrameworkCore;
using Marte.Domain.Entities;
using Marte.Infrastructure.Data;
using Marte.Infrastructure.Interfaces;

namespace Marte.Infrastructure.Repositories
{
    public class AsistenciaRepository : IAsistenciaRepository
    {
        private readonly MarteDbContext _context;

        public AsistenciaRepository(MarteDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Asistencia>> GetAllAsync()
        {
            return await _context.Asistencias
                .Include(a => a.Asistente)
                    .ThenInclude(ast => ast.Categoria)
                .OrderByDescending(a => a.Fecha)
                .ThenByDescending(a => a.HoraIngreso)
                .ToListAsync();
        }

        public async Task<Asistencia?> GetByIdAsync(Guid id)
        {
            return await _context.Asistencias
                .Include(a => a.Asistente)
                    .ThenInclude(ast => ast.Categoria)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Asistencia>> GetByFechaAsync(DateTime fecha)
        {
            var fechaSolo = fecha.Date;
            return await _context.Asistencias
                .Include(a => a.Asistente)
                    .ThenInclude(ast => ast.Categoria)
                .Where(a => a.Fecha.Date == fechaSolo)
                .OrderByDescending(a => a.HoraIngreso)
                .ToListAsync();
        }

        public async Task<IEnumerable<Asistencia>> GetByFechaRangoAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            var inicio = fechaInicio.Date;
            var fin = fechaFin.Date;
            return await _context.Asistencias
                .Include(a => a.Asistente)
                    .ThenInclude(ast => ast.Categoria)
                .Where(a => a.Fecha.Date >= inicio && a.Fecha.Date <= fin)
                .OrderBy(a => a.Fecha)
                .ThenBy(a => a.Asistente.Apellidos)
                .ThenBy(a => a.Asistente.Nombres)
                .ToListAsync();
        }

        public async Task<IEnumerable<Asistencia>> GetAsistenciasPresentesAsync()
        {
            var hoy = DateTime.Today;
            return await _context.Asistencias
                .Include(a => a.Asistente)
                    .ThenInclude(ast => ast.Categoria)
                .Where(a => a.Fecha.Date == hoy && a.HoraSalida == null)
                .OrderBy(a => a.Asistente.Apellidos)
                .ThenBy(a => a.Asistente.Nombres)
                .ToListAsync();
        }

        public async Task<Asistencia?> GetAsistenciaAbiertaByAsistenteIdAsync(Guid asistenteId)
        {
            var hoy = DateTime.Today;
            return await _context.Asistencias
                .Include(a => a.Asistente)
                    .ThenInclude(ast => ast.Categoria)
                .FirstOrDefaultAsync(a => a.AsistenteId == asistenteId 
                                       && a.Fecha.Date == hoy 
                                       && a.HoraSalida == null);
        }

        public async Task<bool> ExisteAsistenciaAbiertaAsync(Guid asistenteId)
        {
            var hoy = DateTime.Today;
            return await _context.Asistencias
                .AnyAsync(a => a.AsistenteId == asistenteId 
                            && a.Fecha.Date == hoy 
                            && a.HoraSalida == null);
        }

        public async Task<bool> ExisteAsistenciaHoyAsync(Guid asistenteId)
        {
            var hoy = DateTime.Today;
            return await _context.Asistencias
                .AnyAsync(a => a.AsistenteId == asistenteId 
                            && a.Fecha.Date == hoy);
        }

        public async Task AddAsync(Asistencia asistencia)
        {
            await _context.Asistencias.AddAsync(asistencia);
        }

        public async Task UpdateAsync(Asistencia asistencia)
        {
            _context.Asistencias.Update(asistencia);
            await Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
