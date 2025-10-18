using System;

namespace Marte.Domain.Entities
{
    public class Rol
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Nombre { get; set; } = string.Empty; // Admin, Guardia, etc.

        // Navegación inversa
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}