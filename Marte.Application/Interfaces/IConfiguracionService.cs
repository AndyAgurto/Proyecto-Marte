using Marte.Domain.Entities;

namespace Marte.Application.Interfaces
{
    public interface IConfiguracionService
    {
        Task<ConfiguracionEscuela?> GetConfiguracionAsync();
        Task<ConfiguracionEscuela> UpdateHoraCierreAsync(TimeSpan horaCierre, string nombreFilial);
    }
}
