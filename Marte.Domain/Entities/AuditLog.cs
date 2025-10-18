namespace Marte.Domain.Entities
{
    public class AuditLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Entidad { get; set; } = string.Empty;
        public Guid EntidadId { get; set; }
        public string Accion { get; set; } = string.Empty; // Ejemplo: "Actualizar Categoría"
        public string Usuario { get; set; } = string.Empty;
        public DateTime Fecha { get; set; } = DateTime.Now;
        public string? Detalle { get; set; }
    }
}
