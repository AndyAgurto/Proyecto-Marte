using Marte.Domain.Entities;

namespace Marte.Application.Interfaces
{
    public interface IAsistenciaService
    {
        Task<(bool Success, string Message)> RegistrarIngresoAsync(string dni, string registradoPor);
        Task<(bool Success, string Message)> RegistrarIngresoDinamicoAsync(string busqueda, string registradoPor);
        Task<(bool Success, string Message)> RegistrarIngresoTemporalAsync(string nombres, string apellidos, string registradoPor);
        Task<(bool Success, string Message)> RegistrarSalidaAsync(Guid asistenciaId, string registradoPor, string? observacion = null);
        Task<(bool Success, string Message, int Actualizados)> AplicarCierreAutomaticoAsync(string aplicadoPor);
        Task<IEnumerable<Asistencia>> GetAsistenciasPresentesAsync();
        Task<IEnumerable<Asistencia>> GetAsistenciasByFechaAsync(DateTime fecha);
        Task<IEnumerable<Asistencia>> GetAllAsistenciasAsync();
        Task<IEnumerable<Asistencia>> BuscarAsistenciasPorNombreAsync(string busqueda, DateTime fecha);
    }
}
