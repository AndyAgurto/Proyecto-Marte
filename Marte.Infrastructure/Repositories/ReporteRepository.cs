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

        public async Task<IEnumerable<ReporteHistorialIndividualBusqueda>> GetHistorialIndividualBusquedaUnificadaAsync(string busqueda, DateTime fechaInicio, DateTime fechaFin)
        {
            if (string.IsNullOrWhiteSpace(busqueda))
                return new List<ReporteHistorialIndividualBusqueda>();

            // Normalizar el texto de búsqueda (remover tildes)
            var busquedaNormalizada = RemoverTildes(busqueda.ToLower().Trim());

            // Buscar en asistentes por DNI, nombre o apellido (con normalización)
            var asistentes = await _context.Asistentes
                .Include(a => a.Categoria)
                .Include(a => a.Asistencias.Where(ast => ast.Fecha.Date >= fechaInicio.Date && ast.Fecha.Date <= fechaFin.Date))
                .Where(a => a.Estado)
                .ToListAsync();

            var asistentesFiltrados = asistentes
                .Where(a => 
                    a.DNI.Contains(busqueda, StringComparison.OrdinalIgnoreCase) ||
                    RemoverTildes(a.Nombres.ToLower()).Contains(busquedaNormalizada) ||
                    RemoverTildes(a.Apellidos.ToLower()).Contains(busquedaNormalizada))
                .ToList();

            var resultado = new List<ReporteHistorialIndividualBusqueda>();

            foreach (var asistente in asistentesFiltrados)
            {
                var asistencias = asistente.Asistencias
                    .OrderByDescending(a => a.Fecha)
                    .ToList();

                var totalDias = asistencias.Count;

                foreach (var asistencia in asistencias)
                {
                    resultado.Add(new ReporteHistorialIndividualBusqueda
                    {
                        DNI = asistente.DNI,
                        NombreCompleto = $"{asistente.Apellidos}, {asistente.Nombres}",
                        Categoria = asistente.Categoria.Nombre,
                        Fecha = asistencia.Fecha,
                        HoraIngreso = asistencia.HoraIngreso.TimeOfDay,
                        HoraSalida = asistencia.HoraSalida?.TimeOfDay,
                        HastaCierre = asistencia.HastaCierre,
                        Observacion = asistencia.Observacion
                    });
                }
            }

            return resultado;
        }

        public async Task<IEnumerable<ReporteCategoriaJerarquico>> GetReporteCategoriaJerarquicoAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            var asistencias = await _context.Asistencias
                .Include(a => a.Asistente)
                    .ThenInclude(ast => ast.Categoria)
                .Where(a => a.Fecha.Date >= fechaInicio.Date && a.Fecha.Date <= fechaFin.Date)
                .ToListAsync();

            // Definir jerarquía de categorías
            var jerarquia = new Dictionary<string, (int Orden, string[] Variantes)>
            {
                { "Jefe de Filial", (1, new[] { "Jefe de Filial" }) },
                { "Jefe de Filial LM", (2, new[] { "Jefe de Filial LM" }) },
                { "Jefe de Filial LN", (3, new[] { "Jefe de Filial LN" }) },
                { "Secretarios", (4, new[] { "Secretarios", "Secretario" }) },
                { "G de S", (5, new[] { "G de S", "GS", "Grupo de Seguridad" }) },
                { "GG.FF", (6, new[] { "GG.FF", "GGFF" }) },
                { "GG.MM", (7, new[] { "GG.MM", "GGMM" }) },
                { "Miembros", (8, new[] { "Miembros" }) },
                { "Miembros LM", (9, new[] { "Miembros LM" }) },
                { "Miembros LN", (10, new[] { "Miembros LN" }) },
                { "Filosofía", (11, new[] { "Filosofía", "Filosofia" }) },
                { "Otros", (12, new[] { "Otros", "Otro" }) }
            };

            // Mapear categorías a su jerarquía
            var categoriasConJerarquia = asistencias
                .Select(a => new
                {
                    Asistencia = a,
                    CategoriaJerarquica = ObtenerCategoriaJerarquica(a.Asistente.Categoria.Nombre, jerarquia)
                })
                .ToList();

            // Definir categorías de Fuerzas Vivas
            var fuerzasVivas = new HashSet<string> { "G de S", "GG.FF", "GG.MM" };

            // Agrupar por categoría jerárquica
            var resultado = categoriasConJerarquia
                .GroupBy(x => x.CategoriaJerarquica)
                .Select(g => new ReporteCategoriaJerarquico
                {
                    CategoriaJerarquica = g.Key.Nombre,
                    Orden = g.Key.Orden,
                    GrupoAgrupacion = fuerzasVivas.Contains(g.Key.Nombre) ? "Fuerzas Vivas" : null,
                    TotalAsistencias = g.Count(),
                    TotalHastaCierre = g.Count(x => x.Asistencia.HastaCierre),
                    PorcentajeCierre = g.Count() > 0
                        ? Math.Round((double)g.Count(x => x.Asistencia.HastaCierre) / g.Count() * 100, 2)
                        : 0
                })
                .OrderBy(r => r.Orden)
                .ToList();

            return resultado;
        }

        // Método auxiliar para normalizar texto (remover tildes)
        private string RemoverTildes(string texto)
        {
            var textoNormalizado = texto.Normalize(System.Text.NormalizationForm.FormD);
            var chars = textoNormalizado.Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark).ToArray();
            return new string(chars).Normalize(System.Text.NormalizationForm.FormC);
        }

        // Método auxiliar para obtener categoría jerárquica
        private (string Nombre, int Orden) ObtenerCategoriaJerarquica(string categoriaNombre, Dictionary<string, (int Orden, string[] Variantes)> jerarquia)
        {
            foreach (var kvp in jerarquia)
            {
                foreach (var variante in kvp.Value.Variantes)
                {
                    if (categoriaNombre.Equals(variante, StringComparison.OrdinalIgnoreCase))
                    {
                        return (kvp.Key, kvp.Value.Orden);
                    }
                }
            }

            // Si no se encuentra, asignar a "Otros"
            return ("Otros", 12);
        }
    }
}
