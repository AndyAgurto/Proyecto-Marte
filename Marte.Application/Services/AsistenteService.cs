using Marte.Application.Interfaces;
using Marte.Domain.Entities;
using Marte.Infrastructure.Interfaces;

namespace Marte.Application.Services
{
    public class AsistenteService : IAsistenteService
    {
        private readonly IAsistenteRepository _asistenteRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IAuditLogRepository _auditLogRepository;

        public AsistenteService(
            IAsistenteRepository asistenteRepository,
            ICategoriaRepository categoriaRepository,
            IAuditLogRepository auditLogRepository)
        {
            _asistenteRepository = asistenteRepository;
            _categoriaRepository = categoriaRepository;
            _auditLogRepository = auditLogRepository;
        }

        public async Task<IEnumerable<Asistente>> GetAllAsistentesAsync()
        {
            return await _asistenteRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Asistente>> GetAsistentesActivosAsync()
        {
            return await _asistenteRepository.GetActivosAsync();
        }

        public async Task<Asistente?> GetAsistenteByIdAsync(Guid id)
        {
            return await _asistenteRepository.GetByIdAsync(id);
        }

        public async Task<(bool Success, string Message, Asistente? Asistente)> CreateAsistenteAsync(
            string nombres,
            string apellidos,
            string dni,
            Guid categoriaId,
            string? numeroGrupo,
            string usuarioActual)
        {
            try
            {
                // Validar campos obligatorios
                if (string.IsNullOrWhiteSpace(nombres))
                    return (false, "El nombre es obligatorio.", null);

                if (string.IsNullOrWhiteSpace(apellidos))
                    return (false, "Los apellidos son obligatorios.", null);

                if (string.IsNullOrWhiteSpace(dni))
                    return (false, "El DNI/Código es obligatorio.", null);

                // Validar DNI único
                if (await _asistenteRepository.ExisteDNIAsync(dni))
                    return (false, "Ya existe un asistente con este DNI/Código.", null);

                // Validar categoría
                var categoria = await _categoriaRepository.GetByIdAsync(categoriaId);
                if (categoria == null)
                    return (false, "La categoría seleccionada no existe.", null);

                // Validar número de grupo (solo para categoría "Miembros")
                if (categoria.Nombre.Equals("Miembros", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrWhiteSpace(numeroGrupo))
                        return (false, "El número de grupo es obligatorio para la categoría 'Miembros'.", null);

                    // Validar formato de número de grupo (01, 02, 03, etc.)
                    if (!System.Text.RegularExpressions.Regex.IsMatch(numeroGrupo, @"^\d{2}$"))
                        return (false, "El número de grupo debe tener formato de 2 dígitos (ejemplo: 01, 02, 03).", null);
                }
                else
                {
                    // Para otras categorías, el número de grupo debe ser null o "No aplica"
                    numeroGrupo = null;
                }

                // Crear asistente
                var asistente = new Asistente
                {
                    Nombres = nombres.Trim(),
                    Apellidos = apellidos.Trim(),
                    DNI = dni.Trim().ToUpper(),
                    CategoriaId = categoriaId,
                    NumeroGrupo = numeroGrupo,
                    Estado = true
                };

                var asistenteCreado = await _asistenteRepository.CreateAsync(asistente);

                // Auditar
                await _auditLogRepository.AddAsync(new AuditLog
                {
                    Entidad = "Asistente",
                    EntidadId = asistenteCreado.Id,
                    Accion = "Crear Asistente",
                    Usuario = usuarioActual,
                    Fecha = DateTime.Now,
                    Detalle = $"Asistente creado: {asistenteCreado.Nombres} {asistenteCreado.Apellidos} (DNI: {asistenteCreado.DNI})"
                });
                await _auditLogRepository.SaveChangesAsync();

                return (true, "Asistente registrado exitosamente.", asistenteCreado);
            }
            catch (Exception ex)
            {
                return (false, $"Error al crear asistente: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message)> UpdateAsistenteAsync(
            Guid id,
            string nombres,
            string apellidos,
            string dni,
            Guid categoriaId,
            string? numeroGrupo,
            string usuarioActual)
        {
            try
            {
                var asistente = await _asistenteRepository.GetByIdAsync(id);
                if (asistente == null)
                    return (false, "Asistente no encontrado.");

                // Validar campos obligatorios
                if (string.IsNullOrWhiteSpace(nombres))
                    return (false, "El nombre es obligatorio.");

                if (string.IsNullOrWhiteSpace(apellidos))
                    return (false, "Los apellidos son obligatorios.");

                if (string.IsNullOrWhiteSpace(dni))
                    return (false, "El DNI/Código es obligatorio.");

                // Validar DNI único (excluyendo el actual)
                if (await _asistenteRepository.ExisteDNIAsync(dni, id))
                    return (false, "Ya existe otro asistente con este DNI/Código.");

                // Validar categoría
                var categoria = await _categoriaRepository.GetByIdAsync(categoriaId);
                if (categoria == null)
                    return (false, "La categoría seleccionada no existe.");

                // Validar número de grupo
                if (categoria.Nombre.Equals("Miembros", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrWhiteSpace(numeroGrupo))
                        return (false, "El número de grupo es obligatorio para la categoría 'Miembros'.");

                    if (!System.Text.RegularExpressions.Regex.IsMatch(numeroGrupo, @"^\d{2}$"))
                        return (false, "El número de grupo debe tener formato de 2 dígitos (ejemplo: 01, 02, 03).");
                }
                else
                {
                    numeroGrupo = null;
                }

                // Actualizar asistente
                var dniAnterior = asistente.DNI;
                var categoriaAnterior = asistente.Categoria.Nombre;
                var grupoAnterior = asistente.NumeroGrupo;

                asistente.Nombres = nombres.Trim();
                asistente.Apellidos = apellidos.Trim();
                asistente.DNI = dni.Trim().ToUpper();
                asistente.CategoriaId = categoriaId;
                asistente.NumeroGrupo = numeroGrupo;

                await _asistenteRepository.UpdateAsync(asistente);

                // Auditar cambios
                var cambios = new List<string>();
                if (dniAnterior != asistente.DNI)
                    cambios.Add($"DNI: {dniAnterior} → {asistente.DNI}");
                if (categoriaAnterior != categoria.Nombre)
                    cambios.Add($"Categoría: {categoriaAnterior} → {categoria.Nombre}");
                if (grupoAnterior != numeroGrupo)
                    cambios.Add($"Grupo: {grupoAnterior ?? "N/A"} → {numeroGrupo ?? "N/A"}");

                await _auditLogRepository.AddAsync(new AuditLog
                {
                    Entidad = "Asistente",
                    EntidadId = asistente.Id,
                    Accion = "Actualizar Asistente",
                    Usuario = usuarioActual,
                    Fecha = DateTime.Now,
                    Detalle = $"Asistente actualizado: {asistente.Nombres} {asistente.Apellidos}. Cambios: {string.Join(", ", cambios)}"
                });
                await _auditLogRepository.SaveChangesAsync();

                return (true, "Asistente actualizado exitosamente.");
            }
            catch (Exception ex)
            {
                return (false, $"Error al actualizar asistente: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> DeleteAsistenteAsync(Guid id, string usuarioActual)
        {
            try
            {
                var asistente = await _asistenteRepository.GetByIdAsync(id);
                if (asistente == null)
                    return (false, "Asistente no encontrado.");

                await _asistenteRepository.DeleteAsync(id);

                await _auditLogRepository.AddAsync(new AuditLog
                {
                    Entidad = "Asistente",
                    EntidadId = id,
                    Accion = "Eliminar Asistente",
                    Usuario = usuarioActual,
                    Fecha = DateTime.Now,
                    Detalle = $"Asistente eliminado: {asistente.Nombres} {asistente.Apellidos} (DNI: {asistente.DNI})"
                });
                await _auditLogRepository.SaveChangesAsync();

                return (true, "Asistente eliminado exitosamente.");
            }
            catch (Exception ex)
            {
                return (false, $"Error al eliminar asistente: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> InactivarAsistenteAsync(Guid id, string usuarioActual)
        {
            try
            {
                var asistente = await _asistenteRepository.GetByIdAsync(id);
                if (asistente == null)
                    return (false, "Asistente no encontrado.");

                if (!asistente.Estado)
                    return (false, "El asistente ya está inactivo.");

                await _asistenteRepository.InactivarAsync(id);

                await _auditLogRepository.AddAsync(new AuditLog
                {
                    Entidad = "Asistente",
                    EntidadId = id,
                    Accion = "Inactivar Asistente",
                    Usuario = usuarioActual,
                    Fecha = DateTime.Now,
                    Detalle = $"Asistente inactivado: {asistente.Nombres} {asistente.Apellidos} (DNI: {asistente.DNI})"
                });
                await _auditLogRepository.SaveChangesAsync();

                return (true, "Asistente inactivado exitosamente.");
            }
            catch (Exception ex)
            {
                return (false, $"Error al inactivar asistente: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> ActivarAsistenteAsync(Guid id, string usuarioActual)
        {
            try
            {
                var asistente = await _asistenteRepository.GetByIdAsync(id);
                if (asistente == null)
                    return (false, "Asistente no encontrado.");

                if (asistente.Estado)
                    return (false, "El asistente ya está activo.");

                await _asistenteRepository.ActivarAsync(id);

                await _auditLogRepository.AddAsync(new AuditLog
                {
                    Entidad = "Asistente",
                    EntidadId = id,
                    Accion = "Activar Asistente",
                    Usuario = usuarioActual,
                    Fecha = DateTime.Now,
                    Detalle = $"Asistente activado: {asistente.Nombres} {asistente.Apellidos} (DNI: {asistente.DNI})"
                });
                await _auditLogRepository.SaveChangesAsync();

                return (true, "Asistente activado exitosamente.");
            }
            catch (Exception ex)
            {
                return (false, $"Error al activar asistente: {ex.Message}");
            }
        }

        public async Task<bool> ValidarDNIUnicoAsync(string dni, Guid? excludeId = null)
        {
            return !await _asistenteRepository.ExisteDNIAsync(dni, excludeId);
        }

        public async Task<IEnumerable<Asistente>> BuscarAsistentesNormalizadoAsync(string busqueda)
        {
            if (string.IsNullOrWhiteSpace(busqueda))
                return new List<Asistente>();

            var asistentes = await _asistenteRepository.GetActivosAsync();
            var busquedaNormalizada = RemoverTildes(busqueda.ToLower().Trim());

            return asistentes.Where(a =>
                a.DNI.Contains(busqueda, StringComparison.OrdinalIgnoreCase) ||
                RemoverTildes(a.Nombres.ToLower()).Contains(busquedaNormalizada) ||
                RemoverTildes(a.Apellidos.ToLower()).Contains(busquedaNormalizada) ||
                RemoverTildes($"{a.Nombres} {a.Apellidos}".ToLower()).Contains(busquedaNormalizada)
            ).ToList();
        }

        private string RemoverTildes(string texto)
        {
            var textoNormalizado = texto.Normalize(System.Text.NormalizationForm.FormD);
            var chars = textoNormalizado.Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark).ToArray();
            return new string(chars).Normalize(System.Text.NormalizationForm.FormC);
        }
    }
}
