using Marte.Domain.Entities;

namespace Marte.Infrastructure.Interfaces
{
    public interface IAsistenteRepository
    {
        Task<IEnumerable<Asistente>> GetAllAsync();
        Task<IEnumerable<Asistente>> GetActivosAsync();
        Task<Asistente?> GetByIdAsync(Guid id);
        Task<Asistente?> GetByDNIAsync(string dni);
        Task<bool> ExisteDNIAsync(string dni, Guid? excludeId = null);
        Task<Asistente> CreateAsync(Asistente asistente);
        Task<Asistente> UpdateAsync(Asistente asistente);
        Task DeleteAsync(Guid id);
        Task<bool> InactivarAsync(Guid id);
        Task<bool> ActivarAsync(Guid id);
    }
}
