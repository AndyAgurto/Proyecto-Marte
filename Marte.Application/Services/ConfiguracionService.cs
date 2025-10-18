using Marte.Application.Interfaces;
using Marte.Domain.Entities;
using Marte.Infrastructure.Interfaces;

namespace Marte.Application.Services
{
    public class ConfiguracionService : IConfiguracionService
    {
        private readonly IConfiguracionEscuelaRepository _configuracionRepository;
        private readonly IAuditLogRepository _auditLogRepository;

        public ConfiguracionService(
            IConfiguracionEscuelaRepository configuracionRepository,
            IAuditLogRepository auditLogRepository)
        {
            _configuracionRepository = configuracionRepository;
            _auditLogRepository = auditLogRepository;
        }

        public async Task<ConfiguracionEscuela?> GetConfiguracionAsync()
        {
            return await _configuracionRepository.GetConfiguracionAsync();
        }

        public async Task<ConfiguracionEscuela> UpdateHoraCierreAsync(TimeSpan horaCierre, string nombreFilial)
        {
            var configuracion = await _configuracionRepository.GetConfiguracionAsync();
            
            var nuevaConfiguracion = new ConfiguracionEscuela
            {
                NombreFilial = nombreFilial,
                HoraCierre = horaCierre
            };

            var resultado = await _configuracionRepository.CreateOrUpdateConfiguracionAsync(nuevaConfiguracion);

            // Registrar en auditoría
            await _auditLogRepository.AddAsync(new AuditLog
            {
                Entidad = "ConfiguracionEscuela",
                EntidadId = resultado.Id,
                Accion = "Actualización de Configuración",
                Usuario = "Sistema",
                Fecha = DateTime.Now,
                Detalle = $"Se actualizó la hora de cierre a {horaCierre:hh\\:mm} y filial: {nombreFilial}"
            });
            await _auditLogRepository.SaveChangesAsync();

            return resultado;
        }
    }
}
