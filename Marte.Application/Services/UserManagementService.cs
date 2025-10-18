using Marte.Application.Interfaces;
using Marte.Domain.Entities;
using Marte.Infrastructure.Interfaces;

namespace Marte.Application.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IAuthService _authService;

        public UserManagementService(
            IUsuarioRepository usuarioRepository,
            IAuditLogRepository auditLogRepository,
            IAuthService authService)
        {
            _usuarioRepository = usuarioRepository;
            _auditLogRepository = auditLogRepository;
            _authService = authService;
        }

        public async Task<IEnumerable<Usuario>> GetAllUsersAsync()
        {
            return await _usuarioRepository.GetAllAsync();
        }

        public async Task<Usuario?> GetUserByIdAsync(Guid id)
        {
            return await _usuarioRepository.GetByIdAsync(id);
        }

        public async Task<Usuario> CreateUserAsync(string nombreUsuario, string contraseña, string nombreCompleto, Guid rolId, string currentUser)
        {
            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                NombreUsuario = nombreUsuario,
                ContraseñaHash = _authService.HashPassword(contraseña),
                NombreCompleto = nombreCompleto,
                RolId = rolId,
                FechaCreacion = DateTime.Now,
                FechaModificacion = DateTime.Now,
                Estado = true
            };

            await _usuarioRepository.AddAsync(usuario);
            await _usuarioRepository.SaveChangesAsync();

            // Auditoría
            await _auditLogRepository.AddAsync(new AuditLog
            {
                Entidad = "Usuario",
                EntidadId = usuario.Id,
                Accion = "Crear Usuario",
                Usuario = currentUser,
                Fecha = DateTime.Now,
                Detalle = $"Usuario creado: {nombreUsuario} - {nombreCompleto}"
            });
            await _auditLogRepository.SaveChangesAsync();

            return usuario;
        }

        public async Task UpdateUserAsync(Guid id, string nombreCompleto, Guid rolId, string currentUser)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null)
                throw new Exception("Usuario no encontrado");

            var cambios = new List<string>();
            
            if (usuario.NombreCompleto != nombreCompleto)
            {
                cambios.Add($"Nombre: {usuario.NombreCompleto} → {nombreCompleto}");
                usuario.NombreCompleto = nombreCompleto;
            }

            if (usuario.RolId != rolId)
            {
                cambios.Add($"Rol cambiado");
                usuario.RolId = rolId;
            }

            usuario.FechaModificacion = DateTime.Now;

            await _usuarioRepository.UpdateAsync(usuario);
            await _usuarioRepository.SaveChangesAsync();

            // Auditoría
            if (cambios.Any())
            {
                await _auditLogRepository.AddAsync(new AuditLog
                {
                    Entidad = "Usuario",
                    EntidadId = usuario.Id,
                    Accion = "Actualizar Usuario",
                    Usuario = currentUser,
                    Fecha = DateTime.Now,
                    Detalle = string.Join(", ", cambios)
                });
                await _auditLogRepository.SaveChangesAsync();
            }
        }

        public async Task ChangePasswordAsync(Guid id, string nuevaContraseña, string currentUser)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null)
                throw new Exception("Usuario no encontrado");

            usuario.ContraseñaHash = _authService.HashPassword(nuevaContraseña);
            usuario.FechaModificacion = DateTime.Now;

            await _usuarioRepository.UpdateAsync(usuario);
            await _usuarioRepository.SaveChangesAsync();

            // Auditoría
            await _auditLogRepository.AddAsync(new AuditLog
            {
                Entidad = "Usuario",
                EntidadId = usuario.Id,
                Accion = "Cambio de Contraseña",
                Usuario = currentUser,
                Fecha = DateTime.Now,
                Detalle = $"Contraseña actualizada para usuario: {usuario.NombreUsuario}"
            });
            await _auditLogRepository.SaveChangesAsync();
        }

        public async Task DisableUserAsync(Guid id, string currentUser)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null)
                throw new Exception("Usuario no encontrado");

            usuario.Estado = false;
            usuario.FechaModificacion = DateTime.Now;

            await _usuarioRepository.UpdateAsync(usuario);
            await _usuarioRepository.SaveChangesAsync();

            // Auditoría
            await _auditLogRepository.AddAsync(new AuditLog
            {
                Entidad = "Usuario",
                EntidadId = usuario.Id,
                Accion = "Deshabilitar Usuario",
                Usuario = currentUser,
                Fecha = DateTime.Now,
                Detalle = $"Usuario deshabilitado: {usuario.NombreUsuario}"
            });
            await _auditLogRepository.SaveChangesAsync();
        }

        public async Task EnableUserAsync(Guid id, string currentUser)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null)
                throw new Exception("Usuario no encontrado");

            usuario.Estado = true;
            usuario.FechaModificacion = DateTime.Now;

            await _usuarioRepository.UpdateAsync(usuario);
            await _usuarioRepository.SaveChangesAsync();

            // Auditoría
            await _auditLogRepository.AddAsync(new AuditLog
            {
                Entidad = "Usuario",
                EntidadId = usuario.Id,
                Accion = "Habilitar Usuario",
                Usuario = currentUser,
                Fecha = DateTime.Now,
                Detalle = $"Usuario habilitado: {usuario.NombreUsuario}"
            });
            await _auditLogRepository.SaveChangesAsync();
        }

        public async Task<bool> UserExistsAsync(string nombreUsuario)
        {
            var usuario = await _usuarioRepository.GetByUsernameAsync(nombreUsuario);
            return usuario != null;
        }
    }
}
