namespace Marte.Application.Models
{
    public class DashboardStatsDto
    {
        public int AsistentesHoy { get; set; }
        public int HastaCierreAyer { get; set; }
        public double PorcentajePuntualidad { get; set; }
        public int TotalRegistrados { get; set; }
        
        // Asistentes por categoría
        public int MiembrosGrupo1 { get; set; }
        public int MiembrosGrupo2 { get; set; }
        public int MiembrosGrupo3 { get; set; }
        
        // Top 5 más constantes
        public List<TopAsistenteDto> TopAsistentes { get; set; } = new();
    }

    public class TopAsistenteDto
    {
        public int Posicion { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public int TotalAsistencias { get; set; }
        public int DiasDisponibles { get; set; }
        public double Porcentaje { get; set; }
    }
}
