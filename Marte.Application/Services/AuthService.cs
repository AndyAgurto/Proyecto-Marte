using System.Threading.Tasks;
using Marte.Application.Interfaces;
using Marte.Application.Models;
using Marte.Domain.Entities;
using Marte.Infrastructure.Interfaces;
using BCrypt.Net;

namespace Marte.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IAuditLogRepository _auditLogRepository;

        public AuthService(IUsuarioRepository usuarioRepository, IAuditLogRepository auditLogRepository)
        {
            _usuarioRepository = usuarioRepository;
            _auditLogRepository = auditLogRepository;
        }

        public async Task<Usuario?> LoginAsync(LoginRequest request)
        {
            // Buscar usuario por nombre de usuario
            var user = await _usuarioRepository.GetByUsernameAsync(request.NombreUsuario);

            if (user == null)
                return null;

            if (!user.Estado)
                return null;

            // Validar hash
            bool valid = VerifyPassword(request.Contraseña, user.ContraseñaHash);
            
            if (valid)
            {
                // Registrar inicio de sesión en auditoría
                await _auditLogRepository.AddAsync(new AuditLog
                {
                    Entidad = "Usuario",
                    EntidadId = user.Id,
                    Accion = "Inicio de Sesión",
                    Usuario = user.NombreUsuario,
                    Fecha = DateTime.Now,
                    Detalle = $"Inicio de sesión exitoso - IP: Local"
                });
                await _auditLogRepository.SaveChangesAsync();
            }
            
            return valid ? user : null;
        }

        public Usuario? Login(LoginRequest request)
        {
            return LoginAsync(request).GetAwaiter().GetResult();
        }

        public async Task LogoutAsync(string usuario)
        {
            var user = await _usuarioRepository.GetByUsernameAsync(usuario);
            if (user != null)
            {
                await _auditLogRepository.AddAsync(new AuditLog
                {
                    Entidad = "Usuario",
                    EntidadId = user.Id,
                    Accion = "Cierre de Sesión",
                    Usuario = usuario,
                    Fecha = DateTime.Now,
                    Detalle = "Cierre de sesión del sistema"
                });
                await _auditLogRepository.SaveChangesAsync();
            }
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}