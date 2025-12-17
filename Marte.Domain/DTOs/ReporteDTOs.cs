namespace Marte.Domain.DTOs
{
    // DTO para reporte de totales diarios
    public class ReporteTotalDiario
    {
        public DateTime Fecha { get; set; }
        public int TotalAsistencias { get; set; }
        public int TotalHastaCierre { get; set; }
        // Desglose detallado de asistentes del día
        public List<DetalleAsistenteDia> Asistentes { get; set; } = new List<DetalleAsistenteDia>();
    }

    // DTO para detalle de asistente en un día específico
    public class DetalleAsistenteDia
    {
        public string DNI { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string? NumeroGrupo { get; set; } // Solo para Miembros
        public TimeSpan HoraIngreso { get; set; }
        public TimeSpan? HoraSalida { get; set; }
        public bool HastaCierre { get; set; }
    }

    // DTO para reporte por categoría
    public class ReportePorCategoria
    {
        public string Categoria { get; set; } = string.Empty;
        public string? NumeroGrupo { get; set; } // Solo para Miembros
        public int Total { get; set; }
        public int HastaCierre { get; set; }
    }

    // DTO para reporte por grupo (Miembros)
    public class ReportePorGrupo
    {
        public string NumeroGrupo { get; set; } = string.Empty;
        public int TotalAsistencias { get; set; }
        public double Promedio { get; set; }
    }

    // DTO para historial individual
    public class ReporteHistorialIndividual
    {
        public DateTime Fecha { get; set; }
        public TimeSpan HoraIngreso { get; set; }
        public TimeSpan? HoraSalida { get; set; }
        public bool HastaCierre { get; set; }
        public string? Observacion { get; set; }
        public int TotalDias { get; set; }
    }

    // DTO para tasa de puntualidad
    public class ReportePuntualidad
    {
        public string DNI { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public int TotalAsistencias { get; set; }
        public int AsistenciasPuntuales { get; set; }
        public double PorcentajePuntualidad { get; set; }
        public TimeSpan PromedioHoraIngreso { get; set; }
    }

    // DTO para asistentes hasta cierre
    public class ReporteHastaCierre
    {
        public string DNI { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public int TotalDias { get; set; }
        public int DiasHastaCierre { get; set; }
        public double PorcentajeHastaCierre { get; set; }
    }

    // DTO para día de mayor asistencia
    public class ReporteDiaMayorAsistencia
    {
        public DateTime Fecha { get; set; }
        public string DiaSemana { get; set; } = string.Empty;
        public int TotalAsistencias { get; set; }
    }

    // DTO para top asistentes constantes
    public class ReporteTopConstantes
    {
        public int Posicion { get; set; }
        public string DNI { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string? NumeroGrupo { get; set; }
        public int TotalAsistencias { get; set; }
        public double PorcentajeAsistencia { get; set; }
        public int DiasConHoraSalida { get; set; }
    }

    // DTO para asistencia por grupo detallado
    public class ReporteAsistenciaGrupoDetalle
    {
        public string NumeroGrupo { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public int TotalMiembrosGrupo { get; set; }
        public int AsistentesDelGrupo { get; set; }
        public double PorcentajeAsistencia { get; set; }
        public List<string> NombresAsistentes { get; set; } = new();
    }

    // DTO para búsqueda unificada en historial individual
    public class ReporteHistorialIndividualBusqueda
    {
        public string DNI { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public TimeSpan HoraIngreso { get; set; }
        public TimeSpan? HoraSalida { get; set; }
        public bool HastaCierre { get; set; }
        public string? Observacion { get; set; }
    }

    // DTO para reporte jerárquico por categoría
    public class ReporteCategoriaJerarquico
    {
        public int Orden { get; set; }
        public string CategoriaJerarquica { get; set; } = string.Empty;
        public string? GrupoAgrupacion { get; set; } // "Fuerzas Vivas" para G de S, GG.FF, GG.MM
        public int TotalAsistencias { get; set; }
        public int TotalHastaCierre { get; set; }
        public double PorcentajeCierre { get; set; }
    }
}
