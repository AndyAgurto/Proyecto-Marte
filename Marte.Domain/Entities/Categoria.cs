namespace Marte.Domain.Entities
{
    public class Categoria
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Nombre { get; set; } = string.Empty;

        public ICollection<Asistente> Asistentes { get; set; } = new List<Asistente>();
    }
}
