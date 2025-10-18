using Microsoft.EntityFrameworkCore;
using Marte.Domain.Entities;
using Marte.Domain.DTOs;
using Marte.Infrastructure.Data;
using Marte.Infrastructure.Interfaces;

namespace Marte.Infrastructure.Repositories
{
    public class ReporteRepository : IReporteRepository
    {
        private readonly MarteDbContext _context;

        public ReporteRepository(MarteDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ReporteTotalDiario>> GetTotalesDiariosAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            var asistencias = await _context.Asistencias
                .Include(a => a.Asistente)
                    .ThenInclude(ast => ast.Categoria)
                .Where(a => a.Fecha.Date >= fechaInicio.Date && a.Fecha.Date <= fechaFin.Date)
                .OrderBy(a => a.Fecha.Date)
                .ThenBy(a => a.HoraIngreso)
                .ToListAsync();

            var result = asistencias
                .GroupBy(a => a.Fecha.Date)
                .Select(g => new ReporteTotalDiario
                {
                    Fecha = g.Key,
                    TotalAsistencias = g.Count(),
                    TotalHastaCierre = g.Count(a => a.HastaCierre),
                    Asistentes = g.Select(a => new DetalleAsistenteDia
                    {
                        DNI = a.Asistente.DNI,
                        NombreCompleto = $"{a.Asistente.Nombres} {a.Asistente.Apellidos}",
                        Categoria = a.Asistente.Categoria.Nombre,
                        NumeroGrupo = a.Asistente.Categoria.Nombre == "Miembros" ? a.Asistente.NumeroGrupo : null,
                        HoraIngreso = a.HoraIngreso.TimeOfDay,
                        HoraSalida = a.HoraSalida?.TimeOfDay,
                        HastaCierre = a.HastaCierre
                    }).ToList()
                })
                .OrderBy(r => r.Fecha)
                .ToList();

            return result;
        }

        public async Task<IEnumerable<ReportePorCategoria>> GetTotalesPorCategoriaAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            var asistencias = await _context.Asistencias
                .Include(a => a.Asistente)
                    .ThenInclude(ast => ast.Categoria)
                .Where(a => a.Fecha.Date >= fechaInicio.Date && a.Fecha.Date <= fechaFin.Date)
                .ToListAsync();

            var result = asistencias
                .GroupBy(a => new 
                { 
                    Categoria = a.Asistente.Categoria.Nombre,
                    NumeroGrupo = a.Asistente.Categoria.Nombre == "Miembros" ? a.Asistente.NumeroGrupo : null
                })
                .Select(g => new ReportePorCategoria
                {
                    Categoria = g.Key.Categoria,
                    NumeroGrupo = g.Key.NumeroGrupo,
                    Total = g.Count(),
                    HastaCierre = g.Count(a => a.HastaCierre)
                })
                .OrderBy(r => r.Categoria)
                .ThenBy(r => r.NumeroGrupo)
                .ToList();

            return result;
        }

        public async Task<IEnumerable<ReportePorGrupo>> GetTotalesPorGrupoAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            var asistenciasMiembros = await _context.Asistencias
                .Include(a => a.Asistente)
                    .ThenInclude(ast => ast.Categoria)
                .Where(a => a.Fecha.Date >= fechaInicio.Date 
                         && a.Fecha.Date <= fechaFin.Date
                         && a.Asistente.Categoria.Nombre == "Miembros"
                         && !string.IsNullOrEmpty(a.Asistente.NumeroGrupo))
                .ToListAsync();

            var result = asistenciasMiembros
                .GroupBy(a => a.Asistente.NumeroGrupo!)
                .Select(g => new ReportePorGrupo
                {
                    NumeroGrupo = g.Key,
                    TotalAsistencias = g.Count(),
                    Promedio = g.GroupBy(a => a.Fecha.Date).Count() > 0 
                        ? Math.Round((double)g.Count() / g.GroupBy(a => a.Fecha.Date).Count(), 2) 
                        : 0
                })
                .OrderBy(r => r.NumeroGrupo)
                .ToList();

