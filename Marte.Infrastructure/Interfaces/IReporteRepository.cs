using Marte.Domain.DTOs;

namespace Marte.Infrastructure.Interfaces
{
    public interface IReporteRepository
    {
        // Reportes básicos por fecha
        Task<IEnumerable<ReporteTotalDiario>> GetTotalesDiariosAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<IEnumerable<ReportePorCategoria>> GetTotalesPorCategoriaAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<IEnumerable<ReportePorGrupo>> GetTotalesPorGrupoAsync(DateTime fechaInicio, DateTime fechaFin);

        // Historial individual
        Task<IEnumerable<ReporteHistorialIndividual>> GetHistorialIndividualPorDNIAsync(string dni, DateTime fechaInicio, DateTime fechaFin);
        Task<IEnumerable<ReporteHistorialIndividual>> GetHistorialIndividualPorNombreAsync(string nombres, string apellidos, DateTime fechaInicio, DateTime fechaFin);
        Task<IEnumerable<ReporteHistorialIndividualBusqueda>> GetHistorialIndividualBusquedaUnificadaAsync(string busqueda, DateTime fechaInicio, DateTime fechaFin);
        
        // Reporte jerárquico
        Task<IEnumerable<ReporteCategoriaJerarquico>> GetReporteCategoriaJerarquicoAsync(DateTime fechaInicio, DateTime fechaFin);

        // Reportes analíticos
        Task<IEnumerable<ReportePuntualidad>> GetReportePuntualidadAsync(DateTime fechaInicio, DateTime fechaFin, TimeSpan horaPuntualidad);
        Task<IEnumerable<ReporteHastaCierre>> GetReporteHastaCierreAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<IEnumerable<ReporteAsistenciaGrupoDetalle>> GetReporteAsistenciaPorGrupoAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<ReporteDiaMayorAsistencia?> GetDiaMayorAsistenciaAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<IEnumerable<ReporteTopConstantes>> GetTopAsistentesConstantesAsync(DateTime fechaInicio, DateTime fechaFin, int top = 10);
    }
}
