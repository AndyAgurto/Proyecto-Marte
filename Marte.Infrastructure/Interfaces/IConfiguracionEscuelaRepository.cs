using Marte.Domain.Entities;

namespace Marte.Infrastructure.Interfaces
{
    public interface IConfiguracionEscuelaRepository
    {
        Task<ConfiguracionEscuela?> GetConfiguracionAsync();
        Task<ConfiguracionEscuela> CreateOrUpdateConfiguracionAsync(ConfiguracionEscuela configuracion);
    }
}
