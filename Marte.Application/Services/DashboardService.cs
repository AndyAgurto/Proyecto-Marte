using Marte.Application.Interfaces;
using Marte.Application.Models;
using Marte.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Marte.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly MarteDbContext _context;

        public DashboardService(MarteDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            var stats = new DashboardStatsDto();
            var hoy = DateTime.Today;
            var ayer = hoy.AddDays(-1);
            
            // 1. Asistentes hoy (con ingreso registrado)
            stats.AsistentesHoy = await _context.Asistencias
                .Where(a => a.Fecha.Date == hoy)
                .CountAsync();

            // 2. Asistentes que se quedaron hasta el cierre ayer
            stats.HastaCierreAyer = await _context.Asistencias
                .Where(a => a.Fecha.Date == ayer && a.HastaCierre)
                .CountAsync();

            // 3. Total de asistentes registrados (activos)
            stats.TotalRegistrados = await _context.Asistentes
                .Where(a => a.Estado)
                .CountAsync();

            // 4. Puntualidad (porcentaje de asistentes que llegaron antes de las 7:40 PM en el último mes)
            var hace30Dias = hoy.AddDays(-30);
            var asistenciasMes = await _context.Asistencias
                .Where(a => a.Fecha >= hace30Dias && a.Fecha <= hoy)
                .ToListAsync();

            if (asistenciasMes.Any())
            {
                var asistenciasPuntuales = asistenciasMes
                    .Count(a => a.HoraIngreso.TimeOfDay < new TimeSpan(19, 40, 0));
                stats.PorcentajePuntualidad = Math.Round((double)asistenciasPuntuales / asistenciasMes.Count * 100, 1);
            }
            else
            {
                stats.PorcentajePuntualidad = 0;
            }

            // 5. Asistentes por categoría (solo hoy)
            var asistenciasHoyConCategoria = await _context.Asistencias
                .Where(a => a.Fecha.Date == hoy)
                .Include(a => a.Asistente)
                    .ThenInclude(ast => ast.Categoria)
                .ToListAsync();

            // Contar miembros por grupo
            var miembrosHoy = asistenciasHoyConCategoria
                .Where(a => a.Asistente.Categoria.Nombre == "Miembros")
                .ToList();

            stats.MiembrosGrupo1 = miembrosHoy.Count(a => a.Asistente.NumeroGrupo == "1");
            stats.MiembrosGrupo2 = miembrosHoy.Count(a => a.Asistente.NumeroGrupo == "2");
            stats.MiembrosGrupo3 = miembrosHoy.Count(a => a.Asistente.NumeroGrupo == "3");

            // 6. Top 5 más constantes (últimos 30 días)
            var asistentesConAsistencias = await _context.Asistentes
                .Where(a => a.Estado)
                .Include(a => a.Asistencias.Where(ast => ast.Fecha >= hace30Dias && ast.Fecha <= hoy))
                .ToListAsync();

            // Calcular días disponibles (días entre primera y última asistencia, o 30 días)
            var diasDisponibles = 30;

            var topAsistentes = asistentesConAsistencias
                .Where(a => a.Asistencias.Any())
                .Select(a => new
                {
                    Asistente = a,
                    TotalAsistencias = a.Asistencias.Count,
                    DiasDisponibles = diasDisponibles,
                    Porcentaje = Math.Round((double)a.Asistencias.Count / diasDisponibles * 100, 1)
                })
                .OrderByDescending(x => x.TotalAsistencias)
                .ThenByDescending(x => x.Porcentaje)
                .Take(5)
                .ToList();

            stats.TopAsistentes = topAsistentes.Select((item, index) => new TopAsistenteDto
            {
                Posicion = index + 1,
                NombreCompleto = $"{item.Asistente.Nombres} {item.Asistente.Apellidos}",
                TotalAsistencias = item.TotalAsistencias,
                DiasDisponibles = item.DiasDisponibles,
                Porcentaje = item.Porcentaje
            }).ToList();

            return stats;
        }
    }
}