            return result;
        }

        public async Task<IEnumerable<ReporteHistorialIndividual>> GetHistorialIndividualPorDNIAsync(string dni, DateTime fechaInicio, DateTime fechaFin)
        {
            var asistente = await _context.Asistentes
                .FirstOrDefaultAsync(a => a.DNI == dni);

            if (asistente == null)
                return new List<ReporteHistorialIndividual>();

            var asistencias = await _context.Asistencias
                .Where(a => a.AsistenteId == asistente.Id
                         && a.Fecha.Date >= fechaInicio.Date
                         && a.Fecha.Date <= fechaFin.Date)
                .OrderByDescending(a => a.Fecha)
                .ToListAsync();

            var totalDias = asistencias.Count;

            var result = asistencias.Select(a => new ReporteHistorialIndividual
            {
                Fecha = a.Fecha,
                HoraIngreso = a.HoraIngreso.TimeOfDay,
                HoraSalida = a.HoraSalida?.TimeOfDay,
                HastaCierre = a.HastaCierre,
                Observacion = a.Observacion,
                TotalDias = totalDias
            }).ToList();

            return result;
        }

        public async Task<IEnumerable<ReporteHistorialIndividual>> GetHistorialIndividualPorNombreAsync(string nombres, string apellidos, DateTime fechaInicio, DateTime fechaFin)
        {
            var asistente = await _context.Asistentes
                .FirstOrDefaultAsync(a => a.Nombres.ToLower().Contains(nombres.ToLower())
                                       && a.Apellidos.ToLower().Contains(apellidos.ToLower()));

            if (asistente == null)
                return new List<ReporteHistorialIndividual>();

            return await GetHistorialIndividualPorDNIAsync(asistente.DNI, fechaInicio, fechaFin);
        }

        public async Task<IEnumerable<ReportePuntualidad>> GetReportePuntualidadAsync(DateTime fechaInicio, DateTime fechaFin, TimeSpan horaPuntualidad)
        {
            var asistencias = await _context.Asistencias
                .Include(a => a.Asistente)
                    .ThenInclude(ast => ast.Categoria)
                .Where(a => a.Fecha.Date >= fechaInicio.Date && a.Fecha.Date <= fechaFin.Date)
                .ToListAsync();

            var result = asistencias
                .GroupBy(a => new 
                { 
                    a.Asistente.DNI, 
                    NombreCompleto = $"{a.Asistente.Apellidos}, {a.Asistente.Nombres}",
                    Categoria = a.Asistente.Categoria.Nombre
                })
                .Select(g => new ReportePuntualidad
                {
                    DNI = g.Key.DNI,
                    NombreCompleto = g.Key.NombreCompleto,
                    Categoria = g.Key.Categoria,
                    TotalAsistencias = g.Count(),
                    AsistenciasPuntuales = g.Count(a => a.HoraIngreso.TimeOfDay <= horaPuntualidad),
                    PorcentajePuntualidad = g.Count() > 0 
                        ? Math.Round((double)g.Count(a => a.HoraIngreso.TimeOfDay <= horaPuntualidad) / g.Count() * 100, 2)
                        : 0,
                    PromedioHoraIngreso = TimeSpan.FromTicks((long)g.Average(a => a.HoraIngreso.TimeOfDay.Ticks))
                })
                .OrderByDescending(r => r.PorcentajePuntualidad)
                .ThenByDescending(r => r.TotalAsistencias)
                .ToList();

            return result;
        }

        public async Task<IEnumerable<ReporteHastaCierre>> GetReporteHastaCierreAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            var asistencias = await _context.Asistencias
                .Include(a => a.Asistente)
                    .ThenInclude(ast => ast.Categoria)
                .Where(a => a.Fecha.Date >= fechaInicio.Date && a.Fecha.Date <= fechaFin.Date)
                .ToListAsync();

            var result = asistencias
                .GroupBy(a => new 
                { 
                    a.Asistente.DNI, 
                    NombreCompleto = $"{a.Asistente.Apellidos}, {a.Asistente.Nombres}",
                    Categoria = a.Asistente.Categoria.Nombre
                })
                .Select(g => new ReporteHastaCierre
                {
                    DNI = g.Key.DNI,
                    NombreCompleto = g.Key.NombreCompleto,
                    Categoria = g.Key.Categoria,
                    TotalDias = g.Count(),
                    DiasHastaCierre = g.Count(a => a.HastaCierre),
                    PorcentajeHastaCierre = g.Count() > 0 
                        ? Math.Round((double)g.Count(a => a.HastaCierre) / g.Count() * 100, 2)
                        : 0
                })
                .OrderByDescending(r => r.DiasHastaCierre)
                .ThenByDescending(r => r.PorcentajeHastaCierre)
                .ToList();

            return result;
        }

        public async Task<IEnumerable<ReporteAsistenciaGrupoDetalle>> GetReporteAsistenciaPorGrupoAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            var asistenciasMiembros = await _context.Asistencias
                .Include(a => a.Asistente)
                    .ThenInclude(ast => ast.Categoria)
                .Where(a => a.Fecha.Date >= fechaInicio.Date 
                         && a.Fecha.Date <= fechaFin.Date
                         && a.Asistente.Categoria.Nombre == "Miembros"
                         && !string.IsNullOrEmpty(a.Asistente.NumeroGrupo))
                .ToListAsync();

            // Obtener total de miembros por grupo
            var miembrosPorGrupo = await _context.Asistentes
                .Include(a => a.Categoria)
                .Where(a => a.Categoria.Nombre == "Miembros" 
                         && !string.IsNullOrEmpty(a.NumeroGrupo)
                         && a.Estado)
                .GroupBy(a => a.NumeroGrupo!)
                .ToDictionaryAsync(g => g.Key, g => g.Count());

            var result = asistenciasMiembros
                .GroupBy(a => new { a.Asistente.NumeroGrupo, a.Fecha.Date })
                .Select(g => new ReporteAsistenciaGrupoDetalle
                {
                    NumeroGrupo = g.Key.NumeroGrupo!,
                    Fecha = g.Key.Date,
                    TotalMiembrosGrupo = miembrosPorGrupo.GetValueOrDefault(g.Key.NumeroGrupo!, 0),
                    AsistentesDelGrupo = g.Select(a => a.AsistenteId).Distinct().Count(),
                    PorcentajeAsistencia = miembrosPorGrupo.GetValueOrDefault(g.Key.NumeroGrupo!, 0) > 0
                        ? Math.Round((double)g.Select(a => a.AsistenteId).Distinct().Count() / miembrosPorGrupo[g.Key.NumeroGrupo!] * 100, 2)
                        : 0,
                    NombresAsistentes = g.Select(a => $"{a.Asistente.Apellidos}, {a.Asistente.Nombres}").Distinct().ToList()
                })
                .OrderBy(r => r.NumeroGrupo)
                .ThenByDescending(r => r.Fecha)
                .ToList();

            return result;
        }

        public async Task<ReporteDiaMayorAsistencia?> GetDiaMayorAsistenciaAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            var asistenciasPorDia = await _context.Asistencias
                .Where(a => a.Fecha.Date >= fechaInicio.Date && a.Fecha.Date <= fechaFin.Date)
                .GroupBy(a => a.Fecha.Date)
                .Select(g => new
                {
                    Fecha = g.Key,
                    TotalAsistencias = g.Count()
                })
                .OrderByDescending(r => r.TotalAsistencias)
                .FirstOrDefaultAsync();

            if (asistenciasPorDia == null)
                return null;

            var culturaEspanol = new System.Globalization.CultureInfo("es-ES");
            var diaSemana = culturaEspanol.DateTimeFormat.GetDayName(asistenciasPorDia.Fecha.DayOfWeek);
            diaSemana = char.ToUpper(diaSemana[0]) + diaSemana.Substring(1);

            return new ReporteDiaMayorAsistencia
            {
                Fecha = asistenciasPorDia.Fecha,
                DiaSemana = diaSemana,
                TotalAsistencias = asistenciasPorDia.TotalAsistencias
            };
        }

        public async Task<IEnumerable<ReporteTopConstantes>> GetTopAsistentesConstantesAsync(DateTime fechaInicio, DateTime fechaFin, int top = 10)
        {
            var totalDias = (fechaFin.Date - fechaInicio.Date).Days + 1;

            var asistencias = await _context.Asistencias
                .Include(a => a.Asistente)
                    .ThenInclude(ast => ast.Categoria)
                .Where(a => a.Fecha.Date >= fechaInicio.Date && a.Fecha.Date <= fechaFin.Date)
                .ToListAsync();

            var result = asistencias
                .GroupBy(a => new 
                { 
                    a.Asistente.DNI, 
                    NombreCompleto = $"{a.Asistente.Apellidos}, {a.Asistente.Nombres}",
                    Categoria = a.Asistente.Categoria.Nombre,
                    NumeroGrupo = a.Asistente.NumeroGrupo
                })
                .Select(g => new ReporteTopConstantes
                {
                    DNI = g.Key.DNI,
                    NombreCompleto = g.Key.NombreCompleto,
                    Categoria = g.Key.Categoria,
                    NumeroGrupo = g.Key.NumeroGrupo,
                    TotalAsistencias = g.Count(),
                    PorcentajeAsistencia = totalDias > 0 
                        ? Math.Round((double)g.Select(a => a.Fecha.Date).Distinct().Count() / totalDias * 100, 2)
                        : 0,
                    DiasConHoraSalida = g.Count(a => a.HoraSalida != null)
                })
                .OrderByDescending(r => r.TotalAsistencias)
                .ThenByDescending(r => r.PorcentajeAsistencia)
                .Take(top)
                .ToList();

            // Asignar posiciones
            for (int i = 0; i < result.Count; i++)
            {
                result[i].Posicion = i + 1;
            }

            return result;
        }
    }
}
