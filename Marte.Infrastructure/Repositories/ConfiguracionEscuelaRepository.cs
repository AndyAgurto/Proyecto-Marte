using Microsoft.EntityFrameworkCore;
using Marte.Domain.Entities;
using Marte.Infrastructure.Data;
using Marte.Infrastructure.Interfaces;

namespace Marte.Infrastructure.Repositories
{
    public class ConfiguracionEscuelaRepository : IConfiguracionEscuelaRepository
    {
        private readonly MarteDbContext _context;

        public ConfiguracionEscuelaRepository(MarteDbContext context)
        {
            _context = context;
        }

        public async Task<ConfiguracionEscuela?> GetConfiguracionAsync()
        {
            return await _context.ConfiguracionesEscuela.FirstOrDefaultAsync();
        }

        public async Task<ConfiguracionEscuela> CreateOrUpdateConfiguracionAsync(ConfiguracionEscuela configuracion)
        {
            var existente = await GetConfiguracionAsync();
            
            if (existente != null)
            {
                existente.NombreFilial = configuracion.NombreFilial;
                existente.HoraCierre = configuracion.HoraCierre;
                existente.FechaConfiguracion = DateTime.Now;
                _context.ConfiguracionesEscuela.Update(existente);
            }
            else
            {
                configuracion.Id = Guid.NewGuid();
                configuracion.FechaConfiguracion = DateTime.Now;
                await _context.ConfiguracionesEscuela.AddAsync(configuracion);
            }

            await _context.SaveChangesAsync();
            return existente ?? configuracion;
        }
    }
}
