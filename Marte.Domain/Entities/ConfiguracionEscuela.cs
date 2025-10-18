namespace Marte.Domain.Entities
{
    public class ConfiguracionEscuela
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string NombreFilial { get; set; } = "N. A. Primavera"; // Nombre de la filial
        public TimeSpan HoraCierre { get; set; } = new TimeSpan(22, 0, 0); // 10:00 PM por defecto
        public DateTime FechaConfiguracion { get; set; } = DateTime.Now;
    }
}
