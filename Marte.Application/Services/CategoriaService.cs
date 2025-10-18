using Marte.Application.Interfaces;
using Marte.Domain.Entities;
using Marte.Infrastructure.Interfaces;

namespace Marte.Application.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IAuditLogRepository _auditLogRepository;

        public CategoriaService(
            ICategoriaRepository categoriaRepository,
            IAuditLogRepository auditLogRepository)
        {
            _categoriaRepository = categoriaRepository;
            _auditLogRepository = auditLogRepository;
        }

        public async Task<List<Categoria>> GetAllCategoriasAsync()
        {
            return await _categoriaRepository.GetAllAsync();
        }

        public async Task<Categoria?> GetCategoriaByIdAsync(Guid id)
        {
            return await _categoriaRepository.GetByIdAsync(id);
        }

        public async Task<Categoria> CreateCategoriaAsync(string nombre)
        {
            // Validar que el nombre no exista
            if (await _categoriaRepository.ExisteNombreAsync(nombre))
            {
                throw new InvalidOperationException($"Ya existe una categoría con el nombre '{nombre}'");
            }

            var categoria = new Categoria
            {
                Nombre = nombre
            };

            var resultado = await _categoriaRepository.CreateAsync(categoria);

            // Registrar en auditoría
            await _auditLogRepository.AddAsync(new AuditLog
            {
                Entidad = "Categoria",
                EntidadId = resultado.Id,
                Accion = "Crear Categoría",
                Usuario = "Sistema",
                Fecha = DateTime.Now,
                Detalle = $"Se creó la categoría '{nombre}'"
            });
            await _auditLogRepository.SaveChangesAsync();

            return resultado;
        }

        public async Task<Categoria> UpdateCategoriaAsync(Guid id, string nombre)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(id);
            if (categoria == null)
            {
                throw new InvalidOperationException("Categoría no encontrada");
            }

            // Validar que el nombre no exista (excepto la misma categoría)
            if (await _categoriaRepository.ExisteNombreAsync(nombre, id))
            {
                throw new InvalidOperationException($"Ya existe otra categoría con el nombre '{nombre}'");
            }

            categoria.Nombre = nombre;

            var resultado = await _categoriaRepository.UpdateAsync(categoria);

            // Registrar en auditoría
            await _auditLogRepository.AddAsync(new AuditLog
            {
                Entidad = "Categoria",
                EntidadId = resultado.Id,
                Accion = "Actualizar Categoría",
                Usuario = "Sistema",
                Fecha = DateTime.Now,
                Detalle = $"Se actualizó la categoría a '{nombre}'"
            });
            await _auditLogRepository.SaveChangesAsync();

            return resultado;
        }

        public async Task DeleteCategoriaAsync(Guid id)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(id);
            if (categoria == null)
            {
                throw new InvalidOperationException("Categoría no encontrada");
            }

            await _categoriaRepository.DeleteAsync(id);

            // Registrar en auditoría
            await _auditLogRepository.AddAsync(new AuditLog
            {
                Entidad = "Categoria",
                EntidadId = id,
                Accion = "Eliminar Categoría",
                Usuario = "Sistema",
                Fecha = DateTime.Now,
                Detalle = $"Se eliminó la categoría '{categoria.Nombre}'"
            });
            await _auditLogRepository.SaveChangesAsync();
        }

        public async Task<bool> ExisteNombreCategoriaAsync(string nombre, Guid? excludeId = null)
        {
            return await _categoriaRepository.ExisteNombreAsync(nombre, excludeId);
        }
    }
}
