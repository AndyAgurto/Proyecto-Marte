using System;

namespace Marte.Domain.Entities
{
    public class Usuario
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string NombreUsuario { get; set; } = string.Empty;
        public string ContraseñaHash { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public bool Estado { get; set; }

        // Relacion con Rol
        public Guid RolId { get; set; }
        public Rol Rol { get; set; }

    }
}