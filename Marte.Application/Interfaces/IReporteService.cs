using Marte.Domain.DTOs;

namespace Marte.Application.Interfaces
{
    public interface IReporteService
    {
        // Reportes básicos
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

        // Exportación
        Task<string> ExportarTotalesDiariosExcelAsync(DateTime fechaInicio, DateTime fechaFin, string rutaArchivo);
        Task<string> ExportarTotalesDiariosPDFAsync(DateTime fechaInicio, DateTime fechaFin, string rutaArchivo);
        Task<string> ExportarPorCategoriaExcelAsync(DateTime fechaInicio, DateTime fechaFin, string rutaArchivo);
        Task<string> ExportarPorCategoriaPDFAsync(DateTime fechaInicio, DateTime fechaFin, string rutaArchivo);
        Task<string> ExportarHistorialIndividualExcelAsync(string dni, DateTime fechaInicio, DateTime fechaFin, string rutaArchivo);
        Task<string> ExportarHistorialIndividualPDFAsync(string dni, DateTime fechaInicio, DateTime fechaFin, string rutaArchivo);
        Task<string> ExportarHistorialBusquedaExcelAsync(string busqueda, DateTime fechaInicio, DateTime fechaFin, string rutaArchivo);
        Task<string> ExportarHistorialBusquedaPDFAsync(string busqueda, DateTime fechaInicio, DateTime fechaFin, string rutaArchivo);
        Task<string> ExportarPuntualidadExcelAsync(DateTime fechaInicio, DateTime fechaFin, TimeSpan horaPuntualidad, string rutaArchivo);
        Task<string> ExportarHastaCierreExcelAsync(DateTime fechaInicio, DateTime fechaFin, string rutaArchivo);
        Task<string> ExportarAsistenciaPorGrupoExcelAsync(DateTime fechaInicio, DateTime fechaFin, string rutaArchivo);
        Task<string> ExportarTopConstantesExcelAsync(DateTime fechaInicio, DateTime fechaFin, int top, string rutaArchivo);
        
        // Exportación para Asistentes Temporales
        Task<string> ExportarAsistentesTemporalesExcelAsync(DateTime fechaInicio, DateTime fechaFin, string rutaArchivo);
        Task<string> ExportarAsistentesTemporalesPDFAsync(DateTime fechaInicio, DateTime fechaFin, string rutaArchivo);
        
        // Exportación para Categoría Jerárquica
        Task<string> ExportarCategoriaJerarquicaExcelAsync(DateTime fechaInicio, DateTime fechaFin, string rutaArchivo);
        Task<string> ExportarCategoriaJerarquicaPDFAsync(DateTime fechaInicio, DateTime fechaFin, string rutaArchivo);
    }
}
