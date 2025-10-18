using Marte.Domain.Entities;

namespace Marte.Infrastructure.Interfaces
{
    public interface IAsistenciaRepository
    {
        Task<IEnumerable<Asistencia>> GetAllAsync();
        Task<Asistencia?> GetByIdAsync(Guid id);
        Task<IEnumerable<Asistencia>> GetByFechaAsync(DateTime fecha);
        Task<IEnumerable<Asistencia>> GetAsistenciasPresentesAsync();
        Task<Asistencia?> GetAsistenciaAbiertaByAsistenteIdAsync(Guid asistenteId);
        Task<bool> ExisteAsistenciaAbiertaAsync(Guid asistenteId);
        Task<bool> ExisteAsistenciaHoyAsync(Guid asistenteId);
        Task AddAsync(Asistencia asistencia);
        Task UpdateAsync(Asistencia asistencia);
        Task<int> SaveChangesAsync();
    }
}
