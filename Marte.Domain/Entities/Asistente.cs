using System;
namespace Marte.Domain.Entities
{
    public class Asistente
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string DNI { get; set; } = string.Empty;

        public Guid CategoriaId { get; set; }
        public Categoria Categoria { get; set; } = null!;

        // Número de Grupo (solo aplicable para categoría "Miembros")
        public string? NumeroGrupo { get; set; }

        public bool Estado { get; set; } = true; // Activo por defecto

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime? FechaModificacion { get; set; }

        public ICollection<Asistencia> Asistencias { get; set; } = new List<Asistencia>();
    }
}
