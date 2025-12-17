using Marte.Domain.Entities;

namespace Marte.Application.Interfaces
{
    public interface IAsistenteService
    {
        Task<IEnumerable<Asistente>> GetAllAsistentesAsync();
        Task<IEnumerable<Asistente>> GetAsistentesActivosAsync();
        Task<Asistente?> GetAsistenteByIdAsync(Guid id);
        Task<(bool Success, string Message, Asistente? Asistente)> CreateAsistenteAsync(
            string nombres,
            string apellidos,
            string dni,
            Guid categoriaId,
            string? numeroGrupo,
            string usuarioActual);
        Task<(bool Success, string Message)> UpdateAsistenteAsync(
            Guid id,
            string nombres,
            string apellidos,
            string dni,
            Guid categoriaId,
            string? numeroGrupo,
            string usuarioActual);
        Task<(bool Success, string Message)> DeleteAsistenteAsync(Guid id, string usuarioActual);
        Task<(bool Success, string Message)> InactivarAsistenteAsync(Guid id, string usuarioActual);
        Task<(bool Success, string Message)> ActivarAsistenteAsync(Guid id, string usuarioActual);
        Task<bool> ValidarDNIUnicoAsync(string dni, Guid? excludeId = null);
        Task<IEnumerable<Asistente>> BuscarAsistentesNormalizadoAsync(string busqueda);
    }
}
